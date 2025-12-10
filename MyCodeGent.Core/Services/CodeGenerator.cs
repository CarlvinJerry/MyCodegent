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
}
