using MyCodeGent.Templates.Models;

namespace MyCodeGent.Core.Interfaces;

public interface ICodeGenerator
{
    Task GenerateAsync(EntityModel entity, GenerationConfig config);
    Task GenerateTestsAsync(EntityModel entity, GenerationConfig config);
    Task GenerateApplicationInfrastructureAsync(List<EntityModel> entities, GenerationConfig config);
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
