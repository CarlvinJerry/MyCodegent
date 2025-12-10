using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class CommandTemplate
{
    public static string GenerateCreateCommand(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        var keyType = keyProp?.Type ?? "int";
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Commands.Create{entity.Name};");
        sb.AppendLine();
        sb.AppendLine($"public record Create{entity.Name}Command : IRequest<{keyType}>");
        sb.AppendLine("{");
        
        foreach (var prop in entity.Properties.Where(p => !p.IsKey))
        {
            var nullableSymbol = prop.IsNullable ? "?" : "";
            sb.AppendLine($"    public {prop.Type}{nullableSymbol} {prop.Name} {{ get; init; }}");
        }
        
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateUpdateCommand(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        var keyType = keyProp?.Type ?? "int";
        var keyName = keyProp?.Name ?? "Id";
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Commands.Update{entity.Name};");
        sb.AppendLine();
        sb.AppendLine($"public record Update{entity.Name}Command : IRequest<bool>");
        sb.AppendLine("{");
        sb.AppendLine($"    public {keyType} {keyName} {{ get; init; }}");
        
        foreach (var prop in entity.Properties.Where(p => !p.IsKey))
        {
            var nullableSymbol = prop.IsNullable ? "?" : "";
            sb.AppendLine($"    public {prop.Type}{nullableSymbol} {prop.Name} {{ get; init; }}");
        }
        
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateDeleteCommand(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        var keyType = keyProp?.Type ?? "int";
        var keyName = keyProp?.Name ?? "Id";
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Commands.Delete{entity.Name};");
        sb.AppendLine();
        sb.AppendLine($"public record Delete{entity.Name}Command({keyType} {keyName}) : IRequest<bool>;");
        
        return sb.ToString();
    }
}
