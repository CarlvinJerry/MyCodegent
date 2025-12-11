using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class PaginationTemplate
{
    public static string GeneratePagedResult(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"namespace {rootNamespace}.Application.Common.Models;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Represents a paginated result set");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public class PagedResult<T>");
        sb.AppendLine("{");
        sb.AppendLine("    public List<T> Items { get; set; } = new();");
        sb.AppendLine("    public int Page { get; set; }");
        sb.AppendLine("    public int PageSize { get; set; }");
        sb.AppendLine("    public int TotalCount { get; set; }");
        sb.AppendLine("    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);");
        sb.AppendLine("    public bool HasPreviousPage => Page > 1;");
        sb.AppendLine("    public bool HasNextPage => Page < TotalPages;");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GeneratePagedQuery(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"namespace {rootNamespace}.Application.Common.Models;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Base class for paginated queries");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public abstract class PagedQuery");
        sb.AppendLine("{");
        sb.AppendLine("    public int Page { get; set; } = 1;");
        sb.AppendLine("    public int PageSize { get; set; } = 10;");
        sb.AppendLine("    public string? SortBy { get; set; }");
        sb.AppendLine("    public bool Descending { get; set; } = false;");
        sb.AppendLine("    ");
        sb.AppendLine("    public int Skip => (Page - 1) * PageSize;");
        sb.AppendLine("    public int Take => PageSize;");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateGetAllPagedQuery(EntityModel entity)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine($"using {entity.Namespace}.Application.Common.Models;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Queries.GetAll{entity.Name}sPaged;");
        sb.AppendLine();
        sb.AppendLine($"public class GetAll{entity.Name}sPagedQuery : PagedQuery, IRequest<PagedResult<{entity.Name}Dto>>");
        sb.AppendLine("{");
        sb.AppendLine("    public string? SearchTerm { get; set; }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateGetAllPagedHandler(EntityModel entity)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {entity.Namespace}.Application.Common.Interfaces;");
        sb.AppendLine($"using {entity.Namespace}.Application.Common.Models;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Queries.GetAll{entity.Name}sPaged;");
        sb.AppendLine();
        sb.AppendLine($"public class GetAll{entity.Name}sPagedQueryHandler : IRequestHandler<GetAll{entity.Name}sPagedQuery, PagedResult<{entity.Name}Dto>>");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly IApplicationDbContext _context;");
        sb.AppendLine();
        sb.AppendLine($"    public GetAll{entity.Name}sPagedQueryHandler(IApplicationDbContext context)");
        sb.AppendLine("    {");
        sb.AppendLine("        _context = context;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<PagedResult<{entity.Name}Dto>> Handle(GetAll{entity.Name}sPagedQuery request, CancellationToken cancellationToken)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var query = _context.{entity.Name}s.AsNoTracking();");
        sb.AppendLine();
        
        // Add soft delete filter
        if (entity.HasSoftDelete)
        {
            sb.AppendLine("        query = query.Where(x => !x.IsDeleted);");
            sb.AppendLine();
        }
        
        // Add search functionality
        sb.AppendLine("        // Apply search filter");
        sb.AppendLine("        if (!string.IsNullOrWhiteSpace(request.SearchTerm))");
        sb.AppendLine("        {");
        sb.AppendLine("            var searchTerm = request.SearchTerm.ToLower();");
        sb.AppendLine("            query = query.Where(x => ");
        
        // Add search across string properties
        var stringProps = entity.Properties.Where(p => p.Type == "string").ToList();
        if (stringProps.Any())
        {
            for (int i = 0; i < stringProps.Count; i++)
            {
                var prop = stringProps[i];
                if (i == 0)
                    sb.AppendLine($"                x.{prop.Name}.ToLower().Contains(searchTerm)");
                else
                    sb.AppendLine($"                || x.{prop.Name}.ToLower().Contains(searchTerm)");
            }
        }
        else
        {
            sb.AppendLine("                false // No searchable string properties");
        }
        
        sb.AppendLine("            );");
        sb.AppendLine("        }");
        sb.AppendLine();
        
        // Get total count
        sb.AppendLine("        var totalCount = await query.CountAsync(cancellationToken);");
        sb.AppendLine();
        
        // Apply sorting
        sb.AppendLine("        // Apply sorting");
        sb.AppendLine("        if (!string.IsNullOrWhiteSpace(request.SortBy))");
        sb.AppendLine("        {");
        sb.AppendLine("            query = request.SortBy.ToLower() switch");
        sb.AppendLine("            {");
        
        foreach (var prop in entity.Properties.Take(5))
        {
            sb.AppendLine($"                \"{prop.Name.ToLower()}\" => request.Descending ? query.OrderByDescending(x => x.{prop.Name}) : query.OrderBy(x => x.{prop.Name}),");
        }
        
        sb.AppendLine("                _ => query");
        sb.AppendLine("            };");
        sb.AppendLine("        }");
        sb.AppendLine();
        
        // Apply pagination and projection
        sb.AppendLine("        var items = await query");
        sb.AppendLine("            .Skip(request.Skip)");
        sb.AppendLine("            .Take(request.Take)");
        sb.AppendLine($"            .Select(x => new {entity.Name}Dto");
        sb.AppendLine("            {");
        
        foreach (var prop in entity.Properties)
        {
            sb.AppendLine($"                {prop.Name} = x.{prop.Name},");
        }
        
        sb.AppendLine("            })");
        sb.AppendLine("            .ToListAsync(cancellationToken);");
        sb.AppendLine();
        sb.AppendLine($"        return new PagedResult<{entity.Name}Dto>");
        sb.AppendLine("        {");
        sb.AppendLine("            Items = items,");
        sb.AppendLine("            PageNumber = request.Page,");
        sb.AppendLine("            PageSize = request.PageSize,");
        sb.AppendLine("            TotalCount = totalCount");
        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GeneratePagedControllerEndpoint(EntityModel entity)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Gets paginated list of {entity.Name}s with optional search and sorting");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    [HttpGet(\"paged\")]");
        sb.AppendLine("    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { \"page\", \"pageSize\", \"searchTerm\", \"sortBy\" })]");
        sb.AppendLine($"    public async Task<ActionResult<PagedResult<{entity.Name}Dto>>> GetAllPaged(");
        sb.AppendLine("        [FromQuery] int page = 1,");
        sb.AppendLine("        [FromQuery] int pageSize = 10,");
        sb.AppendLine("        [FromQuery] string? searchTerm = null,");
        sb.AppendLine("        [FromQuery] string? sortBy = null,");
        sb.AppendLine("        [FromQuery] bool descending = false)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var result = await _mediator.Send(new GetAll{entity.Name}sPagedQuery");
        sb.AppendLine("        {");
        sb.AppendLine("            Page = page,");
        sb.AppendLine("            PageSize = pageSize,");
        sb.AppendLine("            SearchTerm = searchTerm,");
        sb.AppendLine("            SortBy = sortBy,");
        sb.AppendLine("            Descending = descending");
        sb.AppendLine("        });");
        sb.AppendLine("        return Ok(result);");
        sb.AppendLine("    }");
        
        return sb.ToString();
    }
}
