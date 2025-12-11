using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class InfrastructureConfigTemplate
{
    public static string GenerateHealthChecks(string rootNamespace, DatabaseProvider dbProvider)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using Microsoft.Extensions.Diagnostics.HealthChecks;");
        sb.AppendLine($"using {rootNamespace}.Application.Common.Interfaces;");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNamespace}.Infrastructure.HealthChecks;");
        sb.AppendLine();
        sb.AppendLine("public class DatabaseHealthCheck : IHealthCheck");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly IApplicationDbContext _context;");
        sb.AppendLine();
        sb.AppendLine("    public DatabaseHealthCheck(IApplicationDbContext context)");
        sb.AppendLine("    {");
        sb.AppendLine("        _context = context;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public async Task<HealthCheckResult> CheckHealthAsync(");
        sb.AppendLine("        HealthCheckContext context,");
        sb.AppendLine("        CancellationToken cancellationToken = default)");
        sb.AppendLine("    {");
        sb.AppendLine("        try");
        sb.AppendLine("        {");
        sb.AppendLine("            // Try to execute a simple query");
        sb.AppendLine("            await _context.SaveChangesAsync(cancellationToken);");
        sb.AppendLine("            return HealthCheckResult.Healthy(\"Database is accessible\");");
        sb.AppendLine("        }");
        sb.AppendLine("        catch (Exception ex)");
        sb.AppendLine("        {");
        sb.AppendLine("            return HealthCheckResult.Unhealthy(");
        sb.AppendLine("                \"Database is not accessible\",");
        sb.AppendLine("                exception: ex);");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateExceptionMiddleware(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using System.Net;");
        sb.AppendLine("using System.Text.Json;");
        sb.AppendLine("using FluentValidation;");
        sb.AppendLine("using Microsoft.AspNetCore.Http;");
        sb.AppendLine("using Microsoft.Extensions.Logging;");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNamespace}.Api.Middleware;");
        sb.AppendLine();
        sb.AppendLine("public class ExceptionHandlingMiddleware");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly RequestDelegate _next;");
        sb.AppendLine("    private readonly ILogger<ExceptionHandlingMiddleware> _logger;");
        sb.AppendLine();
        sb.AppendLine("    public ExceptionHandlingMiddleware(");
        sb.AppendLine("        RequestDelegate next,");
        sb.AppendLine("        ILogger<ExceptionHandlingMiddleware> logger)");
        sb.AppendLine("    {");
        sb.AppendLine("        _next = next;");
        sb.AppendLine("        _logger = logger;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public async Task InvokeAsync(HttpContext context)");
        sb.AppendLine("    {");
        sb.AppendLine("        try");
        sb.AppendLine("        {");
        sb.AppendLine("            await _next(context);");
        sb.AppendLine("        }");
        sb.AppendLine("        catch (Exception ex)");
        sb.AppendLine("        {");
        sb.AppendLine("            await HandleExceptionAsync(context, ex);");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    private async Task HandleExceptionAsync(HttpContext context, Exception exception)");
        sb.AppendLine("    {");
        sb.AppendLine("        _logger.LogError(exception, \"An unhandled exception occurred\");");
        sb.AppendLine();
        sb.AppendLine("        var response = context.Response;");
        sb.AppendLine("        response.ContentType = \"application/json\";");
        sb.AppendLine();
        sb.AppendLine("        var errorResponse = exception switch");
        sb.AppendLine("        {");
        sb.AppendLine("            ValidationException validationEx => new ErrorResponse");
        sb.AppendLine("            {");
        sb.AppendLine("                StatusCode = (int)HttpStatusCode.BadRequest,");
        sb.AppendLine("                Message = \"Validation failed\",");
        sb.AppendLine("                Errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList()");
        sb.AppendLine("            },");
        sb.AppendLine("            KeyNotFoundException => new ErrorResponse");
        sb.AppendLine("            {");
        sb.AppendLine("                StatusCode = (int)HttpStatusCode.NotFound,");
        sb.AppendLine("                Message = \"Resource not found\"");
        sb.AppendLine("            },");
        sb.AppendLine("            UnauthorizedAccessException => new ErrorResponse");
        sb.AppendLine("            {");
        sb.AppendLine("                StatusCode = (int)HttpStatusCode.Unauthorized,");
        sb.AppendLine("                Message = \"Unauthorized access\"");
        sb.AppendLine("            },");
        sb.AppendLine("            _ => new ErrorResponse");
        sb.AppendLine("            {");
        sb.AppendLine("                StatusCode = (int)HttpStatusCode.InternalServerError,");
        sb.AppendLine("                Message = \"An error occurred while processing your request\"");
        sb.AppendLine("            }");
        sb.AppendLine("        };");
        sb.AppendLine();
        sb.AppendLine("        response.StatusCode = errorResponse.StatusCode;");
        sb.AppendLine();
        sb.AppendLine("        var options = new JsonSerializerOptions");
        sb.AppendLine("        {");
        sb.AppendLine("            PropertyNamingPolicy = JsonNamingPolicy.CamelCase");
        sb.AppendLine("        };");
        sb.AppendLine();
        sb.AppendLine("        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    private class ErrorResponse");
        sb.AppendLine("    {");
        sb.AppendLine("        public int StatusCode { get; set; }");
        sb.AppendLine("        public string Message { get; set; } = string.Empty;");
        sb.AppendLine("        public List<string>? Errors { get; set; }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateCorsConfiguration(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"namespace {rootNamespace}.Api.Configuration;");
        sb.AppendLine();
        sb.AppendLine("public static class CorsConfiguration");
        sb.AppendLine("{");
        sb.AppendLine("    public const string DefaultPolicy = \"DefaultCorsPolicy\";");
        sb.AppendLine();
        sb.AppendLine("    public static IServiceCollection AddCorsConfiguration(");
        sb.AppendLine("        this IServiceCollection services,");
        sb.AppendLine("        IConfiguration configuration)");
        sb.AppendLine("    {");
        sb.AppendLine("        var allowedOrigins = configuration");
        sb.AppendLine("            .GetSection(\"Cors:AllowedOrigins\")");
        sb.AppendLine("            .Get<string[]>() ?? new[] { \"http://localhost:3000\", \"http://localhost:4200\" };");
        sb.AppendLine();
        sb.AppendLine("        services.AddCors(options =>");
        sb.AppendLine("        {");
        sb.AppendLine("            options.AddPolicy(DefaultPolicy, builder =>");
        sb.AppendLine("            {");
        sb.AppendLine("                builder");
        sb.AppendLine("                    .WithOrigins(allowedOrigins)");
        sb.AppendLine("                    .AllowAnyMethod()");
        sb.AppendLine("                    .AllowAnyHeader()");
        sb.AppendLine("                    .AllowCredentials();");
        sb.AppendLine("            });");
        sb.AppendLine("        });");
        sb.AppendLine();
        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateLoggingConfiguration(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using Serilog;");
        sb.AppendLine("using Serilog.Events;");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNamespace}.Api.Configuration;");
        sb.AppendLine();
        sb.AppendLine("public static class LoggingConfiguration");
        sb.AppendLine("{");
        sb.AppendLine("    public static IHostBuilder ConfigureLogging(this IHostBuilder host)");
        sb.AppendLine("    {");
        sb.AppendLine("        host.UseSerilog((context, configuration) =>");
        sb.AppendLine("        {");
        sb.AppendLine("            configuration");
        sb.AppendLine("                .ReadFrom.Configuration(context.Configuration)");
        sb.AppendLine("                .MinimumLevel.Information()");
        sb.AppendLine("                .MinimumLevel.Override(\"Microsoft\", LogEventLevel.Warning)");
        sb.AppendLine("                .MinimumLevel.Override(\"Microsoft.Hosting.Lifetime\", LogEventLevel.Information)");
        sb.AppendLine("                .MinimumLevel.Override(\"System\", LogEventLevel.Warning)");
        sb.AppendLine("                .Enrich.FromLogContext()");
        sb.AppendLine("                .Enrich.WithMachineName()");
        sb.AppendLine("                .Enrich.WithEnvironmentName()");
        sb.AppendLine("                .WriteTo.Console(");
        sb.AppendLine("                    outputTemplate: \"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}\")");
        sb.AppendLine("                .WriteTo.File(");
        sb.AppendLine("                    path: \"logs/log-.txt\",");
        sb.AppendLine("                    rollingInterval: RollingInterval.Day,");
        sb.AppendLine("                    retainedFileCountLimit: 30,");
        sb.AppendLine("                    outputTemplate: \"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}\");");
        sb.AppendLine("        });");
        sb.AppendLine();
        sb.AppendLine("        return host;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateAppSettingsJson(string rootNamespace, DatabaseProvider dbProvider)
    {
        var connectionString = dbProvider switch
        {
            DatabaseProvider.SqlServer => "Server=localhost;Database=MyAppDb;Trusted_Connection=true;TrustServerCertificate=true;",
            DatabaseProvider.PostgreSql => "Host=localhost;Database=myappdb;Username=postgres;Password=postgres;",
            DatabaseProvider.MySql => "Server=localhost;Database=myappdb;User=root;Password=root;",
            DatabaseProvider.Sqlite => "Data Source=myapp.db",
            _ => "Server=localhost;Database=MyAppDb;Trusted_Connection=true;TrustServerCertificate=true;"
        };
        
        var sb = new StringBuilder();
        
        sb.AppendLine("{");
        sb.AppendLine("  \"ConnectionStrings\": {");
        sb.AppendLine($"    \"DefaultConnection\": \"{connectionString}\"");
        sb.AppendLine("  },");
        sb.AppendLine("  \"Cors\": {");
        sb.AppendLine("    \"AllowedOrigins\": [");
        sb.AppendLine("      \"http://localhost:3000\",");
        sb.AppendLine("      \"http://localhost:4200\",");
        sb.AppendLine("      \"http://localhost:5173\"");
        sb.AppendLine("    ]");
        sb.AppendLine("  },");
        sb.AppendLine("  \"Serilog\": {");
        sb.AppendLine("    \"MinimumLevel\": {");
        sb.AppendLine("      \"Default\": \"Information\",");
        sb.AppendLine("      \"Override\": {");
        sb.AppendLine("        \"Microsoft\": \"Warning\",");
        sb.AppendLine("        \"System\": \"Warning\"");
        sb.AppendLine("      }");
        sb.AppendLine("    }");
        sb.AppendLine("  },");
        sb.AppendLine("  \"Logging\": {");
        sb.AppendLine("    \"LogLevel\": {");
        sb.AppendLine("      \"Default\": \"Information\",");
        sb.AppendLine("      \"Microsoft.AspNetCore\": \"Warning\"");
        sb.AppendLine("    }");
        sb.AppendLine("  },");
        sb.AppendLine("  \"AllowedHosts\": \"*\"");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
}
