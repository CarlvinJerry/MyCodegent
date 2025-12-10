using System.Diagnostics;
using System.Text;

namespace MyCodeGent.Core.Services;

public interface IGitService
{
    Task<bool> InitializeRepositoryAsync(string path);
    Task<bool> CommitChangesAsync(string path, string message);
    Task<string> GetGitStatusAsync(string path);
    bool IsGitInstalled();
}

public class GitService : IGitService
{
    public bool IsGitInstalled()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> InitializeRepositoryAsync(string path)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            
            // Check if already a git repository
            var gitDir = Path.Combine(path, ".git");
            if (Directory.Exists(gitDir))
            {
                Console.WriteLine("Git repository already initialized");
                return true;
            }
            
            // Initialize git repository
            var initResult = await ExecuteGitCommandAsync(path, "init");
            if (!initResult.Success)
            {
                Console.WriteLine($"Failed to initialize git: {initResult.Error}");
                return false;
            }
            
            // Create .gitignore
            await CreateGitIgnoreAsync(path);
            
            // Configure git
            await ExecuteGitCommandAsync(path, "config user.name \"MyCodeGent\"");
            await ExecuteGitCommandAsync(path, "config user.email \"mycodegent@generated.local\"");
            
            Console.WriteLine("✓ Git repository initialized");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing git: {ex.Message}");
            return false;
        }
    }
    
    public async Task<bool> CommitChangesAsync(string path, string message)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            
            // Add all files
            var addResult = await ExecuteGitCommandAsync(path, "add .");
            if (!addResult.Success)
            {
                Console.WriteLine($"Failed to stage files: {addResult.Error}");
                return false;
            }
            
            // Commit
            var commitResult = await ExecuteGitCommandAsync(path, $"commit -m \"{message}\"");
            if (!commitResult.Success)
            {
                // Check if it's because there's nothing to commit
                if (commitResult.Error.Contains("nothing to commit") || 
                    commitResult.Error.Contains("no changes added"))
                {
                    Console.WriteLine("No changes to commit");
                    return true;
                }
                
                Console.WriteLine($"Failed to commit: {commitResult.Error}");
                return false;
            }
            
            Console.WriteLine($"✓ Committed: {message}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error committing changes: {ex.Message}");
            return false;
        }
    }
    
    public async Task<string> GetGitStatusAsync(string path)
    {
        try
        {
            var result = await ExecuteGitCommandAsync(path, "status --short");
            return result.Success ? result.Output : result.Error;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
    
    private async Task<GitCommandResult> ExecuteGitCommandAsync(string workingDirectory, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        
        var output = new StringBuilder();
        var error = new StringBuilder();
        
        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null) output.AppendLine(e.Data);
        };
        
        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null) error.AppendLine(e.Data);
        };
        
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        
        await process.WaitForExitAsync();
        
        return new GitCommandResult
        {
            Success = process.ExitCode == 0,
            Output = output.ToString(),
            Error = error.ToString()
        };
    }
    
    private async Task CreateGitIgnoreAsync(string path)
    {
        var gitignorePath = Path.Combine(path, ".gitignore");
        
        var gitignoreContent = @"## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.

# User-specific files
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio cache/options
.vs/
*.cache

# NuGet Packages
*.nupkg
**/packages/*
!**/packages/build/

# Database files
*.db
*.db-shm
*.db-wal

# Log files
logs/
*.log

# OS generated files
.DS_Store
Thumbs.db
";
        
        await File.WriteAllTextAsync(gitignorePath, gitignoreContent);
    }
    
    private class GitCommandResult
    {
        public bool Success { get; set; }
        public string Output { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
