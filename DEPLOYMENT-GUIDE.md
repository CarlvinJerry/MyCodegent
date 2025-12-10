# MyCodeGent - Deployment & Usage Guide

## ✅ Application Status

**FULLY FUNCTIONAL** - All issues resolved!

### What Was Fixed:
1. ✅ **Swagger/OpenAPI** - Enabled with Swashbuckle.AspNetCore
2. ✅ **Static Files** - Proper middleware ordering
3. ✅ **Global Exception Handler** - Comprehensive error handling
4. ✅ **Build Warnings** - Resolved async/await warning
5. ✅ **CORS** - Configured for development
6. ✅ **Logging** - Enhanced startup logging

---

## 🚀 Quick Start

### 1. Run the Application

```bash
cd MyCodeGent.Web
dotnet run
```

### 2. Access the Application

The application will start on:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001

### 3. Available Endpoints

- **Web UI:** https://localhost:5001/
- **Swagger UI:** https://localhost:5001/swagger
- **API Base:** https://localhost:5001/api/codegen

---

## 📡 API Endpoints

### 1. Generate Code
**POST** `/api/codegen/generate`

Generate complete CRUD code for multiple entities.

**Example Request:**
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
        },
        {
          "name": "Price",
          "type": "decimal",
          "isRequired": true
        }
      ]
    }
  ]
}
```

**Example Response:**
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

### 4. Get Sample Configuration
**GET** `/api/codegen/sample-config`

Get a sample configuration with example entities.

---

## 🧪 Testing the API

### Using PowerShell

```powershell
# Get sample config
Invoke-RestMethod -Uri "https://localhost:5001/api/codegen/sample-config" -Method Get

# Generate code
$body = @{
    config = @{
        rootNamespace = "TestApp"
        generateApi = $true
        generateApplication = $true
        generateDomain = $true
        generateInfrastructure = $true
    }
    entities = @(
        @{
            name = "Customer"
            hasAuditFields = $true
            hasSoftDelete = $true
            properties = @(
                @{
                    name = "Id"
                    type = "int"
                    isKey = $true
                    isRequired = $true
                },
                @{
                    name = "Name"
                    type = "string"
                    isRequired = $true
                    maxLength = 100
                }
            )
        }
    )
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "https://localhost:5001/api/codegen/generate" -Method Post -Body $body -ContentType "application/json"
```

### Using cURL

```bash
# Get sample config
curl https://localhost:5001/api/codegen/sample-config

# Generate code
curl -X POST https://localhost:5001/api/codegen/generate \
  -H "Content-Type: application/json" \
  -d @config.json
```

---

## 🐛 Troubleshooting

### Issue: Port Already in Use

**Solution:**
```bash
dotnet run --urls "https://localhost:5002"
```

Or edit `appsettings.json`:
```json
{
  "Urls": "https://localhost:5002"
}
```

### Issue: HTTPS Certificate Error

**Solution:**
```bash
dotnet dev-certs https --trust
```

### Issue: Swagger Not Loading

**Solution:**
- Ensure you're accessing `/swagger` (not `/swagger/index.html`)
- Check that Swashbuckle.AspNetCore package is installed
- Verify the application is running

### Issue: Static Files Not Serving

**Solution:**
- Check that `wwwroot` folder exists
- Verify `UseDefaultFiles()` comes before `UseStaticFiles()`
- Ensure files are marked as "Content" in the project

---

## 📦 Deployment

### IIS Deployment

1. **Publish the application:**
```bash
dotnet publish -c Release -o ./publish
```

2. **Install .NET Hosting Bundle** on the server

3. **Create IIS Site:**
   - Point to the `publish` folder
   - Set Application Pool to "No Managed Code"
   - Enable HTTPS

### Docker Deployment

Create `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

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
docker run -p 8080:80 -p 8443:443 mycodegent-web
```

### Azure App Service

```bash
# Login to Azure
az login

# Create resource group
az group create --name mycodegent-rg --location eastus

# Create App Service plan
az appservice plan create --name mycodegent-plan --resource-group mycodegent-rg --sku B1 --is-linux

# Create web app
az webapp create --name mycodegent-app --resource-group mycodegent-rg --plan mycodegent-plan --runtime "DOTNETCORE:9.0"

# Deploy
dotnet publish -c Release
cd bin/Release/net9.0/publish
Compress-Archive -Path * -DestinationPath deploy.zip
az webapp deployment source config-zip --resource-group mycodegent-rg --name mycodegent-app --src deploy.zip
```

---

## 🔐 Production Considerations

### 1. Update CORS Policy

In `Program.cs`, restrict CORS to specific origins:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### 2. Add Authentication

Install package:
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

Configure in `Program.cs`:
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* configure */ });

// Add [Authorize] to controllers
```

### 3. Add Rate Limiting

Install package:
```bash
dotnet add package AspNetCoreRateLimit
```

### 4. Configure Logging

Update `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 5. Add Health Checks

```csharp
builder.Services.AddHealthChecks();
app.MapHealthChecks("/health");
```

---

## 📊 Monitoring

### Application Insights (Azure)

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

In `Program.cs`:
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Serilog

```bash
dotnet add package Serilog.AspNetCore
```

---

## 🎯 Performance Tips

1. **Enable Response Compression**
```csharp
builder.Services.AddResponseCompression();
app.UseResponseCompression();
```

2. **Add Response Caching**
```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

3. **Configure Kestrel Limits**
```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
});
```

---

## ✅ Verification Checklist

- [ ] Application builds without errors
- [ ] Application starts successfully
- [ ] Swagger UI is accessible at `/swagger`
- [ ] Web UI is accessible at `/`
- [ ] API endpoints respond correctly
- [ ] File generation works
- [ ] ZIP download works
- [ ] Preview functionality works
- [ ] Error handling works properly
- [ ] Logs are being written

---

## 📞 Support

For issues or questions:
1. Check the logs in the console
2. Visit Swagger UI for API documentation
3. Review this deployment guide
4. Check the main README.md files

---

**Application Status: ✅ READY FOR USE**

*Last Updated: December 2024*
