using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class ProjectFileTemplate
{
    public static string GenerateApiProject(GenerationConfig config)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk.Web\">");
        sb.AppendLine();
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <TargetFramework>net9.0</TargetFramework>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        
        if (config.GenerateXmlDocumentation)
        {
            sb.AppendLine("    <GenerateDocumentationFile>true</GenerateDocumentationFile>");
            sb.AppendLine("    <NoWarn>$(NoWarn);1591</NoWarn>");
        }
        
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine();
        sb.AppendLine("  <ItemGroup>");
        
        // Core packages
        if (config.GenerateSwagger)
        {
            sb.AppendLine("    <PackageReference Include=\"Swashbuckle.AspNetCore\" Version=\"7.2.0\" />");
        }
        
        // Database packages
        if (config.GenerateInfrastructure)
        {
            switch (config.DatabaseProvider)
            {
                case DatabaseProvider.SqlServer:
                    sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.SqlServer\" Version=\"9.0.0\" />");
                    break;
                case DatabaseProvider.PostgreSql:
                    sb.AppendLine("    <PackageReference Include=\"Npgsql.EntityFrameworkCore.PostgreSQL\" Version=\"9.0.0\" />");
                    break;
                case DatabaseProvider.MySql:
                    sb.AppendLine("    <PackageReference Include=\"Pomelo.EntityFrameworkCore.MySql\" Version=\"9.0.0\" />");
                    break;
                case DatabaseProvider.Sqlite:
                    sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Sqlite\" Version=\"9.0.0\" />");
                    break;
                case DatabaseProvider.InMemory:
                    sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.InMemory\" Version=\"9.0.0\" />");
                    break;
            }
            
            sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Design\" Version=\"9.0.0\">");
            sb.AppendLine("      <PrivateAssets>all</PrivateAssets>");
            sb.AppendLine("      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>");
            sb.AppendLine("    </PackageReference>");
            sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Tools\" Version=\"9.0.0\">");
            sb.AppendLine("      <PrivateAssets>all</PrivateAssets>");
            sb.AppendLine("      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>");
            sb.AppendLine("    </PackageReference>");
        }
        
        // MediatR
        if (config.UseMediator)
        {
            sb.AppendLine("    <PackageReference Include=\"MediatR\" Version=\"12.4.1\" />");
        }
        
        // FluentValidation
        if (config.UseFluentValidation)
        {
            sb.AppendLine("    <PackageReference Include=\"FluentValidation\" Version=\"11.11.0\" />");
            sb.AppendLine("    <PackageReference Include=\"FluentValidation.AspNetCore\" Version=\"11.3.0\" />");
            sb.AppendLine("    <PackageReference Include=\"FluentValidation.DependencyInjectionExtensions\" Version=\"11.11.0\" />");
        }
        
        // AutoMapper
        if (config.UseAutoMapper)
        {
            sb.AppendLine("    <PackageReference Include=\"AutoMapper\" Version=\"12.0.1\" />");
            sb.AppendLine("    <PackageReference Include=\"AutoMapper.Extensions.Microsoft.DependencyInjection\" Version=\"12.0.1\" />");
        }
        
        // Authentication
        if (config.GenerateAuthentication)
        {
            sb.AppendLine("    <PackageReference Include=\"Microsoft.AspNetCore.Authentication.JwtBearer\" Version=\"9.0.0\" />");
            
            if (config.GenerateIdentity)
            {
                sb.AppendLine("    <PackageReference Include=\"Microsoft.AspNetCore.Identity.EntityFrameworkCore\" Version=\"9.0.0\" />");
            }
        }
        
        // Logging
        if (config.LoggingProvider == LoggingProvider.Serilog)
        {
            sb.AppendLine("    <PackageReference Include=\"Serilog.AspNetCore\" Version=\"8.0.2\" />");
            sb.AppendLine("    <PackageReference Include=\"Serilog.Sinks.Console\" Version=\"5.0.1\" />");
            sb.AppendLine("    <PackageReference Include=\"Serilog.Sinks.File\" Version=\"5.0.0\" />");
            sb.AppendLine("    <PackageReference Include=\"Serilog.Enrichers.Environment\" Version=\"3.0.1\" />");
            sb.AppendLine("    <PackageReference Include=\"Serilog.Enrichers.Thread\" Version=\"3.1.0\" />");
        }
        
        // Caching
        if (config.GenerateCaching && config.CachingProvider == CachingProvider.Redis)
        {
            sb.AppendLine("    <PackageReference Include=\"Microsoft.Extensions.Caching.StackExchangeRedis\" Version=\"9.0.0\" />");
        }
        
        // Health Checks
        if (config.GenerateHealthChecks)
        {
            sb.AppendLine("    <PackageReference Include=\"Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore\" Version=\"9.0.0\" />");
        }
        
        // Application Insights
        if (config.GenerateApplicationInsights)
        {
            sb.AppendLine("    <PackageReference Include=\"Microsoft.ApplicationInsights.AspNetCore\" Version=\"2.22.0\" />");
        }
        
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine();
        
        // Project References
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine($"    <ProjectReference Include=\"..\\{config.RootNamespace}.Application\\{config.RootNamespace}.Application.csproj\" />");
        sb.AppendLine($"    <ProjectReference Include=\"..\\{config.RootNamespace}.Infrastructure\\{config.RootNamespace}.Infrastructure.csproj\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine();
        sb.AppendLine("</Project>");
        
        return sb.ToString();
    }
    
    public static string GenerateApplicationProject(GenerationConfig config)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        sb.AppendLine();
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <TargetFramework>net9.0</TargetFramework>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine();
        sb.AppendLine("  <ItemGroup>");
        
        if (config.UseMediator)
        {
            sb.AppendLine("    <PackageReference Include=\"MediatR\" Version=\"12.4.1\" />");
        }
        
        if (config.UseFluentValidation)
        {
            sb.AppendLine("    <PackageReference Include=\"FluentValidation\" Version=\"11.11.0\" />");
        }
        
        if (config.UseAutoMapper)
        {
            sb.AppendLine("    <PackageReference Include=\"AutoMapper\" Version=\"12.0.1\" />");
        }
        
        sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore\" Version=\"9.0.0\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine();
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine($"    <ProjectReference Include=\"..\\{config.RootNamespace}.Domain\\{config.RootNamespace}.Domain.csproj\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine();
        sb.AppendLine("</Project>");
        
        return sb.ToString();
    }
    
    public static string GenerateInfrastructureProject(GenerationConfig config)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        sb.AppendLine();
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <TargetFramework>net9.0</TargetFramework>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine();
        sb.AppendLine("  <ItemGroup>");
        
        switch (config.DatabaseProvider)
        {
            case DatabaseProvider.SqlServer:
                sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.SqlServer\" Version=\"9.0.0\" />");
                break;
            case DatabaseProvider.PostgreSql:
                sb.AppendLine("    <PackageReference Include=\"Npgsql.EntityFrameworkCore.PostgreSQL\" Version=\"9.0.0\" />");
                break;
            case DatabaseProvider.MySql:
                sb.AppendLine("    <PackageReference Include=\"Pomelo.EntityFrameworkCore.MySql\" Version=\"9.0.0\" />");
                break;
            case DatabaseProvider.Sqlite:
                sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Sqlite\" Version=\"9.0.0\" />");
                break;
            case DatabaseProvider.InMemory:
                sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.InMemory\" Version=\"9.0.0\" />");
                break;
        }
        
        sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore\" Version=\"9.0.0\" />");
        sb.AppendLine("    <PackageReference Include=\"Microsoft.Extensions.Diagnostics.HealthChecks\" Version=\"9.0.0\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine();
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine($"    <ProjectReference Include=\"..\\{config.RootNamespace}.Application\\{config.RootNamespace}.Application.csproj\" />");
        sb.AppendLine($"    <ProjectReference Include=\"..\\{config.RootNamespace}.Domain\\{config.RootNamespace}.Domain.csproj\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine();
        sb.AppendLine("</Project>");
        
        return sb.ToString();
    }
    
    public static string GenerateDomainProject(GenerationConfig config)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        sb.AppendLine();
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <TargetFramework>net9.0</TargetFramework>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine();
        sb.AppendLine("</Project>");
        
        return sb.ToString();
    }
}
