using System.Reflection;
using System.Runtime.InteropServices;

namespace MyCodeGent.Web.Services;

public interface IVersionService
{
    VersionInfo GetVersionInfo();
}

public class VersionService : IVersionService
{
    private readonly IGitVersionService _gitVersionService;

    public VersionService(IGitVersionService gitVersionService)
    {
        _gitVersionService = gitVersionService;
    }

    public VersionInfo GetVersionInfo()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            var buildDate = GetBuildDate(assembly);
            var dotnetVersion = Environment.Version;
            
            // Try to get Git version info (may fail if Git not available)
            string semanticVersion = "2.0.0";
            string gitBranch = string.Empty;
            string gitCommitHash = string.Empty;
            DateTime gitCommitDate = default;
            string gitCommitMessage = string.Empty;
            string gitCommitAuthor = string.Empty;
            string gitTag = string.Empty;
            bool gitIsClean = true;
            int gitCommitCount = 0;
            
            try
            {
                var gitInfo = _gitVersionService.GetGitVersionInfo();
                semanticVersion = _gitVersionService.GetSemanticVersion();
                gitBranch = gitInfo.Branch;
                gitCommitHash = gitInfo.ShortCommitHash;
                gitCommitDate = gitInfo.CommitDate;
                gitCommitMessage = gitInfo.CommitMessage;
                gitCommitAuthor = gitInfo.CommitAuthor;
                gitTag = gitInfo.Tag;
                gitIsClean = gitInfo.IsClean;
                gitCommitCount = gitInfo.CommitCount;
            }
            catch
            {
                // Git not available, use assembly version
                semanticVersion = version?.ToString(3) ?? "2.0.0";
            }
            
            return new VersionInfo
            {
                Version = semanticVersion,
                BuildNumber = $"{buildDate:yyyy.MM.dd}.{version?.Revision ?? 1:D3}",
                BuildDate = buildDate,
                DotNetVersion = $"{dotnetVersion.Major}.{dotnetVersion.Minor}",
                RuntimeVersion = RuntimeInformation.FrameworkDescription,
                OsDescription = RuntimeInformation.OSDescription,
                ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                LastUpdated = buildDate.ToString("MMMM yyyy"),
                GitBranch = gitBranch,
                GitCommitHash = gitCommitHash,
                GitCommitDate = gitCommitDate,
                GitCommitMessage = gitCommitMessage,
                GitCommitAuthor = gitCommitAuthor,
                GitTag = gitTag,
                GitIsClean = gitIsClean,
                GitCommitCount = gitCommitCount
            };
        }
        catch (Exception ex)
        {
            // Fallback to minimal version info if everything fails
            return new VersionInfo
            {
                Version = "2.0.0",
                BuildNumber = $"{DateTime.Now:yyyy.MM.dd}.001",
                BuildDate = DateTime.Now,
                DotNetVersion = "9.0",
                RuntimeVersion = RuntimeInformation.FrameworkDescription,
                OsDescription = RuntimeInformation.OSDescription,
                ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                LastUpdated = DateTime.Now.ToString("MMMM yyyy")
            };
        }
    }
    
    private DateTime GetBuildDate(Assembly assembly)
    {
        // Try to get build date from assembly attribute
        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        
        // If not available, use file creation time
        var location = assembly.Location;
        if (!string.IsNullOrEmpty(location) && File.Exists(location))
        {
            return File.GetLastWriteTime(location);
        }
        
        // Fallback to current date
        return DateTime.Now;
    }
}

public class VersionInfo
{
    public string Version { get; set; } = "2.0.0";
    public string BuildNumber { get; set; } = string.Empty;
    public DateTime BuildDate { get; set; }
    public string DotNetVersion { get; set; } = "9.0";
    public string RuntimeVersion { get; set; } = string.Empty;
    public string OsDescription { get; set; } = string.Empty;
    public string ProcessArchitecture { get; set; } = string.Empty;
    public string LastUpdated { get; set; } = string.Empty;
    
    // Git information
    public string GitBranch { get; set; } = string.Empty;
    public string GitCommitHash { get; set; } = string.Empty;
    public DateTime GitCommitDate { get; set; }
    public string GitCommitMessage { get; set; } = string.Empty;
    public string GitCommitAuthor { get; set; } = string.Empty;
    public string GitTag { get; set; } = string.Empty;
    public bool GitIsClean { get; set; }
    public int GitCommitCount { get; set; }
}
