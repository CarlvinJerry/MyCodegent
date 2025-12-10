using MyCodeGent.Core.Interfaces;
using MyCodeGent.Templates.Models;
using MyCodeGent.Templates;

namespace MyCodeGent.Core.Services;

public class IncrementalCodeGenerator
{
    private readonly IFileWriter _fileWriter;
    
    public IncrementalCodeGenerator(IFileWriter fileWriter)
    {
        _fileWriter = fileWriter;
    }
    
    /// <summary>
    /// Generates code for new entities and updates common files (DbContext, etc.)
    /// without overwriting existing entity files
    /// </summary>
    public async Task<IncrementalGenerationResult> GenerateIncrementalAsync(
        List<EntityModel> newEntities, 
        GenerationConfig config,
        string existingProjectPath)
    {
        var result = new IncrementalGenerationResult();
        
        Console.WriteLine($"Starting incremental generation for {newEntities.Count} new entities");
        Console.WriteLine($"Target project path: {existingProjectPath}");
        
        // Validate that the target path exists
        if (!Directory.Exists(existingProjectPath))
        {
            throw new DirectoryNotFoundException($"Project path not found: {existingProjectPath}");
        }
        
        // Generate code for each new entity
        foreach (var entity in newEntities)
        {
            var entityFiles = await GenerateEntityFilesAsync(entity, config, existingProjectPath);
            result.NewFiles.AddRange(entityFiles);
            result.EntitiesAdded.Add(entity.Name);
        }
        
        // Update common files (DbContext, IApplicationDbContext)
        var updatedFiles = await UpdateCommonFilesAsync(newEntities, config, existingProjectPath);
        result.UpdatedFiles.AddRange(updatedFiles);
        
        Console.WriteLine($"✓ Incremental generation completed");
        Console.WriteLine($"  - New files: {result.NewFiles.Count}");
        Console.WriteLine($"  - Updated files: {result.UpdatedFiles.Count}");
        Console.WriteLine($"  - Entities added: {string.Join(", ", result.EntitiesAdded)}");
        
        return result;
    }
    
    private async Task<List<string>> GenerateEntityFilesAsync(
        EntityModel entity, 
        GenerationConfig config, 
        string projectPath)
    {
        var generatedFiles = new List<string>();
        
        // Generate Domain Layer
        if (config.GenerateDomain)
        {
            var domainPath = Path.Combine(projectPath, "Domain", "Entities");
            var entityFile = Path.Combine(domainPath, $"{entity.Name}.cs");
            
            if (await _fileWriter.FileExistsAsync(entityFile))
            {
                Console.WriteLine($"  ⚠ Skipping existing entity: {entity.Name}.cs");
            }
            else
            {
                var entityCode = EntityTemplate.Generate(entity);
                await _fileWriter.WriteFileAsync(entityFile, entityCode);
                generatedFiles.Add(entityFile);
                Console.WriteLine($"  ✓ Generated Domain Entity: {entity.Name}.cs");
            }
        }
        
        // Generate Application Layer
        if (config.GenerateApplication)
        {
            var appFiles = await GenerateApplicationLayerAsync(entity, config, projectPath);
            generatedFiles.AddRange(appFiles);
        }
        
        // Generate Infrastructure Layer
        if (config.GenerateInfrastructure)
        {
            var infraPath = Path.Combine(projectPath, "Infrastructure", "Persistence", "Configurations");
            var configFile = Path.Combine(infraPath, $"{entity.Name}Configuration.cs");
            
            if (!await _fileWriter.FileExistsAsync(configFile))
            {
                var configCode = InfrastructureTemplate.GenerateEntityConfiguration(entity);
                await _fileWriter.WriteFileAsync(configFile, configCode);
                generatedFiles.Add(configFile);
                Console.WriteLine($"  ✓ Generated Entity Configuration");
            }
        }
        
        // Generate API Layer
        if (config.GenerateApi)
        {
            var apiPath = Path.Combine(projectPath, "Api", "Controllers");
            var controllerFile = Path.Combine(apiPath, $"{entity.Name}sController.cs");
            
            if (!await _fileWriter.FileExistsAsync(controllerFile))
            {
                var controllerCode = ControllerTemplate.Generate(entity);
                await _fileWriter.WriteFileAsync(controllerFile, controllerCode);
                generatedFiles.Add(controllerFile);
                Console.WriteLine($"  ✓ Generated API Controller");
            }
        }
        
        return generatedFiles;
    }
    
