using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class ValidatorTemplate
{
    public static string GenerateCreateValidator(EntityModel entity)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using FluentValidation;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Commands.Create{entity.Name};");
        sb.AppendLine();
        sb.AppendLine($"public class Create{entity.Name}CommandValidator : AbstractValidator<Create{entity.Name}Command>");
        sb.AppendLine("{");
        sb.AppendLine($"    public Create{entity.Name}CommandValidator()");
        sb.AppendLine("    {");
        
        foreach (var prop in entity.Properties.Where(p => !p.IsKey))
        {
            var hasRules = prop.IsRequired || prop.MaxLength.HasValue || prop.Constraints != null;
            if (!hasRules) continue;
            
            sb.AppendLine($"        RuleFor(x => x.{prop.Name})");
            
            // Required validation
            if (prop.IsRequired)
            {
                sb.AppendLine("            .NotEmpty()");
            }
            
            // String constraints
            if (prop.Type == "string" && prop.Constraints != null)
            {
                if (!string.IsNullOrEmpty(prop.Constraints.MinLength))
                {
                    sb.AppendLine($"            .MinimumLength({prop.Constraints.MinLength})");
                }
                
                if (!string.IsNullOrEmpty(prop.Constraints.MaxLength))
                {
                    sb.AppendLine($"            .MaximumLength({prop.Constraints.MaxLength})");
                }
                else if (prop.MaxLength.HasValue)
                {
                    sb.AppendLine($"            .MaximumLength({prop.MaxLength.Value})");
                }
                
                if (!string.IsNullOrEmpty(prop.Constraints.RegexPattern))
                {
                    var escapedPattern = prop.Constraints.RegexPattern.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    sb.AppendLine($"            .Matches(@\"{escapedPattern}\")");
                }
                
                // Email validation
                if (prop.Name.Contains("Email", StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine("            .EmailAddress()");
                }
            }
            // Numeric constraints
            else if (IsNumericType(prop.Type) && prop.Constraints != null)
            {
                if (!string.IsNullOrEmpty(prop.Constraints.MinValue))
                {
                    sb.AppendLine($"            .GreaterThanOrEqualTo({prop.Constraints.MinValue})");
                }
                
                if (!string.IsNullOrEmpty(prop.Constraints.MaxValue))
                {
                    sb.AppendLine($"            .LessThanOrEqualTo({prop.Constraints.MaxValue})");
                }
                
                // Precision and scale for decimal
                if (prop.Type == "decimal" && !string.IsNullOrEmpty(prop.Constraints.Precision) && !string.IsNullOrEmpty(prop.Constraints.Scale))
                {
                    sb.AppendLine($"            .PrecisionScale({prop.Constraints.Precision}, {prop.Constraints.Scale}, true)");
                }
            }
            
            // When clause for nullable properties
            if (!prop.IsRequired)
            {
                sb.AppendLine($"            .When(x => x.{prop.Name} != null)");
            }
            
            sb.AppendLine($"            .WithMessage(\"{prop.Name} validation failed.\");");
            sb.AppendLine();
        }
        
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    private static bool IsNumericType(string type)
    {
        return type switch
        {
            "int" or "long" or "short" or "byte" or "decimal" or "double" or "float" => true,
            _ => false
        };
    }
    
    public static string GenerateUpdateValidator(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        var keyName = keyProp?.Name ?? "Id";
        
        sb.AppendLine("using FluentValidation;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Commands.Update{entity.Name};");
        sb.AppendLine();
        sb.AppendLine($"public class Update{entity.Name}CommandValidator : AbstractValidator<Update{entity.Name}Command>");
        sb.AppendLine("{");
        sb.AppendLine($"    public Update{entity.Name}CommandValidator()");
        sb.AppendLine("    {");
        sb.AppendLine($"        RuleFor(x => x.{keyName})");
        sb.AppendLine("            .NotEmpty();");
        sb.AppendLine();
        
        foreach (var prop in entity.Properties.Where(p => !p.IsKey))
        {
            var hasRules = prop.IsRequired || prop.MaxLength.HasValue || prop.Constraints != null;
            if (!hasRules) continue;
            
            sb.AppendLine($"        RuleFor(x => x.{prop.Name})");
            
            // Required validation
            if (prop.IsRequired)
            {
                sb.AppendLine("            .NotEmpty()");
            }
            
            // String constraints
            if (prop.Type == "string" && prop.Constraints != null)
            {
                if (!string.IsNullOrEmpty(prop.Constraints.MinLength))
                {
                    sb.AppendLine($"            .MinimumLength({prop.Constraints.MinLength})");
                }
                
                if (!string.IsNullOrEmpty(prop.Constraints.MaxLength))
                {
                    sb.AppendLine($"            .MaximumLength({prop.Constraints.MaxLength})");
                }
                else if (prop.MaxLength.HasValue)
                {
                    sb.AppendLine($"            .MaximumLength({prop.MaxLength.Value})");
                }
                
                if (!string.IsNullOrEmpty(prop.Constraints.RegexPattern))
                {
                    var escapedPattern = prop.Constraints.RegexPattern.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    sb.AppendLine($"            .Matches(@\"{escapedPattern}\")");
                }
                
                // Email validation
                if (prop.Name.Contains("Email", StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine("            .EmailAddress()");
                }
            }
            // Numeric constraints
            else if (IsNumericType(prop.Type) && prop.Constraints != null)
            {
                if (!string.IsNullOrEmpty(prop.Constraints.MinValue))
                {
                    sb.AppendLine($"            .GreaterThanOrEqualTo({prop.Constraints.MinValue})");
                }
                
                if (!string.IsNullOrEmpty(prop.Constraints.MaxValue))
                {
                    sb.AppendLine($"            .LessThanOrEqualTo({prop.Constraints.MaxValue})");
                }
                
                // Precision and scale for decimal
                if (prop.Type == "decimal" && !string.IsNullOrEmpty(prop.Constraints.Precision) && !string.IsNullOrEmpty(prop.Constraints.Scale))
                {
                    sb.AppendLine($"            .PrecisionScale({prop.Constraints.Precision}, {prop.Constraints.Scale}, true)");
                }
            }
            
            // When clause for nullable properties
            if (!prop.IsRequired)
            {
                sb.AppendLine($"            .When(x => x.{prop.Name} != null)");
            }
            
            sb.AppendLine($"            .WithMessage(\"{prop.Name} validation failed.\");");
            sb.AppendLine();
        }
        
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
}
