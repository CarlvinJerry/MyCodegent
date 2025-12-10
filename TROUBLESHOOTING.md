# MyCodeGent - Troubleshooting Guide

## 🔍 "Error generation failed" Issue

If you see "Error generation failed" in the logs, follow these steps:

### Step 1: Check the Detailed Error
The application now logs detailed error information. Look for:
- Exception Type
- Error Message
- Stack Trace
- Inner Exception Details

### Step 2: Verify API Health
Test the health endpoint:
```bash
# PowerShell
Invoke-RestMethod -Uri "https://localhost:5001/api/codegen/health"

# Or open in browser
https://localhost:5001/api/codegen/health
```

Expected response:
```json
{
  "status": "healthy",
  "timestamp": "2024-12-10T...",
  "version": "1.0.0",
  "services": {
    "codeGenerator": "OK",
    "fileWriter": "OK"
  }
}
```

### Step 3: Test with Sample Config
Get and test the sample configuration:

```powershell
# Get sample config
$sample = Invoke-RestMethod -Uri "https://localhost:5001/api/codegen/sample-config"

# Generate code with sample
$response = Invoke-RestMethod -Uri "https://localhost:5001/api/codegen/generate" `
    -Method Post `
    -Body ($sample | ConvertTo-Json -Depth 10) `
    -ContentType "application/json"

Write-Host "Generated $($response.filesGenerated) files"
Write-Host "Session ID: $($response.sessionId)"
```

### Step 4: Check Swagger UI
Open Swagger UI to test endpoints interactively:
```
https://localhost:5001/swagger
```

### Step 5: Common Issues and Solutions

#### Issue: "Request body is required"
**Cause:** Empty or malformed JSON request
**Solution:** Ensure you're sending valid JSON with proper Content-Type header

```powershell
# Correct way
$headers = @{
    "Content-Type" = "application/json"
}
$body = @{
    config = @{ rootNamespace = "MyApp" }
    entities = @(...)
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $body
```

#### Issue: "No entities provided"
**Cause:** Empty entities array
**Solution:** Provide at least one entity with properties

```json
{
  "config": { "rootNamespace": "MyApp" },
  "entities": [
    {
      "name": "Product",
      "properties": [
        { "name": "Id", "type": "int", "isKey": true }
      ]
    }
  ]
}
```

#### Issue: "Entity must have at least one property"
**Cause:** Entity has no properties defined
**Solution:** Add at least one property to each entity

#### Issue: File system permissions error
**Cause:** Cannot write to temp directory
**Solution:** 
1. Check temp directory permissions: `$env:TEMP`
2. Ensure the application has write access
3. Try running as administrator (not recommended for production)

#### Issue: Template assembly reflection errors
**Cause:** Templates assembly being scanned by API
**Solution:** Already fixed in Program.cs - Templates assembly is excluded from API scanning

### Step 6: Enable Detailed Logging

The application now logs at Debug level in Development mode. Check the console output for:

```
[DBG] Converting to template models...
[INF] Generating code for entity: Product
[DBG] Generating common files...
[DBG] Collecting generated files from: C:\Users\...\Temp\MyCodeGent\...
[INF] Code generation completed successfully. Generated 42 files
```

### Step 7: Test Individual Endpoints

#### Test Preview (No File Generation)
```powershell
$preview = @{
    config = @{ rootNamespace = "TestApp" }
    entity = @{
        name = "Product"
        properties = @(
            @{ name = "Id"; type = "int"; isKey = $true }
            @{ name = "Name"; type = "string"; isRequired = $true }
        )
    }
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "https://localhost:5001/api/codegen/preview" `
    -Method Post `
    -Body $preview `
    -ContentType "application/json"
```

This tests code generation without file I/O.

### Step 8: Check Output Directory
After successful generation, check the temp directory:

```powershell
# Get temp path
$tempPath = [System.IO.Path]::GetTempPath()
$codegenPath = Join-Path $tempPath "MyCodeGent"

# List sessions
Get-ChildItem $codegenPath

# View generated files for a session
Get-ChildItem "$codegenPath\<session-id>" -Recurse
```

## 🔧 Quick Fixes

### Reset the Application
```bash
# Stop the application (Ctrl+C)
# Clear temp files
Remove-Item "$env:TEMP\MyCodeGent" -Recurse -Force -ErrorAction SilentlyContinue

# Rebuild
dotnet clean
dotnet build

# Run again
dotnet run
```

### Verify Dependencies
```bash
# Check all packages are restored
dotnet restore

# List packages
dotnet list package
```

Expected packages:
- Microsoft.AspNetCore.OpenApi (9.0.9)
- Swashbuckle.AspNetCore (7.2.0)

### Check Project References
```bash
# Verify project references
dotnet list reference
```

Should show:
- MyCodeGent.Core
- MyCodeGent.Templates (indirectly through Core)

## 📊 Diagnostic Endpoints

### Health Check
```
GET /api/codegen/health
```
Returns service status and dependency checks

### Sample Config
```
GET /api/codegen/sample-config
```
Returns a working sample configuration

### Preview (Safe Test)
```
POST /api/codegen/preview
```
Tests code generation without file I/O

## 🐛 Still Having Issues?

### Collect Diagnostic Information

1. **Application Logs** - Copy all console output
2. **Request Details** - Save the JSON you're sending
3. **Response Details** - Save the error response
4. **Environment Info**:
   ```powershell
   dotnet --info
   $PSVersionTable
   [Environment]::OSVersion
   ```

### Enable Maximum Logging

Edit `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "Microsoft": "Debug",
      "MyCodeGent": "Trace"
    }
  }
}
```

### Test with Minimal Entity

Use the simplest possible entity:
```json
{
  "config": {
    "rootNamespace": "Test",
    "generateApi": true,
    "generateApplication": true,
    "generateDomain": true,
    "generateInfrastructure": true
  },
  "entities": [
    {
      "name": "Item",
      "hasAuditFields": false,
      "hasSoftDelete": false,
      "properties": [
        {
          "name": "Id",
          "type": "int",
          "isKey": true,
          "isRequired": true
        }
      ]
    }
  ]
}
```

## ✅ Verification Steps

After fixing issues, verify:

1. ✅ Application starts without errors
2. ✅ Swagger UI loads at `/swagger`
3. ✅ Health endpoint returns "healthy"
4. ✅ Sample config endpoint works
5. ✅ Preview endpoint generates code
6. ✅ Generate endpoint creates files
7. ✅ Download endpoint returns ZIP

## 📞 Additional Resources

- **Swagger UI**: https://localhost:5001/swagger
- **Main README**: See README.md
- **Web README**: See WEB-README.md
- **Deployment Guide**: See DEPLOYMENT-GUIDE.md

---

**Last Updated**: December 2024
