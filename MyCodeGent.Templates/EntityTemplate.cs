using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class EntityTemplate
{
    public static string Generate(EntityModel entity)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"using System;");
        sb.AppendLine($"using System.ComponentModel.DataAnnotations;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Domain.Entities;");
        sb.AppendLine();
        sb.AppendLine($"public class {entity.Name}");
        sb.AppendLine("{");
        
        // Generate properties
        foreach (var prop in entity.Properties)
        {
            if (prop.IsRequired && !prop.IsNullable)
            {
                sb.AppendLine("    [Required]");
            }
            
            if (prop.MaxLength.HasValue)
            {
                sb.AppendLine($"    [MaxLength({prop.MaxLength.Value})]");
            }
            
            if (prop.IsKey)
            {
                sb.AppendLine("    [Key]");
            }
            
            var nullableSymbol = prop.IsNullable && !IsValueType(prop.Type) ? "?" : "";
            sb.AppendLine($"    public {prop.Type}{nullableSymbol} {prop.Name} {{ get; set; }}");
            sb.AppendLine();
        }
        
        // Add audit fields if enabled
        if (entity.HasAuditFields)
        {
            sb.AppendLine("    public DateTime CreatedAt { get; set; }");
            sb.AppendLine("    public string? CreatedBy { get; set; }");
            sb.AppendLine("    public DateTime? UpdatedAt { get; set; }");
            sb.AppendLine("    public string? UpdatedBy { get; set; }");
            sb.AppendLine();
        }
        
        // Add soft delete if enabled
        if (entity.HasSoftDelete)
        {
            sb.AppendLine("    public bool IsDeleted { get; set; }");
            sb.AppendLine("    public DateTime? DeletedAt { get; set; }");
            sb.AppendLine("    public string? DeletedBy { get; set; }");
        }
        
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    private static bool IsValueType(string type)
    {
        return type switch
        {
            "int" or "long" or "decimal" or "double" or "float" or "bool" or "DateTime" or "Guid" => true,
            _ => false
        };
    }
}
