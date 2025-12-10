using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class QueryTemplate
{
    public static string GenerateGetByIdQuery(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        var keyType = keyProp?.Type ?? "int";
        var keyName = keyProp?.Name ?? "Id";
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Queries.Get{entity.Name}ById;");
        sb.AppendLine();
        sb.AppendLine($"public record Get{entity.Name}ByIdQuery({keyType} {keyName}) : IRequest<{entity.Name}Dto?>;");
        
        return sb.ToString();
    }
    
    public static string GenerateGetAllQuery(EntityModel entity)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Queries.GetAll{entity.Name}s;");
        sb.AppendLine();
        sb.AppendLine($"public record GetAll{entity.Name}sQuery : IRequest<List<{entity.Name}Dto>>;");
        
        return sb.ToString();
    }
    
    public static string GenerateGetPagedQuery(EntityModel entity)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Queries.Get{entity.Name}sPaged;");
        sb.AppendLine();
        sb.AppendLine($"public record Get{entity.Name}sPagedQuery(int PageNumber, int PageSize) : IRequest<PagedResult<{entity.Name}Dto>>;");
        
        return sb.ToString();
    }
}
