using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class DtoTemplate
{
    public static string Generate(EntityModel entity)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s;");
        sb.AppendLine();
        sb.AppendLine($"public class {entity.Name}Dto");
        sb.AppendLine("{");
        
        foreach (var prop in entity.Properties)
        {
            var nullableSymbol = prop.IsNullable ? "?" : "";
            sb.AppendLine($"    public {prop.Type}{nullableSymbol} {prop.Name} {{ get; set; }}");
        }
        
        if (entity.HasAuditFields)
        {
            sb.AppendLine("    public DateTime CreatedAt { get; set; }");
            sb.AppendLine("    public DateTime? UpdatedAt { get; set; }");
        }
        
        sb.AppendLine("}");
        
        return sb.ToString();
    }
}
