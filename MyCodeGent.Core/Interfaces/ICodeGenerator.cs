using MyCodeGent.Templates.Models;

namespace MyCodeGent.Core.Interfaces;

public interface ICodeGenerator
{
    Task GenerateAsync(EntityModel entity, GenerationConfig config);
}

public interface ITemplateEngine
{
    string Render(string templateName, object model);
}

public interface IFileWriter
{
    Task WriteFileAsync(string path, string content);
    Task<bool> FileExistsAsync(string path);
    Task CreateDirectoryAsync(string path);
}
