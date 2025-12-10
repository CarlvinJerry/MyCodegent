using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MyCodeGent.Web.Services;

public interface IGitVersionService
{
    GitVersionInfo GetGitVersionInfo();
    string GetSemanticVersion();
}

public class GitVersionService : IGitVersionService
{
    private readonly string _repositoryPath;

    public GitVersionService()
    {
        _repositoryPath = FindGitRepository();
    }

    public GitVersionInfo GetGitVersionInfo()
    {
        var info = new GitVersionInfo();

        try
        {
            // Get current branch
            info.Branch = ExecuteGitCommand("rev-parse --abbrev-ref HEAD").Trim();

            // Get latest commit hash
            info.CommitHash = ExecuteGitCommand("rev-parse HEAD").Trim();
            info.ShortCommitHash = ExecuteGitCommand("rev-parse --short HEAD").Trim();

            // Get commit date
            var commitDateStr = ExecuteGitCommand("log -1 --format=%ci").Trim();
            if (DateTime.TryParse(commitDateStr, out var commitDate))
            {
                info.CommitDate = commitDate;
            }

            // Get commit message
            info.CommitMessage = ExecuteGitCommand("log -1 --format=%s").Trim();

            // Get commit author
            info.CommitAuthor = ExecuteGitCommand("log -1 --format=%an").Trim();

            // Get tag (if on a tag)
            info.Tag = ExecuteGitCommand("describe --tags --exact-match 2>nul").Trim();

            // Get total commit count
            var commitCountStr = ExecuteGitCommand("rev-list --count HEAD").Trim();
            if (int.TryParse(commitCountStr, out var commitCount))
            {
                info.CommitCount = commitCount;
            }

            // Check if working directory is clean
            var status = ExecuteGitCommand("status --porcelain").Trim();
            info.IsClean = string.IsNullOrEmpty(status);

            // Get commits since last tag
            var commitsSinceTag = ExecuteGitCommand("describe --tags --long").Trim();
            if (!string.IsNullOrEmpty(commitsSinceTag))
            {
                var match = Regex.Match(commitsSinceTag, @"-(\d+)-g");
                if (match.Success && int.TryParse(match.Groups[1].Value, out var commits))
                {
                    info.CommitsSinceTag = commits;
                }
            }

            info.IsAvailable = true;
        }
        catch
        {
            info.IsAvailable = false;
        }

        return info;
    }

    public string GetSemanticVersion()
    {
        var gitInfo = GetGitVersionInfo();
        
        if (!gitInfo.IsAvailable)
        {
            return "2.0.0-nogit";
        }

        // Try to get version from tag
        if (!string.IsNullOrEmpty(gitInfo.Tag) && gitInfo.Tag.StartsWith("v"))
        {
            var tagVersion = gitInfo.Tag.TrimStart('v');
            if (!gitInfo.IsClean)
            {
                tagVersion += "-dirty";
            }
            return tagVersion;
        }

        // Calculate version based on commit count and changes
        var major = 2; // Current major version
        var minor = 0;
        var patch = gitInfo.CommitsSinceTag > 0 ? gitInfo.CommitsSinceTag : 0;

        // Check commit messages for version hints
        var recentCommits = GetRecentCommitMessages(10);
        
        if (recentCommits.Any(c => c.Contains("BREAKING CHANGE", StringComparison.OrdinalIgnoreCase) ||
                                   c.Contains("major:", StringComparison.OrdinalIgnoreCase)))
        {
            major++;
            minor = 0;
            patch = 0;
        }
        else if (recentCommits.Any(c => c.Contains("feat:", StringComparison.OrdinalIgnoreCase) ||
                                        c.Contains("feature:", StringComparison.OrdinalIgnoreCase)))
        {
            minor++;
            patch = 0;
        }

        var version = $"{major}.{minor}.{patch}";
        
        if (!gitInfo.IsClean)
        {
            version += $"-dev+{gitInfo.ShortCommitHash}";
        }
        else if (!string.IsNullOrEmpty(gitInfo.ShortCommitHash))
        {
            version += $"+{gitInfo.ShortCommitHash}";
        }

        return version;
    }

    private List<string> GetRecentCommitMessages(int count)
    {
        try
        {
            var output = ExecuteGitCommand($"log -{count} --format=%s");
            return output.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        catch
        {
            return new List<string>();
        }
    }

    private string ExecuteGitCommand(string arguments)
    {
        try
        {
            if (string.IsNullOrEmpty(_repositoryPath))
            {
                return string.Empty;
            }

            var processInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = _repositoryPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                return string.Empty;
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return string.Empty;
            }

            return output;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string FindGitRepository()
    {
        try
        {
            var currentDir = Directory.GetCurrentDirectory();
            
            while (!string.IsNullOrEmpty(currentDir))
            {
                if (Directory.Exists(Path.Combine(currentDir, ".git")))
                {
                    return currentDir;
                }

                var parent = Directory.GetParent(currentDir);
                currentDir = parent?.FullName ?? string.Empty;
            }

            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}

public class GitVersionInfo
{
    public bool IsAvailable { get; set; }
    public string Branch { get; set; } = string.Empty;
    public string CommitHash { get; set; } = string.Empty;
    public string ShortCommitHash { get; set; } = string.Empty;
    public DateTime CommitDate { get; set; }
    public string CommitMessage { get; set; } = string.Empty;
    public string CommitAuthor { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public int CommitCount { get; set; }
    public int CommitsSinceTag { get; set; }
    public bool IsClean { get; set; }
}
