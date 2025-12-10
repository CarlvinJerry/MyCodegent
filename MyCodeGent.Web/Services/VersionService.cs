using System.Reflection;
using System.Runtime.InteropServices;

namespace MyCodeGent.Web.Services;

public interface IVersionService
{
    VersionInfo GetVersionInfo();
}

public class VersionService : IVersionService
{
    public VersionInfo GetVersionInfo()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        var buildDate = GetBuildDate(assembly);
        var dotnetVersion = Environment.Version;
        
        return new VersionInfo
        {
            Version = version?.ToString(3) ?? "2.0.0",
            BuildNumber = $"{buildDate:yyyy.MM.dd}.{version?.Revision ?? 1:D3}",
            BuildDate = buildDate,
            DotNetVersion = $"{dotnetVersion.Major}.{dotnetVersion.Minor}",
            RuntimeVersion = RuntimeInformation.FrameworkDescription,
            OsDescription = RuntimeInformation.OSDescription,
            ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            LastUpdated = buildDate.ToString("MMMM yyyy")
        };
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
}
