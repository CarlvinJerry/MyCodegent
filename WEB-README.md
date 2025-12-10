# MyCodeGent Web - Code Generator with UI

The web version of MyCodeGent provides a REST API and beautiful web interface for generating CRUD code.

## 🚀 Quick Start

### Run the Web Application

```bash
cd MyCodeGent.Web
dotnet run
```

The application will start on:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001
- **Swagger UI:** https://localhost:5001/swagger

### Access the UI

Open your browser and navigate to:
```
https://localhost:5001
```

## 📡 API Endpoints

### 1. Generate Code

**POST** `/api/codegen/generate`

Generate complete CRUD code for multiple entities.

**Request Body:**
```json
{
  "config": {
    "rootNamespace": "MyApp",
    "generateApi": true,
    "generateApplication": true,
    "generateDomain": true,
    "generateInfrastructure": true,
    "useMediator": true,
    "useFluentValidation": true,
    "databaseProvider": "SqlServer"
  },
  "entities": [
    {
      "name": "Product",
      "hasAuditFields": true,
      "hasSoftDelete": true,
      "properties": [
        {
          "name": "Id",
          "type": "int",
          "isKey": true,
          "isRequired": true
        },
        {
          "name": "Name",
          "type": "string",
          "isRequired": true,
          "maxLength": 200
        }
      ]
    }
  ]
}
```

**Response:**
```json
{
  "sessionId": "abc123-def456",
  "filesGenerated": 42,
  "files": [...],
  "downloadUrl": "/api/codegen/download/abc123-def456"
}
```

### 2. Download Generated Code

**GET** `/api/codegen/download/{sessionId}`

Download all generated files as a ZIP archive.

### 3. Preview Code

**POST** `/api/codegen/preview`

Preview generated code for a single entity without creating files.

**Request Body:**
```json
{
  "config": {
    "rootNamespace": "MyApp",
    "generateApi": true,
    "generateApplication": true,
    "generateDomain": true,
    "generateInfrastructure": true
  },
  "entity": {
    "name": "Product",
    "hasAuditFields": true,
    "hasSoftDelete": true,
    "properties": [...]
  }
}
```

**Response:**
```json
{
  "previews": {
    "Domain/Entity": "public class Product { ... }",
    "Application/Dto": "public class ProductDto { ... }",
    "Application/CreateCommand": "public record CreateProductCommand ...",
    "Api/Controller": "public class ProductsController ..."
  }
}
```

### 4. Get Sample Configuration

**GET** `/api/codegen/sample-config`

Get a sample configuration with example entities.

## 🎨 Web UI Features

### Entity Builder Tab
- **Visual entity designer** - Build entities with a drag-and-drop interface
- **Property management** - Add/remove properties with type selection
- **Configuration options** - Toggle audit fields, soft delete, and generation layers
- **Real-time validation** - Instant feedback on your entity design

### JSON Config Tab
- **Direct JSON editing** - For advanced users who prefer JSON
- **Load sample** - Quick start with pre-configured examples
- **Syntax highlighting** - Easy-to-read JSON formatting

### Preview Tab
- **Live code preview** - See generated code before downloading
- **Multi-layer view** - Preview Domain, Application, Infrastructure, and API layers
- **Syntax highlighting** - Color-coded C# code display

## 🔧 Configuration Options

| Option | Type | Description |
|--------|------|-------------|
| `rootNamespace` | string | Root namespace for generated code |
| `generateApi` | bool | Generate API controllers |
| `generateApplication` | bool | Generate CQRS commands/queries |
| `generateDomain` | bool | Generate domain entities |
| `generateInfrastructure` | bool | Generate EF Core configurations |
| `useMediator` | bool | Use MediatR pattern |
| `useFluentValidation` | bool | Generate validators |
| `databaseProvider` | enum | SqlServer, PostgreSql, MySql, Sqlite |

## 📦 Generated Output Structure

```
Generated/
├── Domain/
│   └── Entities/
│       └── Product.cs
├── Application/
│   ├── Products/
│   │   ├── ProductDto.cs
│   │   ├── Commands/
│   │   │   ├── CreateProduct/
│   │   │   ├── UpdateProduct/
│   │   │   └── DeleteProduct/
│   │   └── Queries/
│   │       ├── GetProductById/
│   │       └── GetAllProducts/
│   └── Common/
│       ├── Interfaces/
│       └── Models/
├── Infrastructure/
│   └── Persistence/
│       ├── ApplicationDbContext.cs
│       └── Configurations/
└── Api/
    └── Controllers/
        └── ProductsController.cs
```

