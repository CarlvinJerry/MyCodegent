using MyCodeGent.Core.Interfaces;
using MyCodeGent.Templates.Models;
using MyCodeGent.Templates;

namespace MyCodeGent.Core.Services;

public class CodeGenerator : ICodeGenerator
{
    private readonly IFileWriter _fileWriter;
    
    public CodeGenerator(IFileWriter fileWriter)
    {
        _fileWriter = fileWriter;
    }
    
    public async Task GenerateAsync(EntityModel entity, GenerationConfig config)
    {
        Console.WriteLine($"Generating code for entity: {entity.Name}");
        
        // Generate Domain Layer
        if (config.GenerateDomain)
        {
            await GenerateDomainAsync(entity, config);
        }
        
        // Generate Application Layer
        if (config.GenerateApplication)
        {
            await GenerateApplicationAsync(entity, config);
        }
        
        // Generate Infrastructure Layer
        if (config.GenerateInfrastructure)
        {
            await GenerateInfrastructureAsync(entity, config);
        }
        
        // Generate API Layer
        if (config.GenerateApi)
        {
            await GenerateApiAsync(entity, config);
        }
        
        Console.WriteLine($"✓ Code generation completed for {entity.Name}");
    }
    
    private async Task GenerateDomainAsync(EntityModel entity, GenerationConfig config)
    {
        var domainPath = Path.Combine(config.OutputPath, "Domain", "Entities");
        await _fileWriter.CreateDirectoryAsync(domainPath);
        
        var entityCode = EntityTemplate.Generate(entity);
        var entityFile = Path.Combine(domainPath, $"{entity.Name}.cs");
        await _fileWriter.WriteFileAsync(entityFile, entityCode);
        
        Console.WriteLine($"  ✓ Generated Domain Entity: {entity.Name}.cs");
    }
    
    private async Task GenerateApplicationAsync(EntityModel entity, GenerationConfig config)
    {
        var appPath = Path.Combine(config.OutputPath, "Application", $"{entity.Name}s");
        
        // Generate DTO
        var dtoCode = DtoTemplate.Generate(entity);
        var dtoFile = Path.Combine(appPath, $"{entity.Name}Dto.cs");
        await _fileWriter.WriteFileAsync(dtoFile, dtoCode);
        Console.WriteLine($"  ✓ Generated DTO: {entity.Name}Dto.cs");
        
        // Generate AutoMapper Profile
        if (config.UseAutoMapper)
        {
            var mappingPath = Path.Combine(config.OutputPath, "Application", "Mappings");
            var mappingProfile = MappingProfileTemplate.Generate(entity);
            await _fileWriter.WriteFileAsync(Path.Combine(mappingPath, $"{entity.Name}MappingProfile.cs"), mappingProfile);
            Console.WriteLine($"  ✓ Generated AutoMapper Profile: {entity.Name}MappingProfile.cs");
        }
        
        // Generate Commands
        await GenerateCommandsAsync(entity, config, appPath);
        
        // Generate Queries
        await GenerateQueriesAsync(entity, config, appPath);
    }
    
    private async Task GenerateCommandsAsync(EntityModel entity, GenerationConfig config, string basePath)
    {
        // Create Command
        var createCmdPath = Path.Combine(basePath, "Commands", $"Create{entity.Name}");
        var createCmd = CommandTemplate.GenerateCreateCommand(entity);
        await _fileWriter.WriteFileAsync(Path.Combine(createCmdPath, $"Create{entity.Name}Command.cs"), createCmd);
        
        var createHandler = HandlerTemplate.GenerateCreateHandler(entity);
        await _fileWriter.WriteFileAsync(Path.Combine(createCmdPath, $"Create{entity.Name}CommandHandler.cs"), createHandler);
        
        if (config.UseFluentValidation)
        {
            var createValidator = ValidatorTemplate.GenerateCreateValidator(entity);
            await _fileWriter.WriteFileAsync(Path.Combine(createCmdPath, $"Create{entity.Name}CommandValidator.cs"), createValidator);
        }
        
        Console.WriteLine($"  ✓ Generated Create Command");
        
        // Update Command
        var updateCmdPath = Path.Combine(basePath, "Commands", $"Update{entity.Name}");
        var updateCmd = CommandTemplate.GenerateUpdateCommand(entity);
        await _fileWriter.WriteFileAsync(Path.Combine(updateCmdPath, $"Update{entity.Name}Command.cs"), updateCmd);
        
        var updateHandler = HandlerTemplate.GenerateUpdateHandler(entity);
        await _fileWriter.WriteFileAsync(Path.Combine(updateCmdPath, $"Update{entity.Name}CommandHandler.cs"), updateHandler);
        
        if (config.UseFluentValidation)
        {
            var updateValidator = ValidatorTemplate.GenerateUpdateValidator(entity);
            await _fileWriter.WriteFileAsync(Path.Combine(updateCmdPath, $"Update{entity.Name}CommandValidator.cs"), updateValidator);
        }
        
        Console.WriteLine($"  ✓ Generated Update Command");
        
        // Delete Command
        var deleteCmdPath = Path.Combine(basePath, "Commands", $"Delete{entity.Name}");
        var deleteCmd = CommandTemplate.GenerateDeleteCommand(entity);
        await _fileWriter.WriteFileAsync(Path.Combine(deleteCmdPath, $"Delete{entity.Name}Command.cs"), deleteCmd);
        
        var deleteHandler = HandlerTemplate.GenerateDeleteHandler(entity);
        await _fileWriter.WriteFileAsync(Path.Combine(deleteCmdPath, $"Delete{entity.Name}CommandHandler.cs"), deleteHandler);
        
        Console.WriteLine($"  ✓ Generated Delete Command");
    }
    
