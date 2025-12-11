using Microsoft.AspNetCore.Mvc;
using MyCodeGent.Core.Interfaces;
using MyCodeGent.Web.Models;
using MyCodeGent.Web.Services;
using System.IO.Compression;
using System.Runtime.InteropServices;
using TemplateModels = MyCodeGent.Templates.Models;

namespace MyCodeGent.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CodeGenController : ControllerBase
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly IFileWriter _fileWriter;
    private readonly ILogger<CodeGenController> _logger;
    private readonly IVersionService _versionService;
    private readonly MyCodeGent.Core.Services.IGitService _gitService;

    public CodeGenController(
        ICodeGenerator codeGenerator,
        IFileWriter fileWriter,
        ILogger<CodeGenController> logger,
        IVersionService versionService,
        MyCodeGent.Core.Services.IGitService gitService)
    {
        _codeGenerator = codeGenerator;
        _fileWriter = fileWriter;
        _logger = logger;
        _versionService = versionService;
        _gitService = gitService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateRequest request)
    {
        try
        {
            // Validate request
            if (request == null)
            {
                _logger.LogWarning("Generate request is null");
                return BadRequest(new { error = "Request body is required" });
            }

            if (request.Entities == null || request.Entities.Count == 0)
            {
                _logger.LogWarning("No entities provided in request");
                return BadRequest(new { error = "No entities provided" });
            }

            // Validate entities
            foreach (var entity in request.Entities)
            {
                if (string.IsNullOrWhiteSpace(entity.Name))
                {
                    return BadRequest(new { error = "Entity name is required" });
                }
                if (entity.Properties == null || entity.Properties.Count == 0)
                {
                    return BadRequest(new { error = $"Entity '{entity.Name}' must have at least one property" });
                }
            }

            var webConfig = request.Config ?? new GenerationConfig();
            var sessionId = Guid.NewGuid().ToString();
            webConfig.OutputPath = Path.Combine(Path.GetTempPath(), "MyCodeGent", sessionId);
            
            // Store project name for later use in download
            var projectName = webConfig.RootNamespace ?? "GeneratedProject";
            var metadataPath = Path.Combine(webConfig.OutputPath, ".metadata");
            Directory.CreateDirectory(webConfig.OutputPath);
            await System.IO.File.WriteAllTextAsync(metadataPath, projectName);

            _logger.LogInformation("Starting code generation for {Count} entities. Session: {SessionId}, OutputPath: {OutputPath}", 
                request.Entities.Count, sessionId, webConfig.OutputPath);

            var generatedFiles = new List<GeneratedFile>();
            var gitInitialized = false;
            var gitCommitted = false;
            
            try
            {
                // Convert to template models
                _logger.LogDebug("Converting to template models...");
                var templateConfig = ConvertToTemplateConfig(webConfig);
                var templateEntities = request.Entities.Select(e =>
                {
                    var te = ConvertToTemplateModel(e);
                    te.Namespace = templateConfig.RootNamespace;
                    return te;
                }).ToList();

                // Generate code for each entity
                foreach (var entity in templateEntities)
                {
                    _logger.LogInformation("Generating code for entity: {EntityName}", entity.Name);
                    await _codeGenerator.GenerateAsync(entity, templateConfig);
                    
                    // Generate unit tests for entity
                    await _codeGenerator.GenerateTestsAsync(entity, templateConfig);
                }

                // Generate common files
                _logger.LogDebug("Generating common files...");
                await GenerateCommonFilesAsync(templateEntities, templateConfig);
                
                // Generate application infrastructure (Program.cs, appsettings, project files, docs)
                _logger.LogDebug("Generating application infrastructure...");
                await _codeGenerator.GenerateApplicationInfrastructureAsync(templateEntities, templateConfig);

                // Collect all generated files
                _logger.LogDebug("Collecting generated files from: {OutputPath}", webConfig.OutputPath);
                generatedFiles = await CollectGeneratedFilesAsync(webConfig.OutputPath);
                
                _logger.LogInformation("Code generation completed successfully. Generated {FileCount} files", generatedFiles.Count);
                
                // Initialize Git and commit
                _logger.LogDebug("Initializing Git repository...");
                
                if (_gitService.IsGitInstalled())
                {
                    gitInitialized = await _gitService.InitializeRepositoryAsync(webConfig.OutputPath);
                    
                    if (gitInitialized)
                    {
                        var commitMessage = $"Initial commit: Generated {templateEntities.Count} entities with MyCodeGent\n\n" +
                                          $"Entities: {string.Join(", ", templateEntities.Select(e => e.Name))}\n" +
                                          $"Generated: {generatedFiles.Count} files\n" +
                                          $"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
                        
                        gitCommitted = await _gitService.CommitChangesAsync(webConfig.OutputPath, commitMessage);
                    }
                }
                else
                {
                    _logger.LogWarning("Git is not installed. Skipping repository initialization.");
                }
            }
            catch (Exception genEx)
            {
                _logger.LogError(genEx, "Error during code generation process. SessionId: {SessionId}", sessionId);
                throw; // Re-throw to be caught by outer catch
            }

            return Ok(new
            {
                sessionId,
                filesGenerated = generatedFiles.Count,
                files = generatedFiles,
                downloadUrl = $"/api/codegen/download/{sessionId}",
                git = new
                {
                    initialized = gitInitialized,
                    committed = gitCommitted
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating code. Exception Type: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}", 
                ex.GetType().Name, ex.Message, ex.StackTrace);
            
            return StatusCode(500, new 
            { 
                error = "Error generation failed",
                message = ex.Message,
                type = ex.GetType().Name,
                details = ex.InnerException?.Message
            });
        }
    }

    [HttpGet("download/{sessionId}")]
    public async Task<IActionResult> Download(string sessionId)
    {
        try
        {
            var outputPath = Path.Combine(Path.GetTempPath(), "MyCodeGent", sessionId);
            
            if (!Directory.Exists(outputPath))
            {
                return NotFound(new { error = "Session not found or expired" });
            }
            
            // Read project name from metadata
            var metadataPath = Path.Combine(outputPath, ".metadata");
            var projectName = "GeneratedProject";
            if (System.IO.File.Exists(metadataPath))
            {
                projectName = await System.IO.File.ReadAllTextAsync(metadataPath);
                // Delete metadata file so it's not included in ZIP
                System.IO.File.Delete(metadataPath);
            }

            var zipPath = Path.Combine(Path.GetTempPath(), $"{projectName}_{sessionId}.zip");
            
            if (System.IO.File.Exists(zipPath))
            {
                System.IO.File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(outputPath, zipPath);

            var bytes = await System.IO.File.ReadAllBytesAsync(zipPath);
            
            // Cleanup
            System.IO.File.Delete(zipPath);

            // Sanitize filename for download (remove invalid characters)
            var safeFileName = string.Join("_", projectName.Split(Path.GetInvalidFileNameChars()));
            
            // Set proper Content-Disposition header for download managers
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{safeFileName}.zip\"");
            
            return File(bytes, "application/zip", $"{safeFileName}.zip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading generated code");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("preview")]
    public IActionResult Preview([FromBody] PreviewRequest request)
    {
        try
        {
            if (request.Entity == null)
            {
                return BadRequest(new { error = "Entity is required" });
            }

            var webConfig = request.Config ?? new GenerationConfig();
            var templateConfig = ConvertToTemplateConfig(webConfig);
            var templateEntity = ConvertToTemplateModel(request.Entity);
            templateEntity.Namespace = templateConfig.RootNamespace;

            var previews = new Dictionary<string, string>();

            // Generate previews for each layer
            if (templateConfig.GenerateDomain)
            {
                previews["Domain/Entity"] = MyCodeGent.Templates.EntityTemplate.Generate(templateEntity);
            }

            if (templateConfig.GenerateApplication)
            {
                previews["Application/Dto"] = MyCodeGent.Templates.DtoTemplate.Generate(templateEntity);
                previews["Application/CreateCommand"] = MyCodeGent.Templates.CommandTemplate.GenerateCreateCommand(templateEntity);
                previews["Application/UpdateCommand"] = MyCodeGent.Templates.CommandTemplate.GenerateUpdateCommand(templateEntity);
                previews["Application/DeleteCommand"] = MyCodeGent.Templates.CommandTemplate.GenerateDeleteCommand(templateEntity);
                previews["Application/GetByIdQuery"] = MyCodeGent.Templates.QueryTemplate.GenerateGetByIdQuery(templateEntity);
                previews["Application/GetAllQuery"] = MyCodeGent.Templates.QueryTemplate.GenerateGetAllQuery(templateEntity);
            }

            if (templateConfig.GenerateInfrastructure)
            {
                previews["Infrastructure/EntityConfiguration"] = MyCodeGent.Templates.InfrastructureTemplate.GenerateEntityConfiguration(templateEntity);
            }

            if (templateConfig.GenerateApi)
            {
                previews["Api/Controller"] = MyCodeGent.Templates.ControllerTemplate.Generate(templateEntity);
            }

            return Ok(new { previews });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating preview");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("generate-incremental")]
    public async Task<IActionResult> GenerateIncremental([FromBody] IncrementalGenerateRequest request)
    {
        try
        {
            // Validate request
            if (request == null)
            {
                return BadRequest(new { error = "Request body is required" });
            }

            if (string.IsNullOrWhiteSpace(request.ProjectPath))
            {
                return BadRequest(new { error = "ProjectPath is required for incremental generation" });
            }

            if (!Directory.Exists(request.ProjectPath))
            {
                return BadRequest(new { error = $"Project path not found: {request.ProjectPath}" });
            }

            if (request.NewEntities == null || request.NewEntities.Count == 0)
            {
                return BadRequest(new { error = "No new entities provided" });
            }

            // Validate entities
            foreach (var entity in request.NewEntities)
            {
                if (string.IsNullOrWhiteSpace(entity.Name))
                {
                    return BadRequest(new { error = "Entity name is required" });
                }
                if (entity.Properties == null || entity.Properties.Count == 0)
                {
                    return BadRequest(new { error = $"Entity '{entity.Name}' must have at least one property" });
                }
            }

            var webConfig = request.Config ?? new GenerationConfig();
            
            _logger.LogInformation("Starting incremental generation for {Count} entities. ProjectPath: {ProjectPath}", 
                request.NewEntities.Count, request.ProjectPath);

            // Convert to template models
            var templateConfig = ConvertToTemplateConfig(webConfig);
            templateConfig.OutputPath = request.ProjectPath; // Use existing project path
            
            var templateEntities = request.NewEntities.Select(e =>
            {
                var te = ConvertToTemplateModel(e);
                te.Namespace = templateConfig.RootNamespace;
                return te;
            }).ToList();

            // Use incremental generator
            var incrementalGenerator = new MyCodeGent.Core.Services.IncrementalCodeGenerator(_fileWriter);
            var result = await incrementalGenerator.GenerateIncrementalAsync(
                templateEntities, 
                templateConfig, 
                request.ProjectPath);

            _logger.LogInformation("Incremental generation completed. New files: {NewFiles}, Updated files: {UpdatedFiles}", 
                result.NewFiles.Count, result.UpdatedFiles.Count);

            return Ok(new
            {
                success = true,
                entitiesAdded = result.EntitiesAdded,
                newFilesCount = result.NewFiles.Count,
                updatedFilesCount = result.UpdatedFiles.Count,
                newFiles = result.NewFiles.Select(f => Path.GetRelativePath(request.ProjectPath, f)).ToList(),
                updatedFiles = result.UpdatedFiles.Select(f => Path.GetRelativePath(request.ProjectPath, f)).ToList(),
                message = $"Successfully added {result.EntitiesAdded.Count} new entities to existing project"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during incremental generation");
            return StatusCode(500, new 
            { 
                error = "Incremental generation failed",
                message = ex.Message,
                type = ex.GetType().Name
            });
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new 
        { 
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "2.0.0",
            services = new
            {
                codeGenerator = _codeGenerator != null ? "OK" : "MISSING",
                fileWriter = _fileWriter != null ? "OK" : "MISSING"
            }
        });
    }
    
    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        try
        {
            var versionInfo = _versionService.GetVersionInfo();
            
            return Ok(new
            {
                version = versionInfo.Version ?? "2.0.0",
                releaseDate = versionInfo.BuildDate.ToString("yyyy-MM-dd"),
                buildNumber = versionInfo.BuildNumber ?? "2024.12.10.001",
                buildDate = versionInfo.BuildDate,
                codeName = "Complete Application Generator",
                dotnetVersion = versionInfo.DotNetVersion ?? "9.0",
                runtimeVersion = versionInfo.RuntimeVersion ?? ".NET 9.0",
                osDescription = versionInfo.OsDescription ?? "Unknown",
                processArchitecture = versionInfo.ProcessArchitecture ?? "X64",
                lastUpdated = versionInfo.LastUpdated ?? "December 2024",
                git = new
                {
                    branch = versionInfo.GitBranch ?? "",
                    commitHash = versionInfo.GitCommitHash ?? "",
                    commitDate = versionInfo.GitCommitDate,
                    commitMessage = versionInfo.GitCommitMessage ?? "",
                    commitAuthor = versionInfo.GitCommitAuthor ?? "",
                    tag = versionInfo.GitTag ?? "",
                    isClean = versionInfo.GitIsClean,
                    commitCount = versionInfo.GitCommitCount
                },
                features = new
                {
                    completeAppGeneration = true,
                    incrementalGeneration = true,
                    databaseProviders = new[] { "SqlServer", "PostgreSql", "MySql", "Sqlite", "InMemory" },
                    authentication = new[] { "JWT", "Identity", "AzureAD", "Auth0" },
                    logging = new[] { "Serilog", "NLog", "Default", "ApplicationInsights" },
                    documentation = true,
                    swagger = true,
                    healthChecks = true,
                    propertyTypes = 15,
                    validation = true
                },
                packages = new
                {
                    MediatR = "12.4.1",
                    FluentValidation = "11.11.0",
                    AutoMapper = "13.0.1",
                    Swashbuckle = "7.2.0",
                    EntityFrameworkCore = "9.0.0",
                    Serilog = "8.0.3"
                },
                generatedOutput = new
                {
                    targetFramework = "net9.0",
                    architecturePattern = "Clean Architecture + CQRS",
                    layers = new[] { "Domain", "Application", "Infrastructure", "API" },
                    includesTests = false,
                    includesDocker = false,
                    includesMigrations = true
                },
                enterpriseFeatures = new
                {
                    propertyConstraints = true,
                    relationships = true,
                    businessKeys = true,
                    visualDiagram = true,
                    incrementalUpdates = true
                },
                keepUpToDate = new
                {
                    dotnetSdk = "Download from https://dotnet.microsoft.com/download",
                    nugetPackages = "Check https://www.nuget.org for latest versions",
                    efCoreTools = "dotnet tool update --global dotnet-ef",
                    templateUpdates = "Update package versions in InfrastructureTemplate.cs"
                }
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                version = "2.0.0",
                releaseDate = DateTime.Now.ToString("yyyy-MM-dd"),
                buildNumber = "2024.12.10.001",
                buildDate = DateTime.Now,
                codeName = "Complete Application Generator",
                dotnetVersion = "9.0",
                runtimeVersion = ".NET 9.0",
                osDescription = Environment.OSVersion.ToString(),
                processArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                lastUpdated = "December 2024",
                git = new
                {
                    branch = "",
                    commitHash = "",
                    commitDate = DateTime.MinValue,
                    commitMessage = "",
                    commitAuthor = "",
                    tag = "",
                    isClean = true,
                    commitCount = 0
                },
                features = new
                {
                    completeAppGeneration = true,
                    incrementalGeneration = true,
                    databaseProviders = new[] { "SqlServer", "PostgreSql", "MySql", "Sqlite", "InMemory" },
                    authentication = new[] { "JWT", "Identity", "AzureAD", "Auth0" },
                    logging = new[] { "Serilog", "NLog", "Default", "ApplicationInsights" },
                    documentation = true,
                    swagger = true,
                    healthChecks = true,
                    propertyTypes = 15,
                    validation = true
                },
                packages = new
                {
                    MediatR = "12.4.1",
                    FluentValidation = "11.11.0",
                    AutoMapper = "13.0.1",
                    Swashbuckle = "7.2.0",
                    EntityFrameworkCore = "9.0.0",
                    Serilog = "8.0.3"
                },
                generatedOutput = new
                {
                    targetFramework = "net9.0",
                    architecturePattern = "Clean Architecture + CQRS",
                    layers = new[] { "Domain", "Application", "Infrastructure", "API" },
                    includesTests = false,
                    includesDocker = false,
                    includesMigrations = true
                },
                enterpriseFeatures = new
                {
                    propertyConstraints = true,
                    relationships = true,
                    businessKeys = true,
                    visualDiagram = true,
                    incrementalUpdates = true
                },
                keepUpToDate = new
                {
                    dotnetSdk = "Download from https://dotnet.microsoft.com/download",
                    nugetPackages = "Check https://www.nuget.org for latest versions",
                    efCoreTools = "dotnet tool update --global dotnet-ef",
                    templateUpdates = "Update package versions in InfrastructureTemplate.cs"
                },
                error = ex.Message
            });
        }
    }

    [HttpGet("sample-config")]
    public IActionResult GetSampleConfig()
    {
        var sampleConfig = new GenerateRequest
        {
            Config = new GenerationConfig
            {
                OutputPath = "./Generated",
                RootNamespace = "MyApp",
                GenerateApi = true,
                GenerateApplication = true,
                GenerateDomain = true,
                GenerateInfrastructure = true,
                UseMediator = true,
                UseFluentValidation = true,
                UseAutoMapper = true,
                DatabaseProvider = DatabaseProvider.SqlServer
            },
            Entities = new List<EntityModel>
            {
                new EntityModel
                {
                    Name = "Product",
                    HasAuditFields = true,
                    HasSoftDelete = true,
                    Properties = new List<PropertyModel>
                    {
                        new PropertyModel { Name = "Id", Type = "int", IsKey = true, IsRequired = true },
                        new PropertyModel { Name = "Name", Type = "string", IsRequired = true, MaxLength = 200 },
                        new PropertyModel { Name = "Description", Type = "string", IsNullable = true, MaxLength = 1000 },
                        new PropertyModel { Name = "Price", Type = "decimal", IsRequired = true },
                        new PropertyModel { Name = "Stock", Type = "int", IsRequired = true }
                    }
                }
            }
        };

        return Ok(sampleConfig);
    }

    private async Task GenerateCommonFilesAsync(List<TemplateModels.EntityModel> entities, TemplateModels.GenerationConfig config)
    {
        var interfacePath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Application", "Common", "Interfaces");
        var interfaceCode = MyCodeGent.Templates.InfrastructureTemplate.GenerateApplicationDbContextInterface(entities, config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(interfacePath, "IApplicationDbContext.cs"), interfaceCode);

        var dbContextPath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Infrastructure", "Persistence");
        var dbContextCode = MyCodeGent.Templates.InfrastructureTemplate.GenerateDbContext(entities, config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(dbContextPath, "ApplicationDbContext.cs"), dbContextCode);

        // Generate Master AutoMapper Profile
        if (config.UseAutoMapper)
        {
            var mappingPath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Application", "Mappings");
            var masterProfile = MyCodeGent.Templates.MappingProfileTemplate.GenerateMasterProfile(entities, config.RootNamespace);
            await _fileWriter.WriteFileAsync(Path.Combine(mappingPath, "MappingProfile.cs"), masterProfile);
            _logger.LogInformation("Generated Master AutoMapper Profile");
        }
        
        // Generate Seed Data
        var seedDataPath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Infrastructure", "Persistence");
        var seedData = MyCodeGent.Templates.SeedDataTemplate.Generate(entities, config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(seedDataPath, "ApplicationDbContextSeed.cs"), seedData);
        _logger.LogInformation("Generated Seed Data");
        
        // Generate Health Checks
        var healthChecksPath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Infrastructure", "HealthChecks");
        var healthCheck = MyCodeGent.Templates.InfrastructureConfigTemplate.GenerateHealthChecks(config.RootNamespace, config.DatabaseProvider);
        await _fileWriter.WriteFileAsync(Path.Combine(healthChecksPath, "DatabaseHealthCheck.cs"), healthCheck);
        _logger.LogInformation("Generated Health Checks");
        
        // Generate Exception Handling Middleware
        var middlewarePath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Api", "Middleware");
        var exceptionMiddleware = MyCodeGent.Templates.InfrastructureConfigTemplate.GenerateExceptionMiddleware(config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(middlewarePath, "ExceptionHandlingMiddleware.cs"), exceptionMiddleware);
        _logger.LogInformation("Generated Exception Handling Middleware");
        
        // Generate CORS Configuration
        var configPath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Api", "Configuration");
        var corsConfig = MyCodeGent.Templates.InfrastructureConfigTemplate.GenerateCorsConfiguration(config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(configPath, "CorsConfiguration.cs"), corsConfig);
        _logger.LogInformation("Generated CORS Configuration");
        
        // Generate Logging Configuration
        var loggingConfig = MyCodeGent.Templates.InfrastructureConfigTemplate.GenerateLoggingConfiguration(config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(configPath, "LoggingConfiguration.cs"), loggingConfig);
        _logger.LogInformation("Generated Logging Configuration");
        
        // Generate appsettings.json
        var appSettings = MyCodeGent.Templates.InfrastructureConfigTemplate.GenerateAppSettingsJson(config.RootNamespace, config.DatabaseProvider);
        await _fileWriter.WriteFileAsync(Path.Combine(config.OutputPath, $"{config.RootNamespace}.Api", "appsettings.json"), appSettings);
        _logger.LogInformation("Generated appsettings.json");
        
        // Generate Swagger Configuration
        var swaggerConfig = MyCodeGent.Templates.SwaggerTemplate.GenerateSwaggerConfiguration(config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(configPath, "SwaggerConfiguration.cs"), swaggerConfig);
        _logger.LogInformation("Generated Swagger Configuration");
        
        // Generate Pagination Models
        var modelsPath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Application", "Common", "Models");
        var pagedResultCode = MyCodeGent.Templates.PaginationTemplate.GeneratePagedResult(config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(modelsPath, "PagedResult.cs"), pagedResultCode);
        
        var pagedQueryCode = MyCodeGent.Templates.PaginationTemplate.GeneratePagedQuery(config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(modelsPath, "PagedQuery.cs"), pagedQueryCode);
        _logger.LogInformation("Generated Pagination Models");
        
        // Generate Test Project
        var testProjectPath = Path.Combine(config.OutputPath, "Tests", "Application.Tests");
        var testProject = MyCodeGent.Templates.TestTemplate.GenerateTestProject(config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(testProjectPath, $"{config.RootNamespace}.Application.Tests.csproj"), testProject);
        _logger.LogInformation("Generated Test Project");
        
        // Generate File Upload Service
        var fileStorageInterface = MyCodeGent.Templates.FileUploadTemplate.GenerateFileStorageService(config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(interfacePath, "IFileStorageService.cs"), fileStorageInterface);
        
        var fileStorageService = MyCodeGent.Templates.FileUploadTemplate.GenerateLocalFileStorageService(config.RootNamespace);
        var servicesPath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Infrastructure", "Services");
        await _fileWriter.WriteFileAsync(Path.Combine(servicesPath, "LocalFileStorageService.cs"), fileStorageService);
        
        var filesController = MyCodeGent.Templates.FileUploadTemplate.GenerateFileUploadController(config.RootNamespace);
        var controllersPath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Api", "Controllers");
        await _fileWriter.WriteFileAsync(Path.Combine(controllersPath, "FilesController.cs"), filesController);
        _logger.LogInformation("Generated File Upload Service");
        
        // Generate Audit Logging
        var auditEntity = MyCodeGent.Templates.AuditTemplate.GenerateAuditLogEntity(config.RootNamespace);
        var domainPath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Domain", "Entities");
        await _fileWriter.WriteFileAsync(Path.Combine(domainPath, "AuditLog.cs"), auditEntity);
        
        var auditConfig = MyCodeGent.Templates.AuditTemplate.GenerateAuditLogConfiguration(config.RootNamespace);
        var configurationsPath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Infrastructure", "Persistence", "Configurations");
        await _fileWriter.WriteFileAsync(Path.Combine(configurationsPath, "AuditLogConfiguration.cs"), auditConfig);
        
        var auditService = MyCodeGent.Templates.AuditTemplate.GenerateAuditService(config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(servicesPath, "AuditService.cs"), auditService);
        
        var auditController = MyCodeGent.Templates.AuditTemplate.GenerateAuditController(config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(controllersPath, "AuditController.cs"), auditController);
        _logger.LogInformation("Generated Audit Logging System");
        
        // Generate Quick Start Guides
        var vsQuickStart = MyCodeGent.Templates.QuickStartTemplate.GenerateVisualStudioGuide(config);
        await _fileWriter.WriteFileAsync(Path.Combine(config.OutputPath, "QUICKSTART-VISUAL-STUDIO.md"), vsQuickStart);
        
        var vscodeQuickStart = MyCodeGent.Templates.QuickStartTemplate.GenerateVSCodeGuide(config);
        await _fileWriter.WriteFileAsync(Path.Combine(config.OutputPath, "QUICKSTART-VSCODE.md"), vscodeQuickStart);
        _logger.LogInformation("Generated Quick Start Guides");
        
        // Generate Solution File
        var solutionFile = MyCodeGent.Templates.SolutionTemplate.GenerateSolutionFile(config);
        await _fileWriter.WriteFileAsync(Path.Combine(config.OutputPath, $"{config.RootNamespace}.sln"), solutionFile);
        _logger.LogInformation("Generated Solution File");
    }

    private async Task<List<GeneratedFile>> CollectGeneratedFilesAsync(string rootPath)
    {
        var files = new List<GeneratedFile>();
        
        if (!Directory.Exists(rootPath))
        {
            return files;
        }

        foreach (var filePath in Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(rootPath, filePath);
            var content = await System.IO.File.ReadAllTextAsync(filePath);
            
            files.Add(new GeneratedFile
            {
                Path = relativePath.Replace("\\", "/"),
                Content = content,
                Size = content.Length
            });
        }

        return files;
    }
    
    private TemplateModels.EntityModel ConvertToTemplateModel(EntityModel webModel)
    {
        return new TemplateModels.EntityModel
        {
            Name = webModel.Name,
            Namespace = webModel.Namespace,
            HasAuditFields = webModel.HasAuditFields,
            HasSoftDelete = webModel.HasSoftDelete,
            Properties = webModel.Properties.Select(p => new TemplateModels.PropertyModel
            {
                Name = p.Name,
                Type = p.Type,
                IsRequired = p.IsRequired,
                IsKey = p.IsKey,
                IsNullable = p.IsNullable,
                MaxLength = p.MaxLength,
                DefaultValue = p.DefaultValue,
                Constraints = p.Constraints != null ? new TemplateModels.PropertyConstraints
                {
                    MinLength = p.Constraints.MinLength,
                    MaxLength = p.Constraints.MaxLength,
                    MinValue = p.Constraints.MinValue,
                    MaxValue = p.Constraints.MaxValue,
                    RegexPattern = p.Constraints.RegexPattern,
                    IsUnique = p.Constraints.IsUnique,
                    IsIndexed = p.Constraints.IsIndexed,
                    Precision = p.Constraints.Precision,
                    Scale = p.Constraints.Scale
                } : null
            }).ToList(),
            Relationships = webModel.Relationships?.Select(r => new TemplateModels.RelationshipModel
            {
                RelatedEntity = r.RelatedEntity,
                Type = r.Type,
                ForeignKeyProperty = r.ForeignKeyProperty,
                NavigationProperty = r.NavigationProperty,
                InverseNavigationProperty = r.InverseNavigationProperty
            }).ToList() ?? new List<TemplateModels.RelationshipModel>(),
            BusinessKeys = webModel.BusinessKeys ?? new List<string>()
        };
    }
    
    private TemplateModels.GenerationConfig ConvertToTemplateConfig(GenerationConfig webConfig)
    {
        return new TemplateModels.GenerationConfig
        {
            OutputPath = webConfig.OutputPath,
            RootNamespace = webConfig.RootNamespace,
            GenerateApi = webConfig.GenerateApi,
            GenerateApplication = webConfig.GenerateApplication,
            GenerateDomain = webConfig.GenerateDomain,
            GenerateInfrastructure = webConfig.GenerateInfrastructure,
            UseMediator = webConfig.UseMediator,
            UseFluentValidation = webConfig.UseFluentValidation,
            UseAutoMapper = webConfig.UseAutoMapper,
            DatabaseProvider = (TemplateModels.DatabaseProvider)webConfig.DatabaseProvider
        };
    }
}

public class GenerateRequest
{
    public GenerationConfig? Config { get; set; }
    public List<EntityModel> Entities { get; set; } = new();
}

public class PreviewRequest
{
    public GenerationConfig? Config { get; set; }
    public EntityModel? Entity { get; set; }
}

public class IncrementalGenerateRequest
{
    public string ProjectPath { get; set; } = string.Empty;
    public GenerationConfig? Config { get; set; }
    public List<EntityModel> NewEntities { get; set; } = new();
}

public class GeneratedFile
{
    public string Path { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Size { get; set; }
}