## 🌐 CORS Configuration

The API is configured with CORS to allow requests from any origin during development:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

**⚠️ Production Note:** Update CORS policy to restrict origins in production.

## 🔐 Security Considerations

### For Production Deployment:

1. **Update CORS Policy**
   ```csharp
   policy.WithOrigins("https://yourdomain.com")
   ```

2. **Add Authentication**
   - Implement JWT or OAuth2
   - Protect endpoints with `[Authorize]`

3. **Rate Limiting**
   - Add rate limiting middleware
   - Prevent abuse of generation endpoints

4. **File Size Limits**
   - Configure max request body size
   - Limit number of entities per request

5. **Temporary File Cleanup**
   - Implement background service to clean old sessions
   - Set expiration time for generated files

## 🚀 Deployment

### Docker

Create a `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyCodeGent.Web/MyCodeGent.Web.csproj", "MyCodeGent.Web/"]
COPY ["MyCodeGent.Core/MyCodeGent.Core.csproj", "MyCodeGent.Core/"]
COPY ["MyCodeGent.Templates/MyCodeGent.Templates.csproj", "MyCodeGent.Templates/"]
RUN dotnet restore "MyCodeGent.Web/MyCodeGent.Web.csproj"
COPY . .
WORKDIR "/src/MyCodeGent.Web"
RUN dotnet build "MyCodeGent.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyCodeGent.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyCodeGent.Web.dll"]
```

Build and run:
```bash
docker build -t mycodegent-web .
docker run -p 8080:80 mycodegent-web
```

### Azure App Service

```bash
# Login to Azure
az login

# Create resource group
az group create --name mycodegent-rg --location eastus

# Create App Service plan
az appservice plan create --name mycodegent-plan --resource-group mycodegent-rg --sku B1

# Create web app
az webapp create --name mycodegent-app --resource-group mycodegent-rg --plan mycodegent-plan

# Deploy
dotnet publish -c Release
cd bin/Release/net9.0/publish
zip -r deploy.zip .
az webapp deployment source config-zip --resource-group mycodegent-rg --name mycodegent-app --src deploy.zip
```

## 📊 API Usage Examples

### Using cURL

```bash
# Generate code
curl -X POST https://localhost:5001/api/codegen/generate \
  -H "Content-Type: application/json" \
  -d @config.json

# Download generated code
curl -O https://localhost:5001/api/codegen/download/session-id

# Get sample config
curl https://localhost:5001/api/codegen/sample-config
```

### Using JavaScript/Fetch

```javascript
// Generate code
const response = await fetch('https://localhost:5001/api/codegen/generate', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    config: { rootNamespace: 'MyApp', ... },
    entities: [...]
  })
});

const result = await response.json();
console.log(`Generated ${result.filesGenerated} files`);

// Download
window.open(result.downloadUrl, '_blank');
```

### Using C#

```csharp
using var client = new HttpClient();
var request = new GenerateRequest
{
    Config = new GenerationConfig { RootNamespace = "MyApp" },
    Entities = new List<EntityModel> { ... }
};

var response = await client.PostAsJsonAsync(
    "https://localhost:5001/api/codegen/generate", 
    request
);

var result = await response.Content.ReadFromJsonAsync<GenerateResponse>();
```

## 🎯 Future Enhancements

- [ ] **User Authentication** - Save and manage projects
- [ ] **Project Templates** - Pre-configured entity sets
- [ ] **Code Customization** - Template editing in UI
- [ ] **Version Control** - Track generated code versions
- [ ] **Collaboration** - Share projects with team
- [ ] **Export Options** - GitHub, GitLab integration
- [ ] **Real-time Collaboration** - Multiple users editing
- [ ] **AI Suggestions** - Smart entity/property recommendations

## 🐛 Troubleshooting

### Port Already in Use

```bash
# Change port in appsettings.json or use:
dotnet run --urls "https://localhost:5002"
```

### CORS Errors

Ensure CORS is properly configured in `Program.cs` and the policy is applied before other middleware.

### File Download Issues

Check that the temp directory has write permissions and sufficient disk space.

## 📝 License

MIT License - Same as the main MyCodeGent project.

---

**Happy Coding! 🎉**

For CLI version, see the main [README.md](README.md)