    private async Task<List<string>> GenerateApplicationLayerAsync(
        EntityModel entity, 
        GenerationConfig config, 
        string projectPath)
    {
        var generatedFiles = new List<string>();
        var appPath = Path.Combine(projectPath, "Application", $"{entity.Name}s");
        
        // Generate DTO
        var dtoFile = Path.Combine(appPath, $"{entity.Name}Dto.cs");
        if (!await _fileWriter.FileExistsAsync(dtoFile))
        {
            var dtoCode = DtoTemplate.Generate(entity);
            await _fileWriter.WriteFileAsync(dtoFile, dtoCode);
            generatedFiles.Add(dtoFile);
            Console.WriteLine($"  ✓ Generated DTO: {entity.Name}Dto.cs");
        }
        
        // Generate Commands
        generatedFiles.AddRange(await GenerateCommandsAsync(entity, config, appPath));
        
        // Generate Queries
        generatedFiles.AddRange(await GenerateQueriesAsync(entity, config, appPath));
        
        return generatedFiles;
    }
    
    private async Task<List<string>> GenerateCommandsAsync(
        EntityModel entity, 
        GenerationConfig config, 
        string basePath)
    {
        var generatedFiles = new List<string>();
        
        // Create Command
        var createCmdPath = Path.Combine(basePath, "Commands", $"Create{entity.Name}");
        var createCmdFile = Path.Combine(createCmdPath, $"Create{entity.Name}Command.cs");
        
        if (!await _fileWriter.FileExistsAsync(createCmdFile))
        {
            var createCmd = CommandTemplate.GenerateCreateCommand(entity);
            await _fileWriter.WriteFileAsync(createCmdFile, createCmd);
            generatedFiles.Add(createCmdFile);
            
            var createHandler = HandlerTemplate.GenerateCreateHandler(entity);
            var handlerFile = Path.Combine(createCmdPath, $"Create{entity.Name}CommandHandler.cs");
            await _fileWriter.WriteFileAsync(handlerFile, createHandler);
            generatedFiles.Add(handlerFile);
            
            if (config.UseFluentValidation)
            {
                var createValidator = ValidatorTemplate.GenerateCreateValidator(entity);
                var validatorFile = Path.Combine(createCmdPath, $"Create{entity.Name}CommandValidator.cs");
                await _fileWriter.WriteFileAsync(validatorFile, createValidator);
                generatedFiles.Add(validatorFile);
            }
            
            Console.WriteLine($"  ✓ Generated Create Command");
        }
        
        // Update Command
        var updateCmdPath = Path.Combine(basePath, "Commands", $"Update{entity.Name}");
        var updateCmdFile = Path.Combine(updateCmdPath, $"Update{entity.Name}Command.cs");
        
        if (!await _fileWriter.FileExistsAsync(updateCmdFile))
        {
            var updateCmd = CommandTemplate.GenerateUpdateCommand(entity);
            await _fileWriter.WriteFileAsync(updateCmdFile, updateCmd);
            generatedFiles.Add(updateCmdFile);
            
            var updateHandler = HandlerTemplate.GenerateUpdateHandler(entity);
            var handlerFile = Path.Combine(updateCmdPath, $"Update{entity.Name}CommandHandler.cs");
            await _fileWriter.WriteFileAsync(handlerFile, updateHandler);
            generatedFiles.Add(handlerFile);
            
            if (config.UseFluentValidation)
            {
                var updateValidator = ValidatorTemplate.GenerateUpdateValidator(entity);
                var validatorFile = Path.Combine(updateCmdPath, $"Update{entity.Name}CommandValidator.cs");
                await _fileWriter.WriteFileAsync(validatorFile, updateValidator);
                generatedFiles.Add(validatorFile);
            }
            
            Console.WriteLine($"  ✓ Generated Update Command");
        }
        
        // Delete Command
        var deleteCmdPath = Path.Combine(basePath, "Commands", $"Delete{entity.Name}");
        var deleteCmdFile = Path.Combine(deleteCmdPath, $"Delete{entity.Name}Command.cs");
        
        if (!await _fileWriter.FileExistsAsync(deleteCmdFile))
        {
            var deleteCmd = CommandTemplate.GenerateDeleteCommand(entity);
            await _fileWriter.WriteFileAsync(deleteCmdFile, deleteCmd);
            generatedFiles.Add(deleteCmdFile);
            
            var deleteHandler = HandlerTemplate.GenerateDeleteHandler(entity);
            var handlerFile = Path.Combine(deleteCmdPath, $"Delete{entity.Name}CommandHandler.cs");
            await _fileWriter.WriteFileAsync(handlerFile, deleteHandler);
            generatedFiles.Add(handlerFile);
            
            Console.WriteLine($"  ✓ Generated Delete Command");
        }
        
        return generatedFiles;
    }
    
