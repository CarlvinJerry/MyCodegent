using System.Text.Json.Serialization;

namespace MyCodeGent.Templates.Models;

public class GenerationConfig
{
    // Basic Configuration
    public string OutputPath { get; set; } = "./Generated";
    public string RootNamespace { get; set; } = "MyApp";
    
    // Layer Generation
    public bool GenerateApi { get; set; } = true;
    public bool GenerateApplication { get; set; } = true;
    public bool GenerateDomain { get; set; } = true;
    public bool GenerateInfrastructure { get; set; } = true;
    
    // Patterns & Libraries
    public bool UseMediator { get; set; } = true;
    public bool UseFluentValidation { get; set; } = true;
    public bool UseAutoMapper { get; set; } = true;
    
    // Database Configuration
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DatabaseProvider DatabaseProvider { get; set; } = DatabaseProvider.SqlServer;
    public string ConnectionString { get; set; } = "Server=localhost;Database=MyAppDb;Trusted_Connection=true;TrustServerCertificate=true;";
    public bool GenerateMigrations { get; set; } = true;
    public bool GenerateSeedData { get; set; } = false;
    
    // Application Infrastructure
    public bool GenerateProgramFile { get; set; } = true;
    public bool GenerateAppSettings { get; set; } = true;
    public bool GenerateProjectFiles { get; set; } = true;
    public bool GenerateSolutionFile { get; set; } = true;
    public bool GenerateLaunchSettings { get; set; } = true;
    
    // Authentication & Authorization
    public bool GenerateAuthentication { get; set; } = false;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AuthenticationType AuthenticationType { get; set; } = AuthenticationType.JWT;
    public bool GenerateIdentity { get; set; } = false;
    public bool GenerateRoleBasedAuth { get; set; } = false;
    
    // API Documentation
    public bool GenerateSwagger { get; set; } = true;
    public bool GenerateXmlDocumentation { get; set; } = true;
    public bool GenerateApiVersioning { get; set; } = false;
    
    // Logging & Monitoring
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LoggingProvider LoggingProvider { get; set; } = LoggingProvider.Serilog;
    public bool GenerateHealthChecks { get; set; } = true;
    public bool GenerateApplicationInsights { get; set; } = false;
    
    // Error Handling
    public bool GenerateGlobalExceptionHandler { get; set; } = true;
    public bool GenerateProblemDetails { get; set; } = true;
    
    // Performance & Caching
    public bool GenerateCaching { get; set; } = false;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CachingProvider CachingProvider { get; set; } = CachingProvider.Memory;
    public bool GenerateResponseCompression { get; set; } = true;
    
    // CORS & Security
    public bool GenerateCors { get; set; } = true;
    public string[] AllowedOrigins { get; set; } = new[] { "http://localhost:3000", "http://localhost:4200" };
    public bool GenerateRateLimiting { get; set; } = false;
    
    // Testing
    public bool GenerateUnitTests { get; set; } = false;
    public bool GenerateIntegrationTests { get; set; } = false;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TestFramework TestFramework { get; set; } = TestFramework.XUnit;
    
    // DevOps & Deployment
    public bool GenerateDockerfile { get; set; } = false;
    public bool GenerateDockerCompose { get; set; } = false;
    public bool GenerateKubernetesManifests { get; set; } = false;
    public bool GenerateGitHubActions { get; set; } = false;
    public bool GenerateAzureDevOps { get; set; } = false;
    
    // Documentation
    public bool GenerateReadme { get; set; } = true;
    public bool GenerateArchitectureDocs { get; set; } = true;
    public bool GenerateApiDocs { get; set; } = true;
    public bool GenerateChangeLog { get; set; } = false;
    
    // Code Quality
    public bool GenerateEditorConfig { get; set; } = true;
    public bool GenerateGitIgnore { get; set; } = true;
    public bool GenerateCodeAnalysisRules { get; set; } = false;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DatabaseProvider
{
    SqlServer,
    PostgreSql,
    MySql,
    Sqlite,
    InMemory
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuthenticationType
{
    None,
    JWT,
    IdentityServer,
    AzureAD,
    Auth0
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LoggingProvider
{
    Default,
    Serilog,
    NLog,
    Log4Net,
    ApplicationInsights
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CachingProvider
{
    None,
    Memory,
    Redis,
    Distributed
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TestFramework
{
    XUnit,
    NUnit,
    MSTest
}