    private async Task GenerateQueriesAsync(EntityModel entity, GenerationConfig config, string basePath)
    {
        // GetById Query
        var getByIdPath = Path.Combine(basePath, "Queries", $"Get{entity.Name}ById");
        var getByIdQuery = QueryTemplate.GenerateGetByIdQuery(entity);
        await _fileWriter.WriteFileAsync(Path.Combine(getByIdPath, $"Get{entity.Name}ByIdQuery.cs"), getByIdQuery);
        
        var getByIdHandler = HandlerTemplate.GenerateGetByIdHandler(entity);
        await _fileWriter.WriteFileAsync(Path.Combine(getByIdPath, $"Get{entity.Name}ByIdQueryHandler.cs"), getByIdHandler);
        
        Console.WriteLine($"  ✓ Generated GetById Query");
        
        // GetAll Query
        var getAllPath = Path.Combine(basePath, "Queries", $"GetAll{entity.Name}s");
        var getAllQuery = QueryTemplate.GenerateGetAllQuery(entity);
        await _fileWriter.WriteFileAsync(Path.Combine(getAllPath, $"GetAll{entity.Name}sQuery.cs"), getAllQuery);
        
        var getAllHandler = HandlerTemplate.GenerateGetAllHandler(entity);
        await _fileWriter.WriteFileAsync(Path.Combine(getAllPath, $"GetAll{entity.Name}sQueryHandler.cs"), getAllHandler);
        
        Console.WriteLine($"  ✓ Generated GetAll Query");
    }
    
    private async Task GenerateInfrastructureAsync(EntityModel entity, GenerationConfig config)
    {
        var infraPath = Path.Combine(config.OutputPath, "Infrastructure", "Persistence", "Configurations");
        await _fileWriter.CreateDirectoryAsync(infraPath);
        
        var configCode = InfrastructureTemplate.GenerateEntityConfiguration(entity);
        var configFile = Path.Combine(infraPath, $"{entity.Name}Configuration.cs");
        await _fileWriter.WriteFileAsync(configFile, configCode);
        
        Console.WriteLine($"  ✓ Generated Entity Configuration");
    }
    
    private async Task GenerateApiAsync(EntityModel entity, GenerationConfig config)
    {
        var apiPath = Path.Combine(config.OutputPath, "Api", "Controllers");
        await _fileWriter.CreateDirectoryAsync(apiPath);
        
        var controllerCode = ControllerTemplate.Generate(entity);
        var controllerFile = Path.Combine(apiPath, $"{entity.Name}sController.cs");
        await _fileWriter.WriteFileAsync(controllerFile, controllerCode);
        
        Console.WriteLine($"  ✓ Generated API Controller");
    }
    
