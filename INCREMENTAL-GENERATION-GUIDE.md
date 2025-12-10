# Incremental Code Generation Guide

## 🎯 Overview

The incremental generation feature allows you to add new entities to an **existing project** without overwriting files you've already customized. This is perfect for:

- Adding new entities to an existing codebase
- Extending your application over time
- Preserving manual changes to generated files

## 🔄 How It Works

### What Gets Created (New Files)
- ✅ New entity classes
- ✅ New DTOs
- ✅ New Commands (Create, Update, Delete)
- ✅ New Queries (GetById, GetAll)
- ✅ New Command/Query Handlers
- ✅ New Validators
- ✅ New Entity Configurations
- ✅ New API Controllers

### What Gets Updated (Existing Files)
- 🔄 `IApplicationDbContext.cs` - Adds DbSet for new entities
- 🔄 `ApplicationDbContext.cs` - Adds DbSet and configurations for new entities

### What Gets Skipped
- ⏭️ Existing entity files (won't overwrite)
- ⏭️ Existing controllers
- ⏭️ Existing commands/queries
- ⏭️ Any file that already exists

## 📡 API Endpoint

### POST `/api/codegen/generate-incremental`

Add new entities to an existing project.

**Request Body:**
```json
{
  "projectPath": "C:/MyProjects/MyApp/Generated",
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
  "newEntities": [
    {
      "name": "Customer",
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
          "name": "Email",
          "type": "string",
          "isRequired": true,
          "maxLength": 100
        }
      ]
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "entitiesAdded": ["Customer"],
  "newFilesCount": 15,
  "updatedFilesCount": 2,
  "newFiles": [
    "Domain/Entities/Customer.cs",
    "Application/Customers/CustomerDto.cs",
    "Application/Customers/Commands/CreateCustomer/CreateCustomerCommand.cs",
    "..."
  ],
  "updatedFiles": [
    "Application/Common/Interfaces/IApplicationDbContext.cs",
    "Infrastructure/Persistence/ApplicationDbContext.cs"
  ],
  "message": "Successfully added 1 new entities to existing project"
}
```

## 🧪 Usage Examples

### Example 1: Using PowerShell

```powershell
# Step 1: Generate initial project with Product entity
$initialRequest = @{
    config = @{
        rootNamespace = "MyShop"
        generateApi = $true
        generateApplication = $true
        generateDomain = $true
        generateInfrastructure = $true
        useMediator = $true
        useFluentValidation = $true
        databaseProvider = "SqlServer"
    }
    entities = @(
        @{
            name = "Product"
            hasAuditFields = $true
            hasSoftDelete = $true
            properties = @(
                @{ name = "Id"; type = "int"; isKey = $true; isRequired = $true }
                @{ name = "Name"; type = "string"; isRequired = $true; maxLength = 200 }
                @{ name = "Price"; type = "decimal"; isRequired = $true }
            )
        }
    )
} | ConvertTo-Json -Depth 10

$response = Invoke-RestMethod -Uri "https://localhost:44330/api/codegen/generate" `
    -Method Post `
    -Body $initialRequest `
    -ContentType "application/json" `
    -SkipCertificateCheck

Write-Host "✅ Initial project generated. Session: $($response.sessionId)"

# Step 2: Download and extract to your project location
$downloadUrl = "https://localhost:44330$($response.downloadUrl)"
Invoke-WebRequest -Uri $downloadUrl -OutFile "generated.zip" -SkipCertificateCheck
Expand-Archive -Path "generated.zip" -DestinationPath "C:/MyProjects/MyShop" -Force

# Step 3: Add Customer entity to existing project
$incrementalRequest = @{
    projectPath = "C:/MyProjects/MyShop"
    config = @{
        rootNamespace = "MyShop"
        generateApi = $true
        generateApplication = $true
        generateDomain = $true
        generateInfrastructure = $true
        useMediator = $true
        useFluentValidation = $true
        databaseProvider = "SqlServer"
    }
    newEntities = @(
        @{
            name = "Customer"
            hasAuditFields = $true
            hasSoftDelete = $true
            properties = @(
                @{ name = "Id"; type = "int"; isKey = $true; isRequired = $true }
                @{ name = "Name"; type = "string"; isRequired = $true; maxLength = 200 }
                @{ name = "Email"; type = "string"; isRequired = $true; maxLength = 100 }
            )
        }
    )
} | ConvertTo-Json -Depth 10

$incrementalResponse = Invoke-RestMethod -Uri "https://localhost:44330/api/codegen/generate-incremental" `
    -Method Post `
    -Body $incrementalRequest `
    -ContentType "application/json" `
    -SkipCertificateCheck

Write-Host "✅ $($incrementalResponse.message)" -ForegroundColor Green
Write-Host "   New files: $($incrementalResponse.newFilesCount)" -ForegroundColor Cyan
Write-Host "   Updated files: $($incrementalResponse.updatedFilesCount)" -ForegroundColor Yellow
Write-Host "   Entities added: $($incrementalResponse.entitiesAdded -join ', ')" -ForegroundColor Magenta
```

### Example 2: Using cURL

```bash
# Add Order entity to existing project
curl -X POST https://localhost:44330/api/codegen/generate-incremental \
  -H "Content-Type: application/json" \
  -d '{
    "projectPath": "/path/to/existing/project",
    "config": {
      "rootNamespace": "MyApp",
      "generateApi": true,
      "generateApplication": true,
      "generateDomain": true,
      "generateInfrastructure": true
    },
    "newEntities": [
      {
        "name": "Order",
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
            "name": "OrderNumber",
            "type": "string",
            "isRequired": true,
            "maxLength": 50
          },
          {
            "name": "TotalAmount",
            "type": "decimal",
            "isRequired": true
          }
        ]
      }
    ]
  }'
```

### Example 3: Using Swagger UI

1. Navigate to `https://localhost:44330/swagger`
2. Find the `/api/codegen/generate-incremental` endpoint
3. Click "Try it out"
4. Fill in the request body:
   - Set `projectPath` to your existing project directory
   - Configure your entities
5. Click "Execute"
6. Review the response showing what was created/updated

## 🎯 Workflow

### Initial Project Setup

1. **Generate Initial Project**
   ```
   POST /api/codegen/generate
   ```
   - Creates a complete project structure
   - Download as ZIP
   - Extract to your working directory

2. **Customize Generated Code**
   - Modify business logic
   - Add custom methods
   - Adjust validation rules
   - Customize controllers

3. **Add New Entities Incrementally**
   ```
   POST /api/codegen/generate-incremental
   ```
   - Point to your existing project directory
   - Specify only the NEW entities
   - Existing files are preserved
   - Common files are updated to include new entities

### Continuous Development

```
Initial Generation → Customize → Add Entity 1 → Customize → Add Entity 2 → ...
```

Each time you add entities:
- ✅ New entity files are created
- ✅ DbContext is updated with new DbSets
- ✅ Your customizations are preserved
- ✅ No existing files are overwritten

## ⚠️ Important Notes

### 1. Project Path Must Exist
The `projectPath` must point to an existing directory with the expected structure:
```
ProjectPath/
├── Domain/
│   └── Entities/
├── Application/
│   └── Common/
│       └── Interfaces/
├── Infrastructure/
│   └── Persistence/
└── Api/
    └── Controllers/
```

### 2. Namespace Consistency
Ensure the `rootNamespace` matches your existing project's namespace.

### 3. Backup Before Running
Always backup your project before running incremental generation, especially the first time.

### 4. DbContext Updates
The `ApplicationDbContext.cs` and `IApplicationDbContext.cs` files will be **completely regenerated** to include all entities (existing + new). If you've made custom changes to these files, you'll need to re-apply them.

### 5. Entity Name Conflicts
If you try to add an entity that already exists, the generator will skip it and log a warning.

## 🔍 What Happens Behind the Scenes

1. **Scan Existing Project**
   - Reads existing entity files from `Domain/Entities/`
   - Builds a list of existing entity names

2. **Generate New Entity Files**
   - Creates files for each new entity
   - Skips if file already exists
   - Logs skipped files

3. **Update Common Files**
   - Regenerates `IApplicationDbContext.cs` with all DbSets
   - Regenerates `ApplicationDbContext.cs` with all configurations
   - Includes both existing and new entities

4. **Return Results**
   - Lists all new files created
   - Lists all files updated
   - Lists entities successfully added

## 📊 Comparison: Regular vs Incremental

| Feature | Regular Generation | Incremental Generation |
|---------|-------------------|------------------------|
| **Use Case** | New project | Existing project |
| **Output** | Temp directory + ZIP | Directly to project |
| **Existing Files** | N/A | Preserved |
| **DbContext** | Created fresh | Updated with new entities |
| **Customizations** | N/A | Preserved |
| **Download** | Required | Not needed |

## 🚀 Best Practices

### 1. Version Control
Always commit your changes before running incremental generation:
```bash
git add .
git commit -m "Before adding Customer entity"
```

### 2. Review Changes
After generation, review the updated files:
```bash
git diff
```

### 3. Test Incrementally
Add one or two entities at a time, test, then add more.

### 4. Document Custom Changes
Keep track of customizations you make to generated files so you can re-apply them if needed.

### 5. Use Partial Classes
Consider using partial classes for entities to separate generated code from custom code:
```csharp
// Product.cs (generated)
public partial class Product { ... }

// Product.Custom.cs (your customizations)
public partial class Product 
{
    public decimal CalculateDiscount() { ... }
}
```

## 🐛 Troubleshooting

### Error: "Project path not found"
**Solution:** Ensure the path exists and is accessible. Use absolute paths.

### Error: "Entity must have at least one property"
**Solution:** Each entity must have at least one property defined.

### Warning: "Skipping existing entity"
**Info:** This is normal. The entity already exists and won't be overwritten.

### DbContext doesn't compile after update
**Solution:** Check that all entity configurations are properly imported. You may need to add using statements.

## 📚 Related Documentation

- **Regular Generation**: See [WEB-README.md](WEB-README.md)
- **Deployment**: See [DEPLOYMENT-GUIDE.md](DEPLOYMENT-GUIDE.md)
- **Troubleshooting**: See [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

---

**Happy Incremental Coding! 🎉**
