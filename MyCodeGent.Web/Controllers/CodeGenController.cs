using Microsoft.AspNetCore.Mvc;
using MyCodeGent.Core.Interfaces;
using MyCodeGent.Web.Models;
using System.IO.Compression;
using TemplateModels = MyCodeGent.Templates.Models;

namespace MyCodeGent.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CodeGenController : ControllerBase
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly IFileWriter _fileWriter;
    private readonly ILogger<CodeGenController> _logger;

    public CodeGenController(
        ICodeGenerator codeGenerator,
        IFileWriter fileWriter,
        ILogger<CodeGenController> logger)
    {
        _codeGenerator = codeGenerator;
        _fileWriter = fileWriter;
        _logger = logger;
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

            _logger.LogInformation("Starting code generation for {Count} entities. Session: {SessionId}, OutputPath: {OutputPath}", 
                request.Entities.Count, sessionId, webConfig.OutputPath);

            var generatedFiles = new List<GeneratedFile>();
            
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
                }

                // Generate common files
                _logger.LogDebug("Generating common files...");
                await GenerateCommonFilesAsync(templateEntities, templateConfig);

                // Collect all generated files
                _logger.LogDebug("Collecting generated files from: {OutputPath}", webConfig.OutputPath);
                generatedFiles = await CollectGeneratedFilesAsync(webConfig.OutputPath);
                
                _logger.LogInformation("Code generation completed successfully. Generated {FileCount} files", generatedFiles.Count);
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
                downloadUrl = $"/api/codegen/download/{sessionId}"
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

            var zipPath = Path.Combine(Path.GetTempPath(), $"MyCodeGent_{sessionId}.zip");
            
            if (System.IO.File.Exists(zipPath))
            {
                System.IO.File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(outputPath, zipPath);

            var bytes = await System.IO.File.ReadAllBytesAsync(zipPath);
            
            // Cleanup
            System.IO.File.Delete(zipPath);

            return File(bytes, "application/zip", $"generated-code-{sessionId}.zip");
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

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new 
        { 
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            services = new
            {
                codeGenerator = _codeGenerator != null ? "OK" : "MISSING",
                fileWriter = _fileWriter != null ? "OK" : "MISSING"
            }
        });
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
        var interfacePath = Path.Combine(config.OutputPath, "Application", "Common", "Interfaces");
        var interfaceCode = MyCodeGent.Templates.InfrastructureTemplate.GenerateApplicationDbContextInterface(entities, config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(interfacePath, "IApplicationDbContext.cs"), interfaceCode);

        var dbContextPath = Path.Combine(config.OutputPath, "Infrastructure", "Persistence");
        var dbContextCode = MyCodeGent.Templates.InfrastructureTemplate.GenerateDbContext(entities, config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(dbContextPath, "ApplicationDbContext.cs"), dbContextCode);

        var modelsPath = Path.Combine(config.OutputPath, "Application", "Common", "Models");
        var pagedResultCode = MyCodeGent.Templates.InfrastructureTemplate.GeneratePagedResult(config.RootNamespace);
        await _fileWriter.WriteFileAsync(Path.Combine(modelsPath, "PagedResult.cs"), pagedResultCode);
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
                DefaultValue = p.DefaultValue
            }).ToList()
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

public class GeneratedFile
{
    public string Path { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Size { get; set; }
}
