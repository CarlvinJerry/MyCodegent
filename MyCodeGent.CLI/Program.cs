using MyCodeGent.Core.Interfaces;
using MyCodeGent.Templates.Models;
using MyCodeGent.Core.Services;
using System.Text.Json;

Console.WriteLine("╔════════════════════════════════════════╗");
Console.WriteLine("║   MyCodeGent - CRUD Code Generator     ║");
Console.WriteLine("║   Clean Architecture + CQRS Pattern    ║");
Console.WriteLine("╚════════════════════════════════════════╝");
Console.WriteLine();

// Check for config file argument
string configPath = args.Length > 0 ? args[0] : "codegen-config.json";

if (!File.Exists(configPath))
{
    Console.WriteLine($"Configuration file not found: {configPath}");
    Console.WriteLine("Creating sample configuration file...");
    await CreateSampleConfigAsync(configPath);
    Console.WriteLine($"✓ Sample configuration created: {configPath}");
    Console.WriteLine("Please edit the configuration file and run again.");
    return;
}

// Load configuration
var configJson = await File.ReadAllTextAsync(configPath);
var configData = JsonSerializer.Deserialize<ConfigFile>(configJson);

if (configData == null || configData.Entities == null || configData.Entities.Count == 0)
{
    Console.WriteLine("❌ No entities found in configuration file.");
    return;
}

// Initialize services
IFileWriter fileWriter = new FileWriter();
ICodeGenerator codeGenerator = new CodeGenerator(fileWriter);

var config = configData.Config ?? new GenerationConfig();

Console.WriteLine($"Output Path: {config.OutputPath}");
Console.WriteLine($"Root Namespace: {config.RootNamespace}");
Console.WriteLine($"Entities to generate: {configData.Entities.Count}");
Console.WriteLine();

// Generate code for each entity
foreach (var entity in configData.Entities)
{
    entity.Namespace = config.RootNamespace;
    await codeGenerator.GenerateAsync(entity, config);
    Console.WriteLine();
}

// Generate common infrastructure files
await GenerateCommonFilesAsync(fileWriter, configData.Entities, config);

Console.WriteLine("════════════════════════════════════════");
Console.WriteLine("✓ Code generation completed successfully!");
Console.WriteLine($"✓ Generated files in: {config.OutputPath}");
Console.WriteLine("════════════════════════════════════════");

static async Task CreateSampleConfigAsync(string path)
{
    var sampleConfig = new ConfigFile
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
                    new PropertyModel
                    {
                        Name = "Id",
                        Type = "int",
                        IsKey = true,
                        IsRequired = true
                    },
                    new PropertyModel
                    {
                        Name = "Name",
                        Type = "string",
                        IsRequired = true,
                        MaxLength = 200
                    },
                    new PropertyModel
                    {
                        Name = "Description",
                        Type = "string",
                        IsNullable = true,
                        MaxLength = 1000
                    },
                    new PropertyModel
                    {
                        Name = "Price",
                        Type = "decimal",
                        IsRequired = true
                    },
                    new PropertyModel
                    {
                        Name = "Stock",
                        Type = "int",
                        IsRequired = true
                    }
                }
            },
            new EntityModel
            {
                Name = "Customer",
                HasAuditFields = true,
                HasSoftDelete = true,
                Properties = new List<PropertyModel>
                {
                    new PropertyModel
                    {
                        Name = "Id",
                        Type = "int",
                        IsKey = true,
                        IsRequired = true
                    },
                    new PropertyModel
                    {
                        Name = "FirstName",
                        Type = "string",
                        IsRequired = true,
                        MaxLength = 100
                    },
                    new PropertyModel
                    {
                        Name = "LastName",
                        Type = "string",
                        IsRequired = true,
                        MaxLength = 100
                    },
                    new PropertyModel
                    {
                        Name = "Email",
                        Type = "string",
                        IsRequired = true,
                        MaxLength = 255
                    },
                    new PropertyModel
                    {
                        Name = "Phone",
                        Type = "string",
                        IsNullable = true,
                        MaxLength = 20
                    }
                }
            }
        }
    };
    
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    var json = JsonSerializer.Serialize(sampleConfig, options);
    await File.WriteAllTextAsync(path, json);
}

static async Task GenerateCommonFilesAsync(IFileWriter fileWriter, List<EntityModel> entities, GenerationConfig config)
{
    Console.WriteLine("Generating common infrastructure files...");
    
    // Generate IApplicationDbContext
    var interfacePath = Path.Combine(config.OutputPath, "Application", "Common", "Interfaces");
    var interfaceCode = MyCodeGent.Templates.InfrastructureTemplate.GenerateApplicationDbContextInterface(entities, config.RootNamespace);
    await fileWriter.WriteFileAsync(Path.Combine(interfacePath, "IApplicationDbContext.cs"), interfaceCode);
    Console.WriteLine("  ✓ Generated IApplicationDbContext.cs");
    
    // Generate ApplicationDbContext
    var dbContextPath = Path.Combine(config.OutputPath, "Infrastructure", "Persistence");
    var dbContextCode = MyCodeGent.Templates.InfrastructureTemplate.GenerateDbContext(entities, config.RootNamespace);
    await fileWriter.WriteFileAsync(Path.Combine(dbContextPath, "ApplicationDbContext.cs"), dbContextCode);
    Console.WriteLine("  ✓ Generated ApplicationDbContext.cs");
    
    // Generate PagedResult
    var modelsPath = Path.Combine(config.OutputPath, "Application", "Common", "Models");
    var pagedResultCode = MyCodeGent.Templates.InfrastructureTemplate.GeneratePagedResult(config.RootNamespace);
    await fileWriter.WriteFileAsync(Path.Combine(modelsPath, "PagedResult.cs"), pagedResultCode);
    Console.WriteLine("  ✓ Generated PagedResult.cs");
}

class ConfigFile
{
    public GenerationConfig? Config { get; set; }
    public List<EntityModel>? Entities { get; set; }
}