    private async Task<List<string>> GenerateQueriesAsync(
        EntityModel entity, 
        GenerationConfig config, 
        string basePath)
    {
        var generatedFiles = new List<string>();
        
        // GetById Query
        var getByIdPath = Path.Combine(basePath, "Queries", $"Get{entity.Name}ById");
        var getByIdFile = Path.Combine(getByIdPath, $"Get{entity.Name}ByIdQuery.cs");
        
        if (!await _fileWriter.FileExistsAsync(getByIdFile))
        {
            var getByIdQuery = QueryTemplate.GenerateGetByIdQuery(entity);
            await _fileWriter.WriteFileAsync(getByIdFile, getByIdQuery);
            generatedFiles.Add(getByIdFile);
            
            var getByIdHandler = HandlerTemplate.GenerateGetByIdHandler(entity);
            var handlerFile = Path.Combine(getByIdPath, $"Get{entity.Name}ByIdQueryHandler.cs");
            await _fileWriter.WriteFileAsync(handlerFile, getByIdHandler);
            generatedFiles.Add(handlerFile);
            
            Console.WriteLine($"  ✓ Generated GetById Query");
        }
        
        // GetAll Query
        var getAllPath = Path.Combine(basePath, "Queries", $"GetAll{entity.Name}s");
        var getAllFile = Path.Combine(getAllPath, $"GetAll{entity.Name}sQuery.cs");
        
        if (!await _fileWriter.FileExistsAsync(getAllFile))
        {
            var getAllQuery = QueryTemplate.GenerateGetAllQuery(entity);
            await _fileWriter.WriteFileAsync(getAllFile, getAllQuery);
            generatedFiles.Add(getAllFile);
            
            var getAllHandler = HandlerTemplate.GenerateGetAllHandler(entity);
            var handlerFile = Path.Combine(getAllPath, $"GetAll{entity.Name}sQueryHandler.cs");
            await _fileWriter.WriteFileAsync(handlerFile, getAllHandler);
            generatedFiles.Add(handlerFile);
            
            Console.WriteLine($"  ✓ Generated GetAll Query");
        }
        
        return generatedFiles;
    }
    
    private async Task<List<string>> UpdateCommonFilesAsync(
        List<EntityModel> newEntities, 
        GenerationConfig config, 
        string projectPath)
    {
        var updatedFiles = new List<string>();
        
        // Get all entities (existing + new)
        var allEntities = await GetAllEntitiesAsync(projectPath, newEntities);
        
        // Update IApplicationDbContext
        var interfacePath = Path.Combine(projectPath, "Application", "Common", "Interfaces", "IApplicationDbContext.cs");
        var interfaceCode = InfrastructureTemplate.GenerateApplicationDbContextInterface(allEntities, config.RootNamespace);
        await _fileWriter.WriteFileAsync(interfacePath, interfaceCode);
        updatedFiles.Add(interfacePath);
        Console.WriteLine($"  ✓ Updated IApplicationDbContext.cs");
        
        // Update ApplicationDbContext
        var dbContextPath = Path.Combine(projectPath, "Infrastructure", "Persistence", "ApplicationDbContext.cs");
        var dbContextCode = InfrastructureTemplate.GenerateDbContext(allEntities, config.RootNamespace);
        await _fileWriter.WriteFileAsync(dbContextPath, dbContextCode);
        updatedFiles.Add(dbContextPath);
        Console.WriteLine($"  ✓ Updated ApplicationDbContext.cs");
        
        return updatedFiles;
    }
    
    private async Task<List<EntityModel>> GetAllEntitiesAsync(string projectPath, List<EntityModel> newEntities)
    {
        var allEntities = new List<EntityModel>(newEntities);
        
        // Scan for existing entities
        var domainPath = Path.Combine(projectPath, "Domain", "Entities");
        if (Directory.Exists(domainPath))
        {
            var existingEntityFiles = Directory.GetFiles(domainPath, "*.cs");
            foreach (var file in existingEntityFiles)
            {
                var entityName = Path.GetFileNameWithoutExtension(file);
                
                // Skip if this entity is in the new entities list
                if (newEntities.Any(e => e.Name == entityName))
                    continue;
                
                // Create a basic entity model for existing entities
                // (we just need the name for DbContext generation)
                allEntities.Add(new EntityModel { Name = entityName });
            }
        }
        
        return allEntities;
    }
}

public class IncrementalGenerationResult
{
    public List<string> NewFiles { get; set; } = new();
    public List<string> UpdatedFiles { get; set; } = new();
    public List<string> EntitiesAdded { get; set; } = new();
    public List<string> SkippedFiles { get; set; } = new();
}
