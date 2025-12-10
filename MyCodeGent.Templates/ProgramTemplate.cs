using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class ProgramTemplate
{
    public static string Generate(GenerationConfig config, List<EntityModel> entities)
    {
        var sb = new StringBuilder();
        
        // Using statements
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {config.RootNamespace}.Application.Common.Interfaces;");
        sb.AppendLine($"using {config.RootNamespace}.Infrastructure.Persistence;");
        
        if (config.UseMediator)
        {
            sb.AppendLine("using MediatR;");
        }
        
        if (config.UseFluentValidation)
        {
            sb.AppendLine("using FluentValidation;");
            sb.AppendLine("using FluentValidation.AspNetCore;");
        }
        
        if (config.GenerateSwagger)
        {
            sb.AppendLine("using Microsoft.OpenApi.Models;");
        }
        
        if (config.LoggingProvider == LoggingProvider.Serilog)
        {
            sb.AppendLine("using Serilog;");
        }
        
        if (config.GenerateAuthentication)
        {
            sb.AppendLine("using Microsoft.AspNetCore.Authentication.JwtBearer;");
            sb.AppendLine("using Microsoft.IdentityModel.Tokens;");
            sb.AppendLine("using System.Text;");
        }
        
        sb.AppendLine();
        
        // Builder creation
        sb.AppendLine("var builder = WebApplication.CreateBuilder(args);");
        sb.AppendLine();
        
        // Logging configuration
        if (config.LoggingProvider == LoggingProvider.Serilog)
        {
            sb.AppendLine("// Configure Serilog");
            sb.AppendLine("Log.Logger = new LoggerConfiguration()");
            sb.AppendLine("    .ReadFrom.Configuration(builder.Configuration)");
            sb.AppendLine("    .Enrich.FromLogContext()");
            sb.AppendLine("    .WriteTo.Console()");
            sb.AppendLine("    .WriteTo.File(\"logs/log-.txt\", rollingInterval: RollingInterval.Day)");
            sb.AppendLine("    .CreateLogger();");
            sb.AppendLine();
            sb.AppendLine("builder.Host.UseSerilog();");
            sb.AppendLine();
        }
        
        // Add services to the container
        sb.AppendLine("// Add services to the container");
        sb.AppendLine("builder.Services.AddControllers();");
        sb.AppendLine();
        
        // Database configuration
        sb.AppendLine("// Configure Database");
        var dbProvider = config.DatabaseProvider switch
        {
            DatabaseProvider.SqlServer => "UseSqlServer",
            DatabaseProvider.PostgreSql => "UseNpgsql",
            DatabaseProvider.MySql => "UseMySql",
            DatabaseProvider.Sqlite => "UseSqlite",
            DatabaseProvider.InMemory => "UseInMemoryDatabase",
            _ => "UseSqlServer"
        };
        
        sb.AppendLine($"builder.Services.AddDbContext<ApplicationDbContext>(options =>");
        if (config.DatabaseProvider == DatabaseProvider.InMemory)
        {
            sb.AppendLine($"    options.{dbProvider}(\"{config.RootNamespace}Db\"));");
        }
        else
        {
            sb.AppendLine($"    options.{dbProvider}(builder.Configuration.GetConnectionString(\"DefaultConnection\")));");
        }
        sb.AppendLine();
        sb.AppendLine("builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());");
        sb.AppendLine();
        
        // MediatR
        if (config.UseMediator)
        {
            sb.AppendLine("// Configure MediatR");
            sb.AppendLine($"builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationDbContext).Assembly));");
            sb.AppendLine();
        }
        
        // FluentValidation
        if (config.UseFluentValidation)
        {
            sb.AppendLine("// Configure FluentValidation");
            sb.AppendLine($"builder.Services.AddValidatorsFromAssembly(typeof(IApplicationDbContext).Assembly);");
            sb.AppendLine();
        }
        
        // AutoMapper
        if (config.UseAutoMapper)
        {
            sb.AppendLine("// Configure AutoMapper");
            sb.AppendLine($"builder.Services.AddAutoMapper(typeof(IApplicationDbContext).Assembly);");
            sb.AppendLine();
        }
        
        // Authentication
        if (config.GenerateAuthentication && config.AuthenticationType == AuthenticationType.JWT)
        {
            sb.AppendLine("// Configure JWT Authentication");
            sb.AppendLine("builder.Services.AddAuthentication(options =>");
            sb.AppendLine("{");
            sb.AppendLine("    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;");
            sb.AppendLine("    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;");
            sb.AppendLine("})");
            sb.AppendLine(".AddJwtBearer(options =>");
            sb.AppendLine("{");
            sb.AppendLine("    options.TokenValidationParameters = new TokenValidationParameters");
            sb.AppendLine("    {");
            sb.AppendLine("        ValidateIssuer = true,");
            sb.AppendLine("        ValidateAudience = true,");
            sb.AppendLine("        ValidateLifetime = true,");
            sb.AppendLine("        ValidateIssuerSigningKey = true,");
            sb.AppendLine("        ValidIssuer = builder.Configuration[\"Jwt:Issuer\"],");
            sb.AppendLine("        ValidAudience = builder.Configuration[\"Jwt:Audience\"],");
            sb.AppendLine("        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[\"Jwt:Key\"]!))");
            sb.AppendLine("    };");
            sb.AppendLine("});");
            sb.AppendLine();
            sb.AppendLine("builder.Services.AddAuthorization();");
            sb.AppendLine();
        }
        
        // CORS
        if (config.GenerateCors)
        {
            sb.AppendLine("// Configure CORS");
            sb.AppendLine("builder.Services.AddCors(options =>");
            sb.AppendLine("{");
            sb.AppendLine("    options.AddPolicy(\"AllowSpecificOrigins\", policy =>");
            sb.AppendLine("    {");
            sb.AppendLine($"        policy.WithOrigins({string.Join(", ", config.AllowedOrigins.Select(o => $"\"{o}\""))})");
            sb.AppendLine("              .AllowAnyMethod()");
            sb.AppendLine("              .AllowAnyHeader()");
            sb.AppendLine("              .AllowCredentials();");
            sb.AppendLine("    });");
            sb.AppendLine("});");
            sb.AppendLine();
        }
        
        // Caching
        if (config.GenerateCaching)
        {
            sb.AppendLine("// Configure Caching");
            if (config.CachingProvider == CachingProvider.Memory)
            {
                sb.AppendLine("builder.Services.AddMemoryCache();");
            }
            else if (config.CachingProvider == CachingProvider.Redis)
            {
                sb.AppendLine("builder.Services.AddStackExchangeRedisCache(options =>");
                sb.AppendLine("{");
                sb.AppendLine("    options.Configuration = builder.Configuration.GetConnectionString(\"Redis\");");
                sb.AppendLine("});");
            }
            sb.AppendLine();
        }
        
        // Response Compression
        if (config.GenerateResponseCompression)
        {
            sb.AppendLine("// Configure Response Compression");
            sb.AppendLine("builder.Services.AddResponseCompression(options =>");
            sb.AppendLine("{");
            sb.AppendLine("    options.EnableForHttps = true;");
            sb.AppendLine("});");
            sb.AppendLine();
        }
        
        // Health Checks
        if (config.GenerateHealthChecks)
        {
            sb.AppendLine("// Configure Health Checks");
            sb.AppendLine("builder.Services.AddHealthChecks()");
            sb.AppendLine("    .AddDbContextCheck<ApplicationDbContext>();");
            sb.AppendLine();
        }
        
        // Swagger/OpenAPI
        if (config.GenerateSwagger)
        {
            sb.AppendLine("// Configure Swagger/OpenAPI");
            sb.AppendLine("builder.Services.AddEndpointsApiExplorer();");
            sb.AppendLine("builder.Services.AddSwaggerGen(c =>");
            sb.AppendLine("{");
            sb.AppendLine($"    c.SwaggerDoc(\"v1\", new OpenApiInfo");
            sb.AppendLine("    {");
            sb.AppendLine($"        Title = \"{config.RootNamespace} API\",");
            sb.AppendLine("        Version = \"v1\",");
            sb.AppendLine($"        Description = \"API for {config.RootNamespace} application\",");
            sb.AppendLine("    });");
            
            if (config.GenerateXmlDocumentation)
            {
                sb.AppendLine();
                sb.AppendLine("    // Include XML comments");
                sb.AppendLine($"    var xmlFile = $\"{{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}}.xml\";");
                sb.AppendLine("    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);");
                sb.AppendLine("    if (File.Exists(xmlPath))");
                sb.AppendLine("    {");
                sb.AppendLine("        c.IncludeXmlComments(xmlPath);");
                sb.AppendLine("    }");
            }
            
            if (config.GenerateAuthentication)
            {
                sb.AppendLine();
                sb.AppendLine("    // Add JWT Authentication to Swagger");
                sb.AppendLine("    c.AddSecurityDefinition(\"Bearer\", new OpenApiSecurityScheme");
                sb.AppendLine("    {");
                sb.AppendLine("        Description = \"JWT Authorization header using the Bearer scheme\",");
                sb.AppendLine("        Name = \"Authorization\",");
                sb.AppendLine("        In = ParameterLocation.Header,");
                sb.AppendLine("        Type = SecuritySchemeType.ApiKey,");
                sb.AppendLine("        Scheme = \"Bearer\"");
                sb.AppendLine("    });");
                sb.AppendLine();
                sb.AppendLine("    c.AddSecurityRequirement(new OpenApiSecurityRequirement");
                sb.AppendLine("    {");
                sb.AppendLine("        {");
                sb.AppendLine("            new OpenApiSecurityScheme");
                sb.AppendLine("            {");
                sb.AppendLine("                Reference = new OpenApiReference");
                sb.AppendLine("                {");
                sb.AppendLine("                    Type = ReferenceType.SecurityScheme,");
                sb.AppendLine("                    Id = \"Bearer\"");
                sb.AppendLine("                }");
                sb.AppendLine("            },");
                sb.AppendLine("            Array.Empty<string>()");
                sb.AppendLine("        }");
                sb.AppendLine("    });");
            }
            
            sb.AppendLine("});");
            sb.AppendLine();
        }
        
        // Build the app
        sb.AppendLine("var app = builder.Build();");
        sb.AppendLine();
        
        // Configure the HTTP request pipeline
        sb.AppendLine("// Configure the HTTP request pipeline");
        
        // Global Exception Handler
        if (config.GenerateGlobalExceptionHandler)
        {
            sb.AppendLine("app.UseExceptionHandler(\"/error\");");
        }
        
        // Response Compression
        if (config.GenerateResponseCompression)
        {
            sb.AppendLine("app.UseResponseCompression();");
        }
        
        // Swagger
        if (config.GenerateSwagger)
        {
            sb.AppendLine();
            sb.AppendLine("if (app.Environment.IsDevelopment())");
            sb.AppendLine("{");
            sb.AppendLine("    app.UseSwagger();");
            sb.AppendLine("    app.UseSwaggerUI(c =>");
            sb.AppendLine("    {");
            sb.AppendLine($"        c.SwaggerEndpoint(\"/swagger/v1/swagger.json\", \"{config.RootNamespace} API V1\");");
            sb.AppendLine("        c.RoutePrefix = \"swagger\";");
            sb.AppendLine("    });");
            sb.AppendLine("}");
        }
        
        sb.AppendLine();
        sb.AppendLine("app.UseHttpsRedirection();");
        
        // CORS
        if (config.GenerateCors)
        {
            sb.AppendLine("app.UseCors(\"AllowSpecificOrigins\");");
        }
        
        // Authentication & Authorization
        if (config.GenerateAuthentication)
        {
            sb.AppendLine("app.UseAuthentication();");
        }
        sb.AppendLine("app.UseAuthorization();");
        
        // Health Checks
        if (config.GenerateHealthChecks)
        {
            sb.AppendLine();
            sb.AppendLine("app.MapHealthChecks(\"/health\");");
        }
        
        sb.AppendLine();
        sb.AppendLine("app.MapControllers();");
        sb.AppendLine();
        
        // Database Migration
        if (config.GenerateMigrations)
        {
            sb.AppendLine("// Apply migrations automatically");
            sb.AppendLine("using (var scope = app.Services.CreateScope())");
            sb.AppendLine("{");
            sb.AppendLine("    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();");
            sb.AppendLine("    dbContext.Database.Migrate();");
            sb.AppendLine("}");
            sb.AppendLine();
        }
        
        // Logging
        if (config.LoggingProvider == LoggingProvider.Serilog)
        {
            sb.AppendLine("app.Logger.LogInformation(\"Application started successfully\");");
        }
        else
        {
            sb.AppendLine("app.Logger.LogInformation(\"Application started successfully\");");
        }
        
        sb.AppendLine();
        sb.AppendLine("app.Run();");
        
        return sb.ToString();
    }
}
