using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public class ReadmeTemplate
{
    public static string Generate(List<EntityModel> entities, GenerationConfig config)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"# {config.RootNamespace} - Generated ASP.NET Core Web API");
        sb.AppendLine();
        sb.AppendLine("This project was generated using **MyCodeGent** - A Clean Architecture CRUD Code Generator.");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        
        // Quick Start
        GenerateQuickStart(sb, config);
        
        // Project Structure
        GenerateProjectStructure(sb, config);
        
        // Database Setup
        GenerateDatabaseSetup(sb, config);
        
        // Configuration
        GenerateConfiguration(sb, config);
        
        // Running the Application
        GenerateRunInstructions(sb, config);
        
        // API Documentation
        GenerateApiDocumentation(sb, entities, config);
        
        // What's Included
        GenerateWhatsIncluded(sb, config);
        
        // What's NOT Included (Manual Steps)
        GenerateManualSteps(sb, config);
        
        // Next Steps
        GenerateNextSteps(sb, config);
        
        // Testing
        GenerateTestingSection(sb, config);
        
        // Deployment
        GenerateDeploymentSection(sb, config);
        
        // Troubleshooting
        GenerateTroubleshooting(sb, config);
        
        // Additional Resources
        GenerateResources(sb);
        
        return sb.ToString();
    }
    
    private static void GenerateQuickStart(StringBuilder sb, GenerationConfig config)
    {
        sb.AppendLine("## 🚀 Quick Start");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine("# 1. Navigate to the API project");
        sb.AppendLine($"cd {config.RootNamespace}.API");
        sb.AppendLine();
        sb.AppendLine("# 2. Update connection string in appsettings.json");
        sb.AppendLine("# Edit appsettings.json and set your database connection string");
        sb.AppendLine();
        sb.AppendLine("# 3. Run database migrations");
        sb.AppendLine("dotnet ef database update");
        sb.AppendLine();
        sb.AppendLine("# 4. Run the application");
        sb.AppendLine("dotnet run");
        sb.AppendLine();
        sb.AppendLine("# 5. Open Swagger UI");
        sb.AppendLine("# Navigate to: https://localhost:5001/swagger");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateProjectStructure(StringBuilder sb, GenerationConfig config)
    {
        sb.AppendLine("## 📁 Project Structure");
        sb.AppendLine();
        sb.AppendLine("```");
        sb.AppendLine($"{config.RootNamespace}/");
        
        if (config.GenerateDomain)
        {
            sb.AppendLine($"├── {config.RootNamespace}.Domain/");
            sb.AppendLine("│   ├── Entities/           # Domain entities");
            sb.AppendLine("│   ├── Common/             # Base classes and interfaces");
            sb.AppendLine("│   └── Enums/              # Domain enumerations");
        }
        
        if (config.GenerateApplication)
        {
            sb.AppendLine($"├── {config.RootNamespace}.Application/");
            sb.AppendLine("│   ├── Features/           # CQRS Commands and Queries");
            sb.AppendLine("│   ├── DTOs/               # Data Transfer Objects");
            sb.AppendLine("│   ├── Mappings/           # AutoMapper profiles");
            sb.AppendLine("│   ├── Validators/         # FluentValidation validators");
            sb.AppendLine("│   └── Interfaces/         # Application interfaces");
        }
        
        if (config.GenerateInfrastructure)
        {
            sb.AppendLine($"├── {config.RootNamespace}.Infrastructure/");
            sb.AppendLine("│   ├── Data/               # DbContext and configurations");
            sb.AppendLine("│   ├── Repositories/       # Repository implementations");
            sb.AppendLine("│   └── Migrations/         # EF Core migrations");
        }
        
        if (config.GenerateApi)
        {
            sb.AppendLine($"└── {config.RootNamespace}.API/");
            sb.AppendLine("    ├── Controllers/        # API Controllers");
            sb.AppendLine("    ├── Program.cs          # Application entry point");
            sb.AppendLine("    └── appsettings.json    # Configuration");
        }
        
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("**Architecture:** Clean Architecture + CQRS Pattern");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateDatabaseSetup(StringBuilder sb, GenerationConfig config)
    {
        sb.AppendLine("## 🗄️ Database Setup");
        sb.AppendLine();
        sb.AppendLine($"**Database Provider:** {config.DatabaseProvider}");
        sb.AppendLine();
        
        sb.AppendLine("### Step 1: Update Connection String");
        sb.AppendLine();
        sb.AppendLine("Edit `appsettings.json` in the API project:");
        sb.AppendLine();
        sb.AppendLine("```json");
        sb.AppendLine("{");
        sb.AppendLine("  \"ConnectionStrings\": {");
        
        switch (config.DatabaseProvider)
        {
            case DatabaseProvider.SqlServer:
                sb.AppendLine($"    \"DefaultConnection\": \"Server=localhost;Database={config.RootNamespace}Db;Trusted_Connection=true;TrustServerCertificate=true;\"");
                break;
            case DatabaseProvider.PostgreSql:
                sb.AppendLine($"    \"DefaultConnection\": \"Host=localhost;Database={config.RootNamespace.ToLower()}db;Username=postgres;Password=yourpassword\"");
                break;
            case DatabaseProvider.MySql:
                sb.AppendLine($"    \"DefaultConnection\": \"Server=localhost;Database={config.RootNamespace.ToLower()}db;User=root;Password=yourpassword;\"");
                break;
            case DatabaseProvider.Sqlite:
                sb.AppendLine($"    \"DefaultConnection\": \"Data Source={config.RootNamespace.ToLower()}.db\"");
                break;
            case DatabaseProvider.InMemory:
                sb.AppendLine("    \"DefaultConnection\": \"InMemory\" // No connection string needed");
                break;
        }
        
        sb.AppendLine("  }");
        sb.AppendLine("}");
        sb.AppendLine("```");
        sb.AppendLine();
        
        if (config.DatabaseProvider != DatabaseProvider.InMemory)
        {
            sb.AppendLine("### Step 2: Install EF Core Tools (if not already installed)");
            sb.AppendLine();
            sb.AppendLine("```bash");
            sb.AppendLine("dotnet tool install --global dotnet-ef");
            sb.AppendLine("```");
            sb.AppendLine();
            
            sb.AppendLine("### Step 3: Create Initial Migration");
            sb.AppendLine();
            sb.AppendLine("```bash");
            sb.AppendLine($"cd {config.RootNamespace}.API");
            sb.AppendLine("dotnet ef migrations add InitialCreate");
            sb.AppendLine("```");
            sb.AppendLine();
            
            sb.AppendLine("### Step 4: Update Database");
            sb.AppendLine();
            sb.AppendLine("```bash");
            sb.AppendLine("dotnet ef database update");
            sb.AppendLine("```");
            sb.AppendLine();
            
            sb.AppendLine("### Step 5: Verify Database");
            sb.AppendLine();
            sb.AppendLine("Connect to your database and verify that tables were created successfully.");
            sb.AppendLine();
        }
        
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateConfiguration(StringBuilder sb, GenerationConfig config)
    {
        sb.AppendLine("## ⚙️ Configuration");
        sb.AppendLine();
        
        if (config.GenerateAuthentication)
        {
            sb.AppendLine("### Authentication");
            sb.AppendLine();
            sb.AppendLine($"**Type:** {config.AuthenticationType}");
            sb.AppendLine();
            sb.AppendLine("**⚠️ IMPORTANT:** Update JWT settings in `appsettings.json`:");
            sb.AppendLine();
            sb.AppendLine("```json");
            sb.AppendLine("\"JwtSettings\": {");
            sb.AppendLine("  \"SecretKey\": \"CHANGE-THIS-TO-A-SECURE-KEY-AT-LEAST-32-CHARACTERS-LONG\",");
            sb.AppendLine("  \"Issuer\": \"YourIssuer\",");
            sb.AppendLine("  \"Audience\": \"YourAudience\",");
            sb.AppendLine("  \"ExpirationMinutes\": 60");
            sb.AppendLine("}");
            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("**🔒 Security Note:** Never commit real secrets to source control!");
            sb.AppendLine();
        }
        
        if (config.LoggingProvider != LoggingProvider.Default)
        {
            sb.AppendLine("### Logging");
            sb.AppendLine();
            sb.AppendLine($"**Provider:** {config.LoggingProvider}");
            sb.AppendLine();
            sb.AppendLine("Logging is configured in `appsettings.json`. Adjust log levels as needed.");
            sb.AppendLine();
        }
        
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateRunInstructions(StringBuilder sb, GenerationConfig config)
    {
        sb.AppendLine("## ▶️ Running the Application");
        sb.AppendLine();
        sb.AppendLine("### Development Mode");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine($"cd {config.RootNamespace}.API");
        sb.AppendLine("dotnet run");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("### Production Mode");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine("dotnet run --configuration Release");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("### Watch Mode (Auto-reload on changes)");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine("dotnet watch run");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("### Access Points");
        sb.AppendLine();
        sb.AppendLine("- **Swagger UI:** https://localhost:5001/swagger");
        sb.AppendLine("- **API Base URL:** https://localhost:5001/api");
        sb.AppendLine("- **Health Check:** https://localhost:5001/health");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateApiDocumentation(StringBuilder sb, List<EntityModel> entities, GenerationConfig config)
    {
        sb.AppendLine("## 📚 API Endpoints");
        sb.AppendLine();
        sb.AppendLine("All endpoints follow RESTful conventions:");
        sb.AppendLine();
        
        foreach (var entity in entities)
        {
            var entityLower = entity.Name.ToLower();
            sb.AppendLine($"### {entity.Name}");
            sb.AppendLine();
            sb.AppendLine("| Method | Endpoint | Description |");
            sb.AppendLine("|--------|----------|-------------|");
            sb.AppendLine($"| GET | `/api/{entityLower}` | Get all {entity.Name}s |");
            sb.AppendLine($"| GET | `/api/{entityLower}/{{id}}` | Get {entity.Name} by ID |");
            sb.AppendLine($"| POST | `/api/{entityLower}` | Create new {entity.Name} |");
            sb.AppendLine($"| PUT | `/api/{entityLower}/{{id}}` | Update {entity.Name} |");
            sb.AppendLine($"| DELETE | `/api/{entityLower}/{{id}}` | Delete {entity.Name} |");
            sb.AppendLine();
        }
        
        sb.AppendLine("**📖 Full API documentation available at:** `/swagger`");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateWhatsIncluded(StringBuilder sb, GenerationConfig config)
    {
        sb.AppendLine("## ✅ What's Included");
        sb.AppendLine();
        sb.AppendLine("### Architecture & Patterns");
        sb.AppendLine("- ✅ Clean Architecture (Domain, Application, Infrastructure, API)");
        sb.AppendLine("- ✅ CQRS Pattern with MediatR");
        sb.AppendLine("- ✅ Repository Pattern");
        sb.AppendLine("- ✅ Unit of Work Pattern");
        sb.AppendLine();
        
        sb.AppendLine("### Features");
        sb.AppendLine("- ✅ RESTful API Controllers");
        sb.AppendLine("- ✅ Entity Framework Core with " + config.DatabaseProvider);
        sb.AppendLine("- ✅ AutoMapper for object mapping");
        sb.AppendLine("- ✅ FluentValidation for request validation");
        sb.AppendLine("- ✅ Swagger/OpenAPI documentation");
        
        if (config.GenerateAuthentication)
            sb.AppendLine($"- ✅ {config.AuthenticationType} Authentication");
        
        if (config.LoggingProvider != LoggingProvider.Default)
            sb.AppendLine($"- ✅ {config.LoggingProvider} Logging");
        
        sb.AppendLine("- ✅ Global exception handling");
        sb.AppendLine("- ✅ CORS configuration");
        sb.AppendLine("- ✅ Health checks");
        sb.AppendLine();
        
        sb.AppendLine("### Code Quality");
        sb.AppendLine("- ✅ Async/await throughout");
        sb.AppendLine("- ✅ Dependency Injection");
        sb.AppendLine("- ✅ Separation of Concerns");
        sb.AppendLine("- ✅ SOLID Principles");
        sb.AppendLine();
        
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateManualSteps(StringBuilder sb, GenerationConfig config)
    {
        sb.AppendLine("## ⚠️ What's NOT Included (Manual Steps Required)");
        sb.AppendLine();
        sb.AppendLine("The following aspects need to be implemented manually:");
        sb.AppendLine();
        
        sb.AppendLine("### 1. Database Migrations");
        sb.AppendLine("- ❌ **Initial migration not created** - Run `dotnet ef migrations add InitialCreate`");
        sb.AppendLine("- ❌ **Database not created** - Run `dotnet ef database update`");
        sb.AppendLine("- ❌ **Seed data** - Add seed data in DbContext if needed");
        sb.AppendLine();
        
        sb.AppendLine("### 2. Authentication & Authorization");
        if (config.GenerateAuthentication)
        {
            sb.AppendLine("- ⚠️ **JWT Secret Key** - Update with a secure key in appsettings.json");
            sb.AppendLine("- ❌ **User Registration** - Implement user registration endpoint");
            sb.AppendLine("- ❌ **Login Endpoint** - Implement authentication endpoint");
            sb.AppendLine("- ❌ **Password Hashing** - Implement secure password hashing");
            sb.AppendLine("- ❌ **Role Management** - Implement role-based authorization");
            sb.AppendLine("- ❌ **Token Refresh** - Implement refresh token mechanism");
        }
        else
        {
            sb.AppendLine("- ❌ **Authentication** - Not generated, implement if needed");
            sb.AppendLine("- ❌ **Authorization** - Not generated, implement if needed");
        }
        sb.AppendLine();
        
        sb.AppendLine("### 3. Business Logic");
        sb.AppendLine("- ❌ **Complex Validations** - Add business rule validations");
        sb.AppendLine("- ❌ **Custom Queries** - Add complex queries beyond basic CRUD");
        sb.AppendLine("- ❌ **Transactions** - Implement complex transaction logic");
        sb.AppendLine("- ❌ **Business Rules** - Add domain-specific business rules");
        sb.AppendLine();
        
        sb.AppendLine("### 4. Advanced Features");
        sb.AppendLine("- ❌ **Caching** - Implement caching strategy (Redis, Memory)");
        sb.AppendLine("- ❌ **Rate Limiting** - Add API rate limiting");
        sb.AppendLine("- ❌ **API Versioning** - Implement versioning strategy");
        sb.AppendLine("- ❌ **Background Jobs** - Add background processing (Hangfire, etc.)");
        sb.AppendLine("- ❌ **Email/SMS** - Implement notification services");
        sb.AppendLine("- ❌ **File Upload** - Add file upload/download functionality");
        sb.AppendLine("- ❌ **Search** - Implement full-text search");
        sb.AppendLine("- ❌ **Pagination Optimization** - Fine-tune pagination for large datasets");
        sb.AppendLine();
        
        sb.AppendLine("### 5. Testing");
        sb.AppendLine("- ❌ **Unit Tests** - Write unit tests for business logic");
        sb.AppendLine("- ❌ **Integration Tests** - Write integration tests for API");
        sb.AppendLine("- ❌ **Test Data** - Create test data fixtures");
        sb.AppendLine("- ❌ **Mocking** - Set up mocking framework");
        sb.AppendLine();
        
        sb.AppendLine("### 6. Security");
        sb.AppendLine("- ⚠️ **HTTPS Configuration** - Configure SSL certificates for production");
        sb.AppendLine("- ❌ **Input Sanitization** - Add additional input validation");
        sb.AppendLine("- ❌ **SQL Injection Protection** - Review and test (EF Core provides basic protection)");
        sb.AppendLine("- ❌ **XSS Protection** - Implement cross-site scripting protection");
        sb.AppendLine("- ❌ **CSRF Protection** - Add CSRF tokens if needed");
        sb.AppendLine("- ❌ **Security Headers** - Add security headers middleware");
        sb.AppendLine();
        
        sb.AppendLine("### 7. Monitoring & Logging");
        sb.AppendLine("- ❌ **Application Insights** - Configure telemetry");
        sb.AppendLine("- ❌ **Error Tracking** - Set up error tracking (Sentry, etc.)");
        sb.AppendLine("- ❌ **Performance Monitoring** - Add APM tools");
        sb.AppendLine("- ❌ **Audit Logging** - Implement audit trail");
        sb.AppendLine();
        
        sb.AppendLine("### 8. Deployment");
        sb.AppendLine("- ❌ **Docker Configuration** - Create Dockerfile and docker-compose");
        sb.AppendLine("- ❌ **CI/CD Pipeline** - Set up GitHub Actions/Azure DevOps");
        sb.AppendLine("- ❌ **Environment Configuration** - Configure dev/staging/prod environments");
        sb.AppendLine("- ❌ **Database Backup** - Set up backup strategy");
        sb.AppendLine("- ❌ **Load Balancing** - Configure for high availability");
        sb.AppendLine();
        
        sb.AppendLine("### 9. Documentation");
        sb.AppendLine("- ❌ **API Documentation** - Enhance Swagger descriptions");
        sb.AppendLine("- ❌ **Architecture Docs** - Document architecture decisions");
        sb.AppendLine("- ❌ **Deployment Guide** - Create deployment documentation");
        sb.AppendLine("- ❌ **Troubleshooting Guide** - Document common issues");
        sb.AppendLine();
        
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateNextSteps(StringBuilder sb, GenerationConfig config)
    {
        sb.AppendLine("## 🎯 Recommended Next Steps");
        sb.AppendLine();
        sb.AppendLine("### Immediate (Required)");
        sb.AppendLine("1. ✅ Update connection string in `appsettings.json`");
        sb.AppendLine("2. ✅ Run `dotnet ef migrations add InitialCreate`");
        sb.AppendLine("3. ✅ Run `dotnet ef database update`");
        sb.AppendLine("4. ✅ Test the API using Swagger");
        
        if (config.GenerateAuthentication)
        {
            sb.AppendLine("5. ⚠️ Update JWT secret key in `appsettings.json`");
        }
        
        sb.AppendLine();
        
        sb.AppendLine("### Short Term (First Week)");
        sb.AppendLine("1. Implement authentication endpoints (register, login)");
        sb.AppendLine("2. Add seed data for testing");
        sb.AppendLine("3. Implement business-specific validations");
        sb.AppendLine("4. Add custom queries for complex scenarios");
        sb.AppendLine("5. Write unit tests for critical business logic");
        sb.AppendLine();
        
        sb.AppendLine("### Medium Term (First Month)");
        sb.AppendLine("1. Implement caching strategy");
        sb.AppendLine("2. Add rate limiting");
        sb.AppendLine("3. Set up CI/CD pipeline");
        sb.AppendLine("4. Configure monitoring and logging");
        sb.AppendLine("5. Write integration tests");
        sb.AppendLine("6. Create Docker configuration");
        sb.AppendLine();
        
        sb.AppendLine("### Long Term (Production Ready)");
        sb.AppendLine("1. Security audit and penetration testing");
        sb.AppendLine("2. Performance testing and optimization");
        sb.AppendLine("3. Load testing");
        sb.AppendLine("4. Complete documentation");
        sb.AppendLine("5. Disaster recovery plan");
        sb.AppendLine("6. Production deployment");
        sb.AppendLine();
        
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateTestingSection(StringBuilder sb, GenerationConfig config)
    {
        sb.AppendLine("## 🧪 Testing");
        sb.AppendLine();
        sb.AppendLine("### Manual Testing with Swagger");
        sb.AppendLine();
        sb.AppendLine("1. Run the application");
        sb.AppendLine("2. Navigate to https://localhost:5001/swagger");
        sb.AppendLine("3. Test each endpoint using the Swagger UI");
        sb.AppendLine();
        
        sb.AppendLine("### Testing with cURL");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine("# Example: Get all items");
        sb.AppendLine("curl -X GET \"https://localhost:5001/api/entityname\" -H \"accept: application/json\"");
        sb.AppendLine();
        sb.AppendLine("# Example: Create new item");
        sb.AppendLine("curl -X POST \"https://localhost:5001/api/entityname\" \\");
        sb.AppendLine("  -H \"Content-Type: application/json\" \\");
        sb.AppendLine("  -d '{\"name\":\"Test\",\"description\":\"Test Description\"}'");
        sb.AppendLine("```");
        sb.AppendLine();
        
        sb.AppendLine("### Automated Testing (To Be Implemented)");
        sb.AppendLine();
        sb.AppendLine("Create test projects:");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine($"dotnet new xunit -n {config.RootNamespace}.UnitTests");
        sb.AppendLine($"dotnet new xunit -n {config.RootNamespace}.IntegrationTests");
        sb.AppendLine("```");
        sb.AppendLine();
        
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateDeploymentSection(StringBuilder sb, GenerationConfig config)
    {
        sb.AppendLine("## 🚀 Deployment");
        sb.AppendLine();
        sb.AppendLine("### Build for Production");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine("dotnet publish -c Release -o ./publish");
        sb.AppendLine("```");
        sb.AppendLine();
        
        sb.AppendLine("### Environment Variables");
        sb.AppendLine();
        sb.AppendLine("Set these environment variables in production:");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine("ASPNETCORE_ENVIRONMENT=Production");
        sb.AppendLine("ConnectionStrings__DefaultConnection=<your-production-connection-string>");
        
        if (config.GenerateAuthentication)
        {
            sb.AppendLine("JwtSettings__SecretKey=<your-secure-secret-key>");
        }
        
        sb.AppendLine("```");
        sb.AppendLine();
        
        sb.AppendLine("### Deployment Checklist");
        sb.AppendLine();
        sb.AppendLine("- [ ] Update connection strings for production");
        sb.AppendLine("- [ ] Set secure JWT secret key");
        sb.AppendLine("- [ ] Configure HTTPS/SSL certificates");
        sb.AppendLine("- [ ] Run database migrations on production database");
        sb.AppendLine("- [ ] Set up database backups");
        sb.AppendLine("- [ ] Configure logging for production");
        sb.AppendLine("- [ ] Set up monitoring and alerts");
        sb.AppendLine("- [ ] Configure CORS for production domains");
        sb.AppendLine("- [ ] Review and test security settings");
        sb.AppendLine("- [ ] Set up CI/CD pipeline");
        sb.AppendLine();
        
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateTroubleshooting(StringBuilder sb, GenerationConfig config)
    {
        sb.AppendLine("## 🔧 Troubleshooting");
        sb.AppendLine();
        
        sb.AppendLine("### Database Connection Issues");
        sb.AppendLine();
        sb.AppendLine("**Problem:** Cannot connect to database");
        sb.AppendLine();
        sb.AppendLine("**Solutions:**");
        sb.AppendLine("- Verify connection string in `appsettings.json`");
        sb.AppendLine("- Ensure database server is running");
        sb.AppendLine("- Check firewall settings");
        sb.AppendLine("- Verify database user permissions");
        sb.AppendLine();
        
        sb.AppendLine("### Migration Issues");
        sb.AppendLine();
        sb.AppendLine("**Problem:** `dotnet ef` command not found");
        sb.AppendLine();
        sb.AppendLine("**Solution:**");
        sb.AppendLine("```bash");
        sb.AppendLine("dotnet tool install --global dotnet-ef");
        sb.AppendLine("```");
        sb.AppendLine();
        
        sb.AppendLine("**Problem:** Migration fails");
        sb.AppendLine();
        sb.AppendLine("**Solutions:**");
        sb.AppendLine("- Ensure database server is accessible");
        sb.AppendLine("- Check for naming conflicts in entities");
        sb.AppendLine("- Review migration file for errors");
        sb.AppendLine("- Drop database and recreate if in development");
        sb.AppendLine();
        
        sb.AppendLine("### Port Already in Use");
        sb.AppendLine();
        sb.AppendLine("**Problem:** Port 5001 is already in use");
        sb.AppendLine();
        sb.AppendLine("**Solution:** Change port in `Properties/launchSettings.json`");
        sb.AppendLine();
        
        sb.AppendLine("### CORS Errors");
        sb.AppendLine();
        sb.AppendLine("**Problem:** CORS policy blocking requests");
        sb.AppendLine();
        sb.AppendLine("**Solution:** Update CORS configuration in `Program.cs` to allow your frontend origin");
        sb.AppendLine();
        
        sb.AppendLine("---");
        sb.AppendLine();
    }
    
    private static void GenerateResources(StringBuilder sb)
    {
        sb.AppendLine("## 📖 Additional Resources");
        sb.AppendLine();
        sb.AppendLine("### Documentation");
        sb.AppendLine("- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)");
        sb.AppendLine("- [Entity Framework Core](https://docs.microsoft.com/ef/core)");
        sb.AppendLine("- [MediatR](https://github.com/jbogard/MediatR)");
        sb.AppendLine("- [FluentValidation](https://docs.fluentvalidation.net)");
        sb.AppendLine("- [AutoMapper](https://docs.automapper.org)");
        sb.AppendLine("- [Swagger/OpenAPI](https://swagger.io/docs/)");
        sb.AppendLine();
        
        sb.AppendLine("### Learning Resources");
        sb.AppendLine("- [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)");
        sb.AppendLine("- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)");
        sb.AppendLine("- [Repository Pattern](https://docs.microsoft.com/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application)");
        sb.AppendLine();
        
        sb.AppendLine("### Tools");
        sb.AppendLine("- [Postman](https://www.postman.com/) - API testing");
        sb.AppendLine("- [Azure Data Studio](https://docs.microsoft.com/sql/azure-data-studio) - Database management");
        sb.AppendLine("- [Docker](https://www.docker.com/) - Containerization");
        sb.AppendLine();
        
        sb.AppendLine("---");
        sb.AppendLine();
        
        sb.AppendLine("## 💡 Need Help?");
        sb.AppendLine();
        sb.AppendLine("- Check the [Troubleshooting](#-troubleshooting) section");
        sb.AppendLine("- Review the generated code comments");
        sb.AppendLine("- Consult the official documentation links above");
        sb.AppendLine();
        
        sb.AppendLine("---");
        sb.AppendLine();
        
        sb.AppendLine("**Generated by MyCodeGent** - Clean Architecture CRUD Code Generator");
        sb.AppendLine();
        sb.AppendLine($"*Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}*");
    }
}
