using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class MappingProfileTemplate
{
    public static string Generate(EntityModel entity)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using AutoMapper;");
        sb.AppendLine($"using {entity.Namespace}.Domain.Entities;");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s;");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Commands.Create{entity.Name};");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Commands.Update{entity.Name};");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.Mappings;");
        sb.AppendLine();
        sb.AppendLine($"public class {entity.Name}MappingProfile : Profile");
        sb.AppendLine("{");
        sb.AppendLine($"    public {entity.Name}MappingProfile()");
        sb.AppendLine("    {");
        
        // Entity to Dto
        sb.AppendLine($"        // Entity to Dto");
        sb.AppendLine($"        CreateMap<{entity.Name}, {entity.Name}Dto>();");
        sb.AppendLine();
        
        // Dto to Entity
        sb.AppendLine($"        // Dto to Entity");
        sb.AppendLine($"        CreateMap<{entity.Name}Dto, {entity.Name}>();");
        sb.AppendLine();
        
        // Create Command to Entity
        sb.AppendLine($"        // Create Command to Entity");
        sb.AppendLine($"        CreateMap<Create{entity.Name}Command, {entity.Name}>();");
        sb.AppendLine();
        
        // Update Command to Entity
        sb.AppendLine($"        // Update Command to Entity");
        sb.AppendLine($"        CreateMap<Update{entity.Name}Command, {entity.Name}>();");
        sb.AppendLine();
        
        // Entity to Create Command (for reverse mapping if needed)
        sb.AppendLine($"        // Entity to Commands (for reverse operations)");
        sb.AppendLine($"        CreateMap<{entity.Name}, Create{entity.Name}Command>();");
        sb.AppendLine($"        CreateMap<{entity.Name}, Update{entity.Name}Command>();");
        
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateMasterProfile(List<EntityModel> entities, string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using AutoMapper;");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNamespace}.Application.Mappings;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Master AutoMapper profile - AutoMapper will automatically discover all Profile classes");
        sb.AppendLine("/// This class is optional but can be used for global configuration");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public class MappingProfile : Profile");
        sb.AppendLine("{");
        sb.AppendLine("    public MappingProfile()");
        sb.AppendLine("    {");
        sb.AppendLine("        // AutoMapper will automatically discover and load all Profile classes");
        sb.AppendLine("        // in the assembly, so individual profiles don't need to be explicitly included.");
        sb.AppendLine("        // ");
        sb.AppendLine("        // Individual entity mapping profiles:");
        
        foreach (var entity in entities)
        {
            sb.AppendLine($"        // - {entity.Name}MappingProfile");
        }
        
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
}
