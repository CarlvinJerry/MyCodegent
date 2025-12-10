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
            if (prop.IsRequired)
            {
                sb.AppendLine($"        RuleFor(x => x.{prop.Name})");
                sb.AppendLine("            .NotEmpty()");
                
                if (prop.MaxLength.HasValue)
                {
                    sb.AppendLine($"            .MaximumLength({prop.MaxLength.Value})");
                }
                
                sb.AppendLine($"            .WithMessage(\"{prop.Name} is required.\");");
                sb.AppendLine();
            }
            else if (prop.MaxLength.HasValue)
            {
                sb.AppendLine($"        RuleFor(x => x.{prop.Name})");
                sb.AppendLine($"            .MaximumLength({prop.MaxLength.Value})");
                sb.AppendLine($"            .When(x => x.{prop.Name} != null);");
                sb.AppendLine();
            }
        }
        
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
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
            if (prop.IsRequired)
            {
                sb.AppendLine($"        RuleFor(x => x.{prop.Name})");
                sb.AppendLine("            .NotEmpty()");
                
                if (prop.MaxLength.HasValue)
                {
                    sb.AppendLine($"            .MaximumLength({prop.MaxLength.Value})");
                }
                
                sb.AppendLine($"            .WithMessage(\"{prop.Name} is required.\");");
                sb.AppendLine();
            }
            else if (prop.MaxLength.HasValue)
            {
                sb.AppendLine($"        RuleFor(x => x.{prop.Name})");
                sb.AppendLine($"            .MaximumLength({prop.MaxLength.Value})");
                sb.AppendLine($"            .When(x => x.{prop.Name} != null);");
                sb.AppendLine();
            }
        }
        
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
}
