using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class DocumentationTemplate
{
    public static string GenerateReadme(GenerationConfig config, List<EntityModel> entities)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"# {config.RootNamespace}");
        sb.AppendLine();
        sb.AppendLine($"A Clean Architecture ASP.NET Core Web API application generated with MyCodeGent.");
        sb.AppendLine();
        sb.AppendLine("## 🏗️ Architecture");
        sb.AppendLine();
        sb.AppendLine("This project follows Clean Architecture principles with CQRS pattern:");
        sb.AppendLine();
        sb.AppendLine("- **Domain Layer**: Entities and domain logic");
        sb.AppendLine("- **Application Layer**: Business logic, CQRS commands/queries, DTOs");
        sb.AppendLine("- **Infrastructure Layer**: Data access, EF Core, external services");
        sb.AppendLine("- **API Layer**: RESTful API controllers, middleware");
        sb.AppendLine();
        
        sb.AppendLine("## 🚀 Getting Started");
        sb.AppendLine();
        sb.AppendLine("### Prerequisites");
        sb.AppendLine();
        sb.AppendLine("- .NET 9.0 SDK");
        sb.AppendLine($"- {GetDatabaseName(config.DatabaseProvider)}");
        
        if (config.CachingProvider == CachingProvider.Redis)
        {
            sb.AppendLine("- Redis (for caching)");
        }
        
        sb.AppendLine();
        sb.AppendLine("### Installation");
        sb.AppendLine();
        sb.AppendLine("1. **Clone the repository**");
        sb.AppendLine("   ```bash");
        sb.AppendLine($"   git clone https://github.com/yourusername/{config.RootNamespace.ToLower()}.git");
        sb.AppendLine($"   cd {config.RootNamespace.ToLower()}");
        sb.AppendLine("   ```");
        sb.AppendLine();
        
        sb.AppendLine("2. **Update connection string**");
        sb.AppendLine($"   Edit `{config.RootNamespace}.Api/appsettings.json` and update the connection string:");
        sb.AppendLine("   ```json");
        sb.AppendLine("   \"ConnectionStrings\": {");
        sb.AppendLine($"     \"DefaultConnection\": \"{config.ConnectionString}\"");
        sb.AppendLine("   }");
        sb.AppendLine("   ```");
        sb.AppendLine();
        
        if (config.GenerateMigrations)
        {
            sb.AppendLine("3. **Apply database migrations**");
            sb.AppendLine("   ```bash");
            sb.AppendLine($"   cd {config.RootNamespace}.Api");
            sb.AppendLine("   dotnet ef database update");
            sb.AppendLine("   ```");
            sb.AppendLine();
        }
        
        sb.AppendLine($"{(config.GenerateMigrations ? "4" : "3")}. **Run the application**");
        sb.AppendLine("   ```bash");
        sb.AppendLine("   dotnet run");
        sb.AppendLine("   ```");
        sb.AppendLine();
        
        sb.AppendLine($"{(config.GenerateMigrations ? "5" : "4")}. **Access the API**");
        if (config.GenerateSwagger)
        {
            sb.AppendLine("   - Swagger UI: `https://localhost:5001/swagger`");
        }
        if (config.GenerateHealthChecks)
        {
            sb.AppendLine("   - Health Check: `https://localhost:5001/health`");
        }
        sb.AppendLine("   - API Base: `https://localhost:5001/api`");
        sb.AppendLine();
        
        sb.AppendLine("## 📚 API Endpoints");
        sb.AppendLine();
        sb.AppendLine("### Entities");
        sb.AppendLine();
        
        foreach (var entity in entities)
        {
            sb.AppendLine($"#### {entity.Name}");
            sb.AppendLine();
            sb.AppendLine($"- `GET /api/{entity.Name.ToLower()}s` - Get all {entity.Name.ToLower()}s");
            sb.AppendLine($"- `GET /api/{entity.Name.ToLower()}s/{{id}}` - Get {entity.Name.ToLower()} by ID");
            sb.AppendLine($"- `POST /api/{entity.Name.ToLower()}s` - Create new {entity.Name.ToLower()}");
            sb.AppendLine($"- `PUT /api/{entity.Name.ToLower()}s/{{id}}` - Update {entity.Name.ToLower()}");
            sb.AppendLine($"- `DELETE /api/{entity.Name.ToLower()}s/{{id}}` - Delete {entity.Name.ToLower()}");
            sb.AppendLine();
        }
        
        if (config.GenerateAuthentication)
        {
            sb.AppendLine("### Authentication");
            sb.AppendLine();
            sb.AppendLine("- `POST /api/auth/login` - Login");
            sb.AppendLine("- `POST /api/auth/register` - Register new user");
            sb.AppendLine("- `POST /api/auth/refresh` - Refresh token");
            sb.AppendLine();
        }
        
        sb.AppendLine("## 🛠️ Technologies Used");
        sb.AppendLine();
        sb.AppendLine("- **Framework**: ASP.NET Core 9.0");
        sb.AppendLine($"- **Database**: {GetDatabaseName(config.DatabaseProvider)}");
        sb.AppendLine("- **ORM**: Entity Framework Core 9.0");
        
        if (config.UseMediator)
        {
            sb.AppendLine("- **CQRS**: MediatR");
        }
        
        if (config.UseFluentValidation)
        {
            sb.AppendLine("- **Validation**: FluentValidation");
        }
        
        if (config.UseAutoMapper)
        {
            sb.AppendLine("- **Mapping**: AutoMapper");
        }
        
        if (config.GenerateSwagger)
        {
            sb.AppendLine("- **API Documentation**: Swagger/OpenAPI");
        }
        
        if (config.LoggingProvider == LoggingProvider.Serilog)
        {
            sb.AppendLine("- **Logging**: Serilog");
        }
        
        if (config.GenerateAuthentication)
        {
            sb.AppendLine($"- **Authentication**: {config.AuthenticationType}");
        }
        
        if (config.CachingProvider != CachingProvider.None)
        {
            sb.AppendLine($"- **Caching**: {config.CachingProvider}");
        }
        
        sb.AppendLine();
        
        sb.AppendLine("## 📁 Project Structure");
        sb.AppendLine();
        sb.AppendLine("```");
        sb.AppendLine($"{config.RootNamespace}/");
        sb.AppendLine($"├── {config.RootNamespace}.Domain/");
        sb.AppendLine("│   └── Entities/");
        foreach (var entity in entities.Take(3))
        {
            sb.AppendLine($"│       ├── {entity.Name}.cs");
        }
        if (entities.Count > 3)
        {
            sb.AppendLine($"│       └── ... ({entities.Count - 3} more)");
        }
        sb.AppendLine($"├── {config.RootNamespace}.Application/");
        sb.AppendLine("│   ├── Common/");
        sb.AppendLine("│   │   ├── Interfaces/");
        sb.AppendLine("│   │   └── Models/");
        foreach (var entity in entities.Take(2))
        {
            sb.AppendLine($"│   ├── {entity.Name}s/");
            sb.AppendLine("│   │   ├── Commands/");
            sb.AppendLine("│   │   ├── Queries/");
            sb.AppendLine($"│   │   └── {entity.Name}Dto.cs");
        }
        if (entities.Count > 2)
        {
            sb.AppendLine($"│   └── ... ({entities.Count - 2} more entities)");
        }
        sb.AppendLine($"├── {config.RootNamespace}.Infrastructure/");
        sb.AppendLine("│   └── Persistence/");
        sb.AppendLine("│       ├── ApplicationDbContext.cs");
        sb.AppendLine("│       └── Configurations/");
        sb.AppendLine($"└── {config.RootNamespace}.Api/");
        sb.AppendLine("    ├── Controllers/");
        sb.AppendLine("    ├── Program.cs");
        sb.AppendLine("    └── appsettings.json");
        sb.AppendLine("```");
        sb.AppendLine();
        
        if (config.GenerateDockerfile)
        {
            sb.AppendLine("## 🐳 Docker");
            sb.AppendLine();
            sb.AppendLine("### Build Docker Image");
            sb.AppendLine("```bash");
            sb.AppendLine($"docker build -t {config.RootNamespace.ToLower()}-api .");
            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("### Run with Docker Compose");
            sb.AppendLine("```bash");
            sb.AppendLine("docker-compose up");
            sb.AppendLine("```");
            sb.AppendLine();
        }
        
        if (config.GenerateUnitTests || config.GenerateIntegrationTests)
        {
            sb.AppendLine("## 🧪 Testing");
            sb.AppendLine();
            sb.AppendLine("### Run Tests");
            sb.AppendLine("```bash");
            sb.AppendLine("dotnet test");
            sb.AppendLine("```");
            sb.AppendLine();
        }
        
        sb.AppendLine("## 📝 Configuration");
        sb.AppendLine();
        sb.AppendLine("### Environment Variables");
        sb.AppendLine();
        sb.AppendLine("For production, use environment variables instead of appsettings.json:");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine("export ConnectionStrings__DefaultConnection=\"your-connection-string\"");
        
        if (config.GenerateAuthentication)
        {
            sb.AppendLine("export Jwt__Key=\"your-secret-key\"");
            sb.AppendLine("export Jwt__Issuer=\"your-issuer\"");
            sb.AppendLine("export Jwt__Audience=\"your-audience\"");
        }
        
        sb.AppendLine("```");
        sb.AppendLine();
        
        sb.AppendLine("## 🤝 Contributing");
        sb.AppendLine();
        sb.AppendLine("1. Fork the repository");
        sb.AppendLine("2. Create a feature branch (`git checkout -b feature/AmazingFeature`)");
        sb.AppendLine("3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)");
        sb.AppendLine("4. Push to the branch (`git push origin feature/AmazingFeature`)");
        sb.AppendLine("5. Open a Pull Request");
        sb.AppendLine();
        
        sb.AppendLine("## 📄 License");
        sb.AppendLine();
        sb.AppendLine("This project is licensed under the MIT License.");
        sb.AppendLine();
        
        sb.AppendLine("## 📧 Contact");
        sb.AppendLine();
        sb.AppendLine("Your Name - your.email@example.com");
        sb.AppendLine();
        sb.AppendLine($"Project Link: [https://github.com/yourusername/{config.RootNamespace.ToLower()}](https://github.com/yourusername/{config.RootNamespace.ToLower()})");
        sb.AppendLine();
        
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("**Generated with ❤️ by MyCodeGent**");
        
        return sb.ToString();
    }
    
    public static string GenerateArchitectureDoc(GenerationConfig config)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"# {config.RootNamespace} - Architecture Documentation");
        sb.AppendLine();
        sb.AppendLine("## Overview");
        sb.AppendLine();
        sb.AppendLine("This application follows Clean Architecture principles, ensuring:");
        sb.AppendLine();
        sb.AppendLine("- **Independence of Frameworks**: The architecture doesn't depend on external libraries");
        sb.AppendLine("- **Testability**: Business rules can be tested without UI, database, or external elements");
        sb.AppendLine("- **Independence of UI**: The UI can change without changing the rest of the system");
        sb.AppendLine("- **Independence of Database**: Business rules are not bound to the database");
        sb.AppendLine("- **Independence of External Agency**: Business rules don't know about external interfaces");
        sb.AppendLine();
        
        sb.AppendLine("## Layers");
        sb.AppendLine();
        sb.AppendLine("### 1. Domain Layer");
        sb.AppendLine();
        sb.AppendLine("The innermost layer containing:");
        sb.AppendLine("- **Entities**: Core business objects");
        sb.AppendLine("- **Value Objects**: Immutable objects defined by their attributes");
        sb.AppendLine("- **Domain Events**: Events that occur in the domain");
        sb.AppendLine();
        sb.AppendLine("**Dependencies**: None");
        sb.AppendLine();
        
        sb.AppendLine("### 2. Application Layer");
        sb.AppendLine();
        sb.AppendLine("Contains application business logic:");
        sb.AppendLine("- **Commands**: Write operations (Create, Update, Delete)");
        sb.AppendLine("- **Queries**: Read operations (GetById, GetAll)");
        sb.AppendLine("- **Handlers**: Process commands and queries");
        sb.AppendLine("- **DTOs**: Data transfer objects");
        sb.AppendLine("- **Validators**: Input validation rules");
        sb.AppendLine("- **Interfaces**: Abstractions for infrastructure");
        sb.AppendLine();
        sb.AppendLine("**Dependencies**: Domain Layer");
        sb.AppendLine();
        
        sb.AppendLine("### 3. Infrastructure Layer");
        sb.AppendLine();
        sb.AppendLine("Implements external concerns:");
        sb.AppendLine("- **Persistence**: EF Core DbContext, configurations");
        sb.AppendLine("- **External Services**: Email, SMS, file storage");
        sb.AppendLine("- **Identity**: Authentication and authorization");
        sb.AppendLine();
        sb.AppendLine("**Dependencies**: Application Layer, Domain Layer");
        sb.AppendLine();
        
        sb.AppendLine("### 4. API Layer");
        sb.AppendLine();
        sb.AppendLine("The entry point of the application:");
        sb.AppendLine("- **Controllers**: REST API endpoints");
        sb.AppendLine("- **Middleware**: Request/response pipeline");
        sb.AppendLine("- **Filters**: Exception handling, validation");
        sb.AppendLine("- **Configuration**: Startup, dependency injection");
        sb.AppendLine();
        sb.AppendLine("**Dependencies**: Application Layer, Infrastructure Layer");
        sb.AppendLine();
        
        sb.AppendLine("## Patterns");
        sb.AppendLine();
        
        if (config.UseMediator)
        {
            sb.AppendLine("### CQRS (Command Query Responsibility Segregation)");
            sb.AppendLine();
            sb.AppendLine("Separates read and write operations:");
            sb.AppendLine("- **Commands**: Modify state, return void or ID");
            sb.AppendLine("- **Queries**: Return data, don't modify state");
            sb.AppendLine();
            sb.AppendLine("Implemented using MediatR for loose coupling.");
            sb.AppendLine();
        }
        
        sb.AppendLine("### Repository Pattern");
        sb.AppendLine();
        sb.AppendLine("Abstracts data access through `IApplicationDbContext` interface.");
        sb.AppendLine();
        
        if (config.UseFluentValidation)
        {
            sb.AppendLine("### Validation");
            sb.AppendLine();
            sb.AppendLine("FluentValidation for declarative validation rules.");
            sb.AppendLine();
        }
        
        sb.AppendLine("## Data Flow");
        sb.AppendLine();
        sb.AppendLine("```");
        sb.AppendLine("HTTP Request");
        sb.AppendLine("    ↓");
        sb.AppendLine("Controller (API Layer)");
        sb.AppendLine("    ↓");
        sb.AppendLine("Command/Query (Application Layer)");
        sb.AppendLine("    ↓");
        sb.AppendLine("Handler (Application Layer)");
        sb.AppendLine("    ↓");
        sb.AppendLine("DbContext (Infrastructure Layer)");
        sb.AppendLine("    ↓");
        sb.AppendLine("Database");
        sb.AppendLine("```");
        sb.AppendLine();
        
        sb.AppendLine("## Dependency Injection");
        sb.AppendLine();
        sb.AppendLine("All dependencies are registered in `Program.cs`:");
        sb.AppendLine();
        sb.AppendLine("- **Scoped**: DbContext, Application services");
        sb.AppendLine("- **Transient**: Validators, Mappers");
        sb.AppendLine("- **Singleton**: Configuration, Logging");
        sb.AppendLine();
        
        return sb.ToString();
    }
    
    private static string GetDatabaseName(DatabaseProvider provider)
    {
        return provider switch
        {
            DatabaseProvider.SqlServer => "SQL Server",
            DatabaseProvider.PostgreSql => "PostgreSQL",
            DatabaseProvider.MySql => "MySQL",
            DatabaseProvider.Sqlite => "SQLite",
            DatabaseProvider.InMemory => "In-Memory Database",
            _ => "Database"
        };
    }
}
