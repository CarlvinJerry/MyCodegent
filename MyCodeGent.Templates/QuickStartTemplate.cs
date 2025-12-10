using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class QuickStartTemplate
{
    public static string GenerateVisualStudioGuide(GenerationConfig config)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("# 🚀 Quick Start Guide - Visual Studio");
        sb.AppendLine();
        sb.AppendLine($"## Running {config.RootNamespace} in Visual Studio 2022");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        
        // Step 1: Open Solution
        sb.AppendLine("## Step 1: Open the Solution");
        sb.AppendLine();
        sb.AppendLine("1. **Locate the solution file:**");
        sb.AppendLine($"   - Find `{config.RootNamespace}.sln` in the generated folder");
        sb.AppendLine();
        sb.AppendLine("2. **Open in Visual Studio:**");
        sb.AppendLine("   - Double-click the `.sln` file, OR");
        sb.AppendLine("   - Open Visual Studio → File → Open → Project/Solution");
        sb.AppendLine("   - Navigate to the folder and select the `.sln` file");
        sb.AppendLine();
        sb.AppendLine("3. **Wait for NuGet restore:**");
        sb.AppendLine("   - Visual Studio will automatically restore NuGet packages");
        sb.AppendLine("   - Check the status bar at the bottom for \"Restore complete\"");
        sb.AppendLine();
        
        // Step 2: Update Connection String
        sb.AppendLine("## Step 2: Configure Database Connection");
        sb.AppendLine();
        sb.AppendLine($"1. **Open `appsettings.json`** in the `{config.RootNamespace}.Api` project");
        sb.AppendLine();
        sb.AppendLine("2. **Update the connection string:**");
        sb.AppendLine();
        sb.AppendLine("```json");
        sb.AppendLine("{");
        sb.AppendLine("  \"ConnectionStrings\": {");
        
        switch (config.DatabaseProvider)
        {
            case DatabaseProvider.SqlServer:
                sb.AppendLine("    // For SQL Server (LocalDB):");
                sb.AppendLine($"    \"DefaultConnection\": \"Server=(localdb)\\\\mssqllocaldb;Database={config.RootNamespace}Db;Trusted_Connection=true;\"");
                sb.AppendLine();
                sb.AppendLine("    // OR for SQL Server Express:");
                sb.AppendLine($"    // \"DefaultConnection\": \"Server=localhost\\\\SQLEXPRESS;Database={config.RootNamespace}Db;Trusted_Connection=true;TrustServerCertificate=true;\"");
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
        }
        
        sb.AppendLine("  }");
        sb.AppendLine("}");
        sb.AppendLine("```");
        sb.AppendLine();
        
        // Step 3: Create Database
        if (config.DatabaseProvider != DatabaseProvider.InMemory)
        {
            sb.AppendLine("## Step 3: Create the Database");
            sb.AppendLine();
            sb.AppendLine("### Option A: Using Package Manager Console (Recommended)");
            sb.AppendLine();
            sb.AppendLine("1. **Open Package Manager Console:**");
            sb.AppendLine("   - Go to: `Tools` → `NuGet Package Manager` → `Package Manager Console`");
            sb.AppendLine();
            sb.AppendLine("2. **Set the default project:**");
            sb.AppendLine($"   - In the console dropdown, select `{config.RootNamespace}.Infrastructure`");
            sb.AppendLine();
            sb.AppendLine("3. **Create initial migration:**");
            sb.AppendLine("   ```powershell");
            sb.AppendLine("   Add-Migration InitialCreate");
            sb.AppendLine("   ```");
            sb.AppendLine();
            sb.AppendLine("4. **Update the database:**");
            sb.AppendLine("   ```powershell");
            sb.AppendLine("   Update-Database");
            sb.AppendLine("   ```");
            sb.AppendLine();
            sb.AppendLine("### Option B: Using .NET CLI");
            sb.AppendLine();
            sb.AppendLine("1. **Open Terminal in Visual Studio:**");
            sb.AppendLine("   - Go to: `View` → `Terminal`");
            sb.AppendLine();
            sb.AppendLine("2. **Navigate to API project:**");
            sb.AppendLine("   ```bash");
            sb.AppendLine($"   cd {config.RootNamespace}.Api");
            sb.AppendLine("   ```");
            sb.AppendLine();
            sb.AppendLine("3. **Install EF Core tools (if not installed):**");
            sb.AppendLine("   ```bash");
            sb.AppendLine("   dotnet tool install --global dotnet-ef");
            sb.AppendLine("   ```");
            sb.AppendLine();
            sb.AppendLine("4. **Create migration and update database:**");
            sb.AppendLine("   ```bash");
            sb.AppendLine("   dotnet ef migrations add InitialCreate");
            sb.AppendLine("   dotnet ef database update");
            sb.AppendLine("   ```");
            sb.AppendLine();
        }
        
        // Step 4: Set Startup Project
        sb.AppendLine("## Step 4: Set the Startup Project");
        sb.AppendLine();
        sb.AppendLine("1. **In Solution Explorer:**");
        sb.AppendLine($"   - Right-click on `{config.RootNamespace}.Api` project");
        sb.AppendLine("   - Select **\"Set as Startup Project\"**");
        sb.AppendLine();
        sb.AppendLine("2. **Verify:**");
        sb.AppendLine($"   - The `{config.RootNamespace}.Api` project should now be **bold** in Solution Explorer");
        sb.AppendLine();
        
        // Step 5: Run the Application
        sb.AppendLine("## Step 5: Run the Application");
        sb.AppendLine();
        sb.AppendLine("### Method 1: Press F5 (Debug Mode)");
        sb.AppendLine();
        sb.AppendLine("- Press **F5** or click the **green play button** (▶️) in the toolbar");
        sb.AppendLine("- This will:");
        sb.AppendLine("  - Build the solution");
        sb.AppendLine("  - Start the API");
        sb.AppendLine("  - Open your browser automatically");
        sb.AppendLine("  - Attach the debugger");
        sb.AppendLine();
        sb.AppendLine("### Method 2: Ctrl+F5 (Without Debugging)");
        sb.AppendLine();
        sb.AppendLine("- Press **Ctrl+F5** or select `Debug` → `Start Without Debugging`");
        sb.AppendLine("- Faster startup, no debugger attached");
        sb.AppendLine();
        sb.AppendLine("### Method 3: Using Terminal");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine($"cd {config.RootNamespace}.Api");
        sb.AppendLine("dotnet run");
        sb.AppendLine("```");
        sb.AppendLine();
        
        // Step 6: Access the API
        sb.AppendLine("## Step 6: Access the API");
        sb.AppendLine();
        sb.AppendLine("Once the application starts, you'll see output like:");
        sb.AppendLine();
        sb.AppendLine("```");
        sb.AppendLine("info: Microsoft.Hosting.Lifetime[14]");
        sb.AppendLine("      Now listening on: https://localhost:5001");
        sb.AppendLine("      Now listening on: http://localhost:5000");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("### Access Points:");
        sb.AppendLine();
        sb.AppendLine("1. **Swagger UI (API Documentation):**");
        sb.AppendLine("   - URL: `https://localhost:5001/swagger`");
        sb.AppendLine("   - **This is your main testing interface!**");
        sb.AppendLine("   - Try out all API endpoints here");
        sb.AppendLine();
        sb.AppendLine("2. **API Base URL:**");
        sb.AppendLine("   - URL: `https://localhost:5001/api`");
        sb.AppendLine();
        sb.AppendLine("3. **Health Check:**");
        sb.AppendLine("   - URL: `https://localhost:5001/health`");
        sb.AppendLine("   - Should return: `Healthy`");
        sb.AppendLine();
        
        // Step 7: Test the API
        sb.AppendLine("## Step 7: Test the API with Swagger");
        sb.AppendLine();
        sb.AppendLine("1. **Open Swagger UI** at `https://localhost:5001/swagger`");
        sb.AppendLine();
        sb.AppendLine("2. **Expand an endpoint** (e.g., `GET /api/products`)");
        sb.AppendLine();
        sb.AppendLine("3. **Click \"Try it out\"**");
        sb.AppendLine();
        sb.AppendLine("4. **Click \"Execute\"**");
        sb.AppendLine();
        sb.AppendLine("5. **View the response:**");
        sb.AppendLine("   - Response body (JSON data)");
        sb.AppendLine("   - Response code (200, 404, etc.)");
        sb.AppendLine("   - Response headers");
        sb.AppendLine();
        sb.AppendLine("### Example: Create a New Item");
        sb.AppendLine();
        sb.AppendLine("1. Find the **POST** endpoint");
        sb.AppendLine("2. Click **\"Try it out\"**");
        sb.AppendLine("3. Edit the JSON request body");
        sb.AppendLine("4. Click **\"Execute\"**");
        sb.AppendLine("5. Check the response (should be 201 Created)");
        sb.AppendLine();
        
        // Troubleshooting
        sb.AppendLine("## 🔧 Troubleshooting");
        sb.AppendLine();
        sb.AppendLine("### Issue: Build Errors");
        sb.AppendLine();
        sb.AppendLine("**Solution:**");
        sb.AppendLine("1. Clean the solution: `Build` → `Clean Solution`");
        sb.AppendLine("2. Rebuild: `Build` → `Rebuild Solution`");
        sb.AppendLine("3. Check NuGet packages are restored");
        sb.AppendLine();
        sb.AppendLine("### Issue: Database Connection Failed");
        sb.AppendLine();
        sb.AppendLine("**Solution:**");
        sb.AppendLine("1. Verify connection string in `appsettings.json`");
        
        if (config.DatabaseProvider == DatabaseProvider.SqlServer)
        {
            sb.AppendLine("2. Check SQL Server is running:");
            sb.AppendLine("   - Open **SQL Server Object Explorer** in Visual Studio");
            sb.AppendLine("   - Expand **(localdb)\\\\MSSQLLocalDB**");
            sb.AppendLine("   - If not visible, SQL Server LocalDB may not be installed");
        }
        
        sb.AppendLine("3. Try running migrations again");
        sb.AppendLine();
        sb.AppendLine("### Issue: Port Already in Use");
        sb.AppendLine();
        sb.AppendLine("**Solution:**");
        sb.AppendLine($"1. Open `{config.RootNamespace}.Api/Properties/launchSettings.json`");
        sb.AppendLine("2. Change the port numbers:");
        sb.AppendLine("   ```json");
        sb.AppendLine("   \"applicationUrl\": \"https://localhost:7001;http://localhost:5000\"");
        sb.AppendLine("   ```");
        sb.AppendLine();
        sb.AppendLine("### Issue: Swagger Page Not Loading");
        sb.AppendLine();
        sb.AppendLine("**Solution:**");
        sb.AppendLine("1. Check the console output for the correct URL");
        sb.AppendLine("2. Manually navigate to `https://localhost:5001/swagger`");
        sb.AppendLine("3. Try `http://localhost:5000/swagger` if HTTPS fails");
        sb.AppendLine();
        
        // Hot Reload
        sb.AppendLine("## 🔥 Hot Reload (Visual Studio 2022)");
        sb.AppendLine();
        sb.AppendLine("Visual Studio 2022 supports **Hot Reload** - make code changes without restarting!");
        sb.AppendLine();
        sb.AppendLine("1. Start the application with **F5**");
        sb.AppendLine("2. Make changes to your code");
        sb.AppendLine("3. Save the file (**Ctrl+S**)");
        sb.AppendLine("4. Visual Studio will apply changes automatically");
        sb.AppendLine("5. Look for the 🔥 icon in the toolbar");
        sb.AppendLine();
        sb.AppendLine("**Note:** Some changes require a restart (e.g., adding new dependencies)");
        sb.AppendLine();
        
        // Debugging Tips
        sb.AppendLine("## 🐛 Debugging Tips");
        sb.AppendLine();
        sb.AppendLine("### Set Breakpoints");
        sb.AppendLine();
        sb.AppendLine("1. Click in the left margin next to any line of code");
        sb.AppendLine("2. A red dot appears = breakpoint set");
        sb.AppendLine("3. Run with **F5** (Debug mode)");
        sb.AppendLine("4. Execution pauses at breakpoints");
        sb.AppendLine("5. Use **F10** (Step Over) and **F11** (Step Into) to navigate");
        sb.AppendLine();
        sb.AppendLine("### View Variables");
        sb.AppendLine();
        sb.AppendLine("When paused at a breakpoint:");
        sb.AppendLine("- Hover over variables to see values");
        sb.AppendLine("- Check **Locals** window (`Debug` → `Windows` → `Locals`)");
        sb.AppendLine("- Check **Watch** window to monitor specific variables");
        sb.AppendLine();
        sb.AppendLine("### Output Window");
        sb.AppendLine();
        sb.AppendLine("- View logs: `View` → `Output`");
        sb.AppendLine("- Select \"Debug\" or \"ASP.NET Core Web Server\" from dropdown");
        sb.AppendLine();
        
        // Next Steps
        sb.AppendLine("## 🎯 Next Steps");
        sb.AppendLine();
        sb.AppendLine("Now that your API is running:");
        sb.AppendLine();
        sb.AppendLine("1. ✅ **Explore Swagger UI** - Test all endpoints");
        sb.AppendLine("2. ✅ **Check the database** - Use SQL Server Object Explorer");
        sb.AppendLine("3. ✅ **Review the code** - Understand the architecture");
        sb.AppendLine("4. ✅ **Add business logic** - Customize for your needs");
        sb.AppendLine("5. ✅ **Write tests** - Ensure code quality");
        sb.AppendLine();
        sb.AppendLine("### Recommended Reading");
        sb.AppendLine();
        sb.AppendLine("- 📖 **README.md** - Full documentation");
        sb.AppendLine("- 📖 **ARCHITECTURE.md** - Architecture overview");
        sb.AppendLine("- 📖 **Swagger UI** - Interactive API documentation");
        sb.AppendLine();
        
        // Useful Shortcuts
        sb.AppendLine("## ⌨️ Useful Visual Studio Shortcuts");
        sb.AppendLine();
        sb.AppendLine("| Shortcut | Action |");
        sb.AppendLine("|----------|--------|");
        sb.AppendLine("| **F5** | Start Debugging |");
        sb.AppendLine("| **Ctrl+F5** | Start Without Debugging |");
        sb.AppendLine("| **Shift+F5** | Stop Debugging |");
        sb.AppendLine("| **F9** | Toggle Breakpoint |");
        sb.AppendLine("| **F10** | Step Over |");
        sb.AppendLine("| **F11** | Step Into |");
        sb.AppendLine("| **Ctrl+Shift+B** | Build Solution |");
        sb.AppendLine("| **Ctrl+K, Ctrl+D** | Format Document |");
        sb.AppendLine("| **Ctrl+.** | Quick Actions (Fix errors) |");
        sb.AppendLine("| **Ctrl+T** | Go to File/Type |");
        sb.AppendLine();
        
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("## ✅ Success Checklist");
        sb.AppendLine();
        sb.AppendLine("- [ ] Solution opens without errors");
        sb.AppendLine("- [ ] NuGet packages restored");
        sb.AppendLine("- [ ] Connection string updated");
        sb.AppendLine("- [ ] Database created successfully");
        sb.AppendLine("- [ ] Application starts (F5)");
        sb.AppendLine("- [ ] Swagger UI loads at /swagger");
        sb.AppendLine("- [ ] Health check returns \"Healthy\"");
        sb.AppendLine("- [ ] Can create/read/update/delete items via Swagger");
        sb.AppendLine();
        sb.AppendLine("**If all checked ✅ - You're ready to develop!** 🎉");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine($"*Generated by MyCodeGent on {DateTime.Now:yyyy-MM-dd HH:mm:ss}*");
        
        return sb.ToString();
    }
    
    public static string GenerateVSCodeGuide(GenerationConfig config)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("# 🚀 Quick Start Guide - VS Code");
        sb.AppendLine();
        sb.AppendLine($"## Running {config.RootNamespace} in Visual Studio Code");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        
        // Prerequisites
        sb.AppendLine("## Prerequisites");
        sb.AppendLine();
        sb.AppendLine("1. **.NET SDK** installed (check with `dotnet --version`)");
        sb.AppendLine("2. **VS Code** with C# extension");
        sb.AppendLine("3. **EF Core tools** (install with `dotnet tool install --global dotnet-ef`)");
        sb.AppendLine();
        
        // Step 1: Open Folder
        sb.AppendLine("## Step 1: Open the Project");
        sb.AppendLine();
        sb.AppendLine("1. Open VS Code");
        sb.AppendLine("2. `File` → `Open Folder`");
        sb.AppendLine("3. Select the generated project folder");
        sb.AppendLine("4. VS Code will detect the .NET project and offer to add build assets");
        sb.AppendLine("5. Click **\"Yes\"** to add required assets");
        sb.AppendLine();
        
        // Step 2: Update Connection String
        sb.AppendLine("## Step 2: Configure Database");
        sb.AppendLine();
        sb.AppendLine($"1. Open `{config.RootNamespace}.Api/appsettings.json`");
        sb.AppendLine("2. Update the connection string (see README.md for examples)");
        sb.AppendLine();
        
        // Step 3: Terminal Commands
        sb.AppendLine("## Step 3: Setup Database");
        sb.AppendLine();
        sb.AppendLine("Open integrated terminal (**Ctrl+`**) and run:");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine($"cd {config.RootNamespace}.Api");
        sb.AppendLine("dotnet ef migrations add InitialCreate");
        sb.AppendLine("dotnet ef database update");
        sb.AppendLine("```");
        sb.AppendLine();
        
        // Step 4: Run
        sb.AppendLine("## Step 4: Run the Application");
        sb.AppendLine();
        sb.AppendLine("### Method 1: Using Terminal");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine("dotnet run");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("### Method 2: Using Debugger");
        sb.AppendLine();
        sb.AppendLine("1. Press **F5**");
        sb.AppendLine("2. Select **.NET Core** if prompted");
        sb.AppendLine("3. Application starts with debugger attached");
        sb.AppendLine();
        
        // Access
        sb.AppendLine("## Step 5: Access the API");
        sb.AppendLine();
        sb.AppendLine("- **Swagger UI:** `https://localhost:5001/swagger`");
        sb.AppendLine("- **API:** `https://localhost:5001/api`");
        sb.AppendLine("- **Health:** `https://localhost:5001/health`");
        sb.AppendLine();
        
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine($"*Generated by MyCodeGent on {DateTime.Now:yyyy-MM-dd HH:mm:ss}*");
        
        return sb.ToString();
    }
}
