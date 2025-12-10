using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class SolutionTemplate
{
    public static string GenerateSolutionFile(GenerationConfig config)
    {
        var sb = new StringBuilder();
        
        // Solution header
        sb.AppendLine();
        sb.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
        sb.AppendLine("# Visual Studio Version 17");
        sb.AppendLine("VisualStudioVersion = 17.0.31903.59");
        sb.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");
        
        // Generate GUIDs for each project
        var domainGuid = Guid.NewGuid().ToString().ToUpper();
        var applicationGuid = Guid.NewGuid().ToString().ToUpper();
        var infrastructureGuid = Guid.NewGuid().ToString().ToUpper();
        var apiGuid = Guid.NewGuid().ToString().ToUpper();
        var testsGuid = Guid.NewGuid().ToString().ToUpper();
        
        // Solution folders
        var srcFolderGuid = Guid.NewGuid().ToString().ToUpper();
        var testsFolderGuid = Guid.NewGuid().ToString().ToUpper();
        
        // Add projects
        if (config.GenerateDomain)
        {
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.RootNamespace}.Domain\", \"{config.RootNamespace}.Domain\\{config.RootNamespace}.Domain.csproj\", \"{{{domainGuid}}}\"");
            sb.AppendLine("EndProject");
        }
        
        if (config.GenerateApplication)
        {
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.RootNamespace}.Application\", \"{config.RootNamespace}.Application\\{config.RootNamespace}.Application.csproj\", \"{{{applicationGuid}}}\"");
            sb.AppendLine("EndProject");
        }
        
        if (config.GenerateInfrastructure)
        {
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.RootNamespace}.Infrastructure\", \"{config.RootNamespace}.Infrastructure\\{config.RootNamespace}.Infrastructure.csproj\", \"{{{infrastructureGuid}}}\"");
            sb.AppendLine("EndProject");
        }
        
        if (config.GenerateApi)
        {
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.RootNamespace}.Api\", \"{config.RootNamespace}.Api\\{config.RootNamespace}.Api.csproj\", \"{{{apiGuid}}}\"");
            sb.AppendLine("EndProject");
        }
        
        // Add test project
        sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.RootNamespace}.Application.Tests\", \"Tests\\{config.RootNamespace}.Application.Tests\\{config.RootNamespace}.Application.Tests.csproj\", \"{{{testsGuid}}}\"");
        sb.AppendLine("EndProject");
        
        // Solution folders
        sb.AppendLine($"Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"src\", \"src\", \"{{{srcFolderGuid}}}\"");
        sb.AppendLine("EndProject");
        sb.AppendLine($"Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"tests\", \"tests\", \"{{{testsFolderGuid}}}\"");
        sb.AppendLine("EndProject");
        
        // Global section
        sb.AppendLine("Global");
        
        // Solution configuration platforms
        sb.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
        sb.AppendLine("\t\tDebug|Any CPU = Debug|Any CPU");
        sb.AppendLine("\t\tRelease|Any CPU = Release|Any CPU");
        sb.AppendLine("\tEndGlobalSection");
        
        // Project configuration platforms
        sb.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
        
        if (config.GenerateDomain)
        {
            sb.AppendLine($"\t\t{{{domainGuid}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{domainGuid}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{domainGuid}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
            sb.AppendLine($"\t\t{{{domainGuid}}}.Release|Any CPU.Build.0 = Release|Any CPU");
        }
        
        if (config.GenerateApplication)
        {
            sb.AppendLine($"\t\t{{{applicationGuid}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{applicationGuid}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{applicationGuid}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
            sb.AppendLine($"\t\t{{{applicationGuid}}}.Release|Any CPU.Build.0 = Release|Any CPU");
        }
        
        if (config.GenerateInfrastructure)
        {
            sb.AppendLine($"\t\t{{{infrastructureGuid}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{infrastructureGuid}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{infrastructureGuid}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
            sb.AppendLine($"\t\t{{{infrastructureGuid}}}.Release|Any CPU.Build.0 = Release|Any CPU");
        }
        
        if (config.GenerateApi)
        {
            sb.AppendLine($"\t\t{{{apiGuid}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{apiGuid}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{apiGuid}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
            sb.AppendLine($"\t\t{{{apiGuid}}}.Release|Any CPU.Build.0 = Release|Any CPU");
        }
        
        sb.AppendLine($"\t\t{{{testsGuid}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
        sb.AppendLine($"\t\t{{{testsGuid}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
        sb.AppendLine($"\t\t{{{testsGuid}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
        sb.AppendLine($"\t\t{{{testsGuid}}}.Release|Any CPU.Build.0 = Release|Any CPU");
        
        sb.AppendLine("\tEndGlobalSection");
        
        // Nested projects (organize projects into folders)
        sb.AppendLine("\tGlobalSection(NestedProjects) = preSolution");
        
        if (config.GenerateDomain)
            sb.AppendLine($"\t\t{{{domainGuid}}} = {{{srcFolderGuid}}}");
        
        if (config.GenerateApplication)
            sb.AppendLine($"\t\t{{{applicationGuid}}} = {{{srcFolderGuid}}}");
        
        if (config.GenerateInfrastructure)
            sb.AppendLine($"\t\t{{{infrastructureGuid}}} = {{{srcFolderGuid}}}");
        
        if (config.GenerateApi)
            sb.AppendLine($"\t\t{{{apiGuid}}} = {{{srcFolderGuid}}}");
        
        sb.AppendLine($"\t\t{{{testsGuid}}} = {{{testsFolderGuid}}}");
        
        sb.AppendLine("\tEndGlobalSection");
        
        // Solution properties
        sb.AppendLine("\tGlobalSection(SolutionProperties) = preSolution");
        sb.AppendLine("\t\tHideSolutionNode = FALSE");
        sb.AppendLine("\tEndGlobalSection");
        
        // Extensibility globals
        sb.AppendLine("\tGlobalSection(ExtensibilityGlobals) = postSolution");
        sb.AppendLine($"\t\tSolutionGuid = {{{Guid.NewGuid().ToString().ToUpper()}}}");
        sb.AppendLine("\tEndGlobalSection");
        
        sb.AppendLine("EndGlobal");
        
        return sb.ToString();
    }
}
