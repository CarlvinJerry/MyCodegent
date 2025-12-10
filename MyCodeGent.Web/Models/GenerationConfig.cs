using System.Text.Json.Serialization;

namespace MyCodeGent.Web.Models;

public class GenerationConfig
{
    public string OutputPath { get; set; } = "./Generated";
    public string RootNamespace { get; set; } = "MyApp";
    public bool GenerateApi { get; set; } = true;
    public bool GenerateApplication { get; set; } = true;
    public bool GenerateDomain { get; set; } = true;
    public bool GenerateInfrastructure { get; set; } = true;
    public bool UseMediator { get; set; } = true;
    public bool UseFluentValidation { get; set; } = true;
    public bool UseAutoMapper { get; set; } = true;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DatabaseProvider DatabaseProvider { get; set; } = DatabaseProvider.SqlServer;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DatabaseProvider
{
    SqlServer,
    PostgreSql,
    MySql,
    Sqlite
}
