using MyCodeGent.Core.Interfaces;
using MyCodeGent.Core.Services;
using MyCodeGent.Web.Middleware;
using MyCodeGent.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IGitVersionService, GitVersionService>();
builder.Services.AddSingleton<IVersionService, VersionService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization to handle enums as strings
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    })
    .ConfigureApplicationPartManager(manager =>
    {
        // Remove Templates assembly from API scanning to avoid reflection errors
        var templatesAssembly = manager.ApplicationParts
            .FirstOrDefault(p => p.Name == "MyCodeGent.Templates");
        if (templatesAssembly != null)
        {
            manager.ApplicationParts.Remove(templatesAssembly);
        }
    });

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MyCodeGent API",
        Version = "v1",
        Description = "API for generating Clean Architecture CRUD code",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "MyCodeGent",
            Url = new Uri("https://github.com/yourusername/mycodegent")
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register MyCodeGent services
builder.Services.AddScoped<IFileWriter, FileWriter>();
builder.Services.AddScoped<ICodeGenerator, CodeGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline
// Global exception handler MUST be first
app.UseMiddleware<GlobalExceptionHandler>();

// Static files MUST come before routing
app.UseDefaultFiles();
app.UseStaticFiles();

// Enable Swagger in all environments (can be restricted to Development if needed)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyCodeGent API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("MyCodeGent Web API started successfully");
logger.LogInformation("Swagger UI available at: /swagger");
logger.LogInformation("Web UI available at: /");

app.Run();