    /// <summary>
    /// Generates complete application infrastructure files (Program.cs, appsettings, project files, docs)
    /// </summary>
    public async Task GenerateApplicationInfrastructureAsync(List<EntityModel> entities, GenerationConfig config)
    {
        Console.WriteLine("Generating application infrastructure...");
        
        // Generate Program.cs
        if (config.GenerateProgramFile)
        {
            var programCode = ProgramTemplate.Generate(config, entities);
            var programFile = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Api", "Program.cs");
            await _fileWriter.WriteFileAsync(programFile, programCode);
            Console.WriteLine("  ✓ Generated Program.cs");
        }
        
        // Generate appsettings files
        if (config.GenerateAppSettings)
        {
            var apiPath = Path.Combine(config.OutputPath, $"{config.RootNamespace}.Api");
            
            var appSettings = AppSettingsTemplate.GenerateAppSettings(config);
            await _fileWriter.WriteFileAsync(Path.Combine(apiPath, "appsettings.json"), appSettings);
            Console.WriteLine("  ✓ Generated appsettings.json");
            
            var appSettingsDev = AppSettingsTemplate.GenerateAppSettingsDevelopment(config);
            await _fileWriter.WriteFileAsync(Path.Combine(apiPath, "appsettings.Development.json"), appSettingsDev);
            Console.WriteLine("  ✓ Generated appsettings.Development.json");
            
            var appSettingsProd = AppSettingsTemplate.GenerateAppSettingsProduction(config);
            await _fileWriter.WriteFileAsync(Path.Combine(apiPath, "appsettings.Production.json"), appSettingsProd);
            Console.WriteLine("  ✓ Generated appsettings.Production.json");
        }
        
        // Generate project files
        if (config.GenerateProjectFiles)
        {
            // API Project
            var apiProject = ProjectFileTemplate.GenerateApiProject(config);
            await _fileWriter.WriteFileAsync(
                Path.Combine(config.OutputPath, $"{config.RootNamespace}.Api", $"{config.RootNamespace}.Api.csproj"),
                apiProject);
            Console.WriteLine($"  ✓ Generated {config.RootNamespace}.Api.csproj");
            
            // Application Project
            var appProject = ProjectFileTemplate.GenerateApplicationProject(config);
            await _fileWriter.WriteFileAsync(
                Path.Combine(config.OutputPath, $"{config.RootNamespace}.Application", $"{config.RootNamespace}.Application.csproj"),
                appProject);
            Console.WriteLine($"  ✓ Generated {config.RootNamespace}.Application.csproj");
            
            // Infrastructure Project
            var infraProject = ProjectFileTemplate.GenerateInfrastructureProject(config);
            await _fileWriter.WriteFileAsync(
                Path.Combine(config.OutputPath, $"{config.RootNamespace}.Infrastructure", $"{config.RootNamespace}.Infrastructure.csproj"),
                infraProject);
            Console.WriteLine($"  ✓ Generated {config.RootNamespace}.Infrastructure.csproj");
            
            // Domain Project
            var domainProject = ProjectFileTemplate.GenerateDomainProject(config);
            await _fileWriter.WriteFileAsync(
                Path.Combine(config.OutputPath, $"{config.RootNamespace}.Domain", $"{config.RootNamespace}.Domain.csproj"),
                domainProject);
            Console.WriteLine($"  ✓ Generated {config.RootNamespace}.Domain.csproj");
        }
        
        // Generate documentation
        if (config.GenerateReadme)
        {
            var readmeContent = ReadmeTemplate.Generate(entities, config);
            await _fileWriter.WriteFileAsync(Path.Combine(config.OutputPath, "README.md"), readmeContent);
            Console.WriteLine("  ✓ Generated README.md");
        }
        
        if (config.GenerateArchitectureDocs)
        {
            var archDoc = DocumentationTemplate.GenerateArchitectureDoc(config);
            await _fileWriter.WriteFileAsync(Path.Combine(config.OutputPath, "ARCHITECTURE.md"), archDoc);
            Console.WriteLine("  ✓ Generated ARCHITECTURE.md");
        }
        
        // Generate .gitignore
        if (config.GenerateGitIgnore)
        {
            var gitignore = GenerateGitIgnore();
            await _fileWriter.WriteFileAsync(Path.Combine(config.OutputPath, ".gitignore"), gitignore);
            Console.WriteLine("  ✓ Generated .gitignore");
        }
        
        Console.WriteLine("✓ Application infrastructure generation completed");
    }
    
    private string GenerateGitIgnore()
    {
        return @"## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio cache/options directory
.vs/

# Visual Studio Code
.vscode/

# .NET Core
project.lock.json
project.fragment.lock.json
artifacts/

# Files built by Visual Studio
*_i.c
*_p.c
*_h.h
*.ilk
*.meta
*.obj
*.iobj
*.pch
*.pdb
*.ipdb
*.pgc
*.pgd
*.rsp
*.sbr
*.tlb
*.tli
*.tlh
*.tmp
*.tmp_proj
*_wpftmp.csproj
*.log
*.tlog
*.vspscc
*.vssscc
.builds
*.pidb
*.svclog
*.scc

# NuGet Packages
*.nupkg
*.snupkg
**/[Pp]ackages/*
!**/[Pp]ackages/build/
*.nuget.props
*.nuget.targets

# Database files
*.mdf
*.ldf
*.ndf

# Logs
logs/
*.log

# OS generated files
.DS_Store
.DS_Store?
._*
.Spotlight-V100
.Trashes
ehthumbs.db
Thumbs.db
";
    }
}
