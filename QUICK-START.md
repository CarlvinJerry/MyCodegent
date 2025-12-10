# MyCodeGent - Quick Start Guide

## 🚀 Get Started in 3 Steps

### Step 1: Start the Application

```bash
cd MyCodeGent.Web
dotnet run
```

Application will be available at:
- **Web UI**: https://localhost:44330/
- **Swagger**: https://localhost:44330/swagger
- **API**: https://localhost:44330/api/codegen

### Step 2: Choose Your Workflow

#### Option A: Generate New Project

```powershell
# Get sample configuration
$sample = Invoke-RestMethod -Uri "https://localhost:44330/api/codegen/sample-config" -SkipCertificateCheck

# Generate code
$response = Invoke-RestMethod -Uri "https://localhost:44330/api/codegen/generate" `
    -Method Post `
    -Body ($sample | ConvertTo-Json -Depth 10) `
    -ContentType "application/json" `
    -SkipCertificateCheck

# Download ZIP
$downloadUrl = "https://localhost:44330$($response.downloadUrl)"
Invoke-WebRequest -Uri $downloadUrl -OutFile "generated.zip" -SkipCertificateCheck
```

#### Option B: Add to Existing Project

```powershell
$request = @{
    projectPath = "C:/MyProject/Generated"
    config = @{
        rootNamespace = "MyApp"
        generateApi = $true
        generateApplication = $true
        generateDomain = $true
        generateInfrastructure = $true
    }
    newEntities = @(
        @{
            name = "Customer"
            hasAuditFields = $true
            properties = @(
                @{ name = "Id"; type = "int"; isKey = $true; isRequired = $true }
                @{ name = "Name"; type = "string"; isRequired = $true; maxLength = 200 }
            )
        }
    )
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "https://localhost:44330/api/codegen/generate-incremental" `
    -Method Post `
    -Body $request `
    -ContentType "application/json" `
    -SkipCertificateCheck
```

### Step 3: Use Generated Code

The generator creates:
- ✅ Domain entities
- ✅ DTOs
- ✅ CQRS Commands & Queries
- ✅ MediatR Handlers
- ✅ FluentValidation Validators
- ✅ EF Core Configurations
- ✅ API Controllers

## 📚 Documentation

- **Full Guide**: [README.md](README.md)
- **Web Version**: [WEB-README.md](WEB-README.md)
- **Incremental Updates**: [INCREMENTAL-GENERATION-GUIDE.md](INCREMENTAL-GENERATION-GUIDE.md)
- **Deployment**: [DEPLOYMENT-GUIDE.md](DEPLOYMENT-GUIDE.md)
- **Troubleshooting**: [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

## 🎯 Key Features

| Feature | Endpoint |
|---------|----------|
| Generate New Project | `POST /api/codegen/generate` |
| Add to Existing | `POST /api/codegen/generate-incremental` |
| Preview Code | `POST /api/codegen/preview` |
| Sample Config | `GET /api/codegen/sample-config` |
| Health Check | `GET /api/codegen/health` |

## 💡 Tips

1. **Use Swagger UI** for interactive testing
2. **Start with sample config** to understand the structure
3. **Use incremental generation** to add entities over time
4. **Backup before incremental updates**
5. **Review generated code** before using in production

---

**Ready to generate some code? 🎉**
