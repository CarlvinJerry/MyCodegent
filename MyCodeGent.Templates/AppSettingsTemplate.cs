using MyCodeGent.Templates.Models;
using System.Text;
using System.Text.Json;

namespace MyCodeGent.Templates;

public static class AppSettingsTemplate
{
    public static string GenerateAppSettings(GenerationConfig config)
    {
        var settings = new Dictionary<string, object>
        {
            ["Logging"] = new Dictionary<string, object>
            {
                ["LogLevel"] = new Dictionary<string, string>
                {
                    ["Default"] = "Information",
                    ["Microsoft.AspNetCore"] = "Warning",
                    ["Microsoft.EntityFrameworkCore"] = "Warning"
                }
            },
            ["AllowedHosts"] = "*"
        };
        
        // Connection Strings
        if (config.GenerateInfrastructure)
        {
            settings["ConnectionStrings"] = new Dictionary<string, string>
            {
                ["DefaultConnection"] = config.ConnectionString
            };
            
            if (config.CachingProvider == CachingProvider.Redis)
            {
                ((Dictionary<string, string>)settings["ConnectionStrings"])["Redis"] = "localhost:6379";
            }
        }
        
        // JWT Configuration
        if (config.GenerateAuthentication && config.AuthenticationType == AuthenticationType.JWT)
        {
            settings["Jwt"] = new Dictionary<string, string>
            {
                ["Key"] = "YourSuperSecretKeyHere_ChangeThisInProduction_AtLeast32Characters",
                ["Issuer"] = $"{config.RootNamespace}API",
                ["Audience"] = $"{config.RootNamespace}Client",
                ["ExpiryInMinutes"] = "60"
            };
        }
        
        // Serilog Configuration
        if (config.LoggingProvider == LoggingProvider.Serilog)
        {
            settings["Serilog"] = new Dictionary<string, object>
            {
                ["Using"] = new[] { "Serilog.Sinks.Console", "Serilog.Sinks.File" },
                ["MinimumLevel"] = new Dictionary<string, object>
                {
                    ["Default"] = "Information",
                    ["Override"] = new Dictionary<string, string>
                    {
                        ["Microsoft"] = "Warning",
                        ["System"] = "Warning"
                    }
                },
                ["WriteTo"] = new object[]
                {
                    new Dictionary<string, string> { ["Name"] = "Console" },
                    new Dictionary<string, object>
                    {
                        ["Name"] = "File",
                        ["Args"] = new Dictionary<string, object>
                        {
                            ["path"] = "logs/log-.txt",
                            ["rollingInterval"] = "Day",
                            ["outputTemplate"] = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                        }
                    }
                },
                ["Enrich"] = new[] { "FromLogContext", "WithMachineName", "WithThreadId" }
            };
        }
        
        // Application Insights
        if (config.GenerateApplicationInsights)
        {
            settings["ApplicationInsights"] = new Dictionary<string, string>
            {
                ["InstrumentationKey"] = "your-instrumentation-key-here",
                ["ConnectionString"] = "InstrumentationKey=your-instrumentation-key-here"
            };
        }
        
        // CORS Origins
        if (config.GenerateCors)
        {
            settings["CorsOrigins"] = config.AllowedOrigins;
        }
        
        // API Configuration
        settings[$"{config.RootNamespace}"] = new Dictionary<string, object>
        {
            ["ApiVersion"] = "1.0",
            ["ApiTitle"] = $"{config.RootNamespace} API",
            ["EnableSwagger"] = config.GenerateSwagger,
            ["EnableHealthChecks"] = config.GenerateHealthChecks
        };
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        return JsonSerializer.Serialize(settings, options);
    }
    
    public static string GenerateAppSettingsDevelopment(GenerationConfig config)
    {
        var settings = new Dictionary<string, object>
        {
            ["Logging"] = new Dictionary<string, object>
            {
                ["LogLevel"] = new Dictionary<string, string>
                {
                    ["Default"] = "Debug",
                    ["Microsoft.AspNetCore"] = "Information",
                    ["Microsoft.EntityFrameworkCore"] = "Information"
                }
            }
        };
        
        // Development Connection String
        if (config.GenerateInfrastructure)
        {
            var devConnectionString = config.DatabaseProvider switch
            {
                DatabaseProvider.SqlServer => "Server=localhost;Database=MyAppDb_Dev;Trusted_Connection=true;TrustServerCertificate=true;",
                DatabaseProvider.PostgreSql => "Host=localhost;Database=myappdb_dev;Username=postgres;Password=postgres",
                DatabaseProvider.MySql => "Server=localhost;Database=myappdb_dev;User=root;Password=root;",
                DatabaseProvider.Sqlite => "Data Source=myappdb_dev.db",
                _ => config.ConnectionString
            };
            
            settings["ConnectionStrings"] = new Dictionary<string, string>
            {
                ["DefaultConnection"] = devConnectionString
            };
        }
        
        // Detailed Serilog for Development
        if (config.LoggingProvider == LoggingProvider.Serilog)
        {
            settings["Serilog"] = new Dictionary<string, object>
            {
                ["MinimumLevel"] = new Dictionary<string, object>
                {
                    ["Default"] = "Debug",
                    ["Override"] = new Dictionary<string, string>
                    {
                        ["Microsoft"] = "Information",
                        ["System"] = "Information"
                    }
                }
            };
        }
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        return JsonSerializer.Serialize(settings, options);
    }
    
    public static string GenerateAppSettingsProduction(GenerationConfig config)
    {
        var settings = new Dictionary<string, object>
        {
            ["Logging"] = new Dictionary<string, object>
            {
                ["LogLevel"] = new Dictionary<string, string>
                {
                    ["Default"] = "Warning",
                    ["Microsoft.AspNetCore"] = "Warning",
                    ["Microsoft.EntityFrameworkCore"] = "Error"
                }
            }
        };
        
        // Production settings are typically environment variables
        settings["UseEnvironmentVariables"] = true;
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        return JsonSerializer.Serialize(settings, options);
    }
}
