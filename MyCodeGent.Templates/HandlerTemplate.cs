using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class HandlerTemplate
{
    public static string GenerateCreateHandler(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        var keyType = keyProp?.Type ?? "int";
        var keyName = keyProp?.Name ?? "Id";
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine($"using {entity.Namespace}.Domain.Entities;");
        sb.AppendLine($"using {entity.Namespace}.Application.Common.Interfaces;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Commands.Create{entity.Name};");
        sb.AppendLine();
        sb.AppendLine($"public class Create{entity.Name}CommandHandler : IRequestHandler<Create{entity.Name}Command, {keyType}>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly IApplicationDbContext _context;");
        sb.AppendLine();
        sb.AppendLine($"    public Create{entity.Name}CommandHandler(IApplicationDbContext context)");
        sb.AppendLine("    {");
        sb.AppendLine("        _context = context;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<{keyType}> Handle(Create{entity.Name}Command request, CancellationToken cancellationToken)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var entity = new {entity.Name}");
        sb.AppendLine("        {");
        
        foreach (var prop in entity.Properties.Where(p => !p.IsKey))
        {
            sb.AppendLine($"            {prop.Name} = request.{prop.Name},");
        }
        
        if (entity.HasAuditFields)
        {
            sb.AppendLine("            CreatedAt = DateTime.UtcNow");
        }
        
        sb.AppendLine("        };");
        sb.AppendLine();
        sb.AppendLine($"        _context.{entity.Name}s.Add(entity);");
        sb.AppendLine("        await _context.SaveChangesAsync(cancellationToken);");
        sb.AppendLine();
        sb.AppendLine($"        return entity.{keyName};");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateUpdateHandler(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        var keyName = keyProp?.Name ?? "Id";
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {entity.Namespace}.Application.Common.Interfaces;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Commands.Update{entity.Name};");
        sb.AppendLine();
        sb.AppendLine($"public class Update{entity.Name}CommandHandler : IRequestHandler<Update{entity.Name}Command, bool>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly IApplicationDbContext _context;");
        sb.AppendLine();
        sb.AppendLine($"    public Update{entity.Name}CommandHandler(IApplicationDbContext context)");
        sb.AppendLine("    {");
        sb.AppendLine("        _context = context;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<bool> Handle(Update{entity.Name}Command request, CancellationToken cancellationToken)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var entity = await _context.{entity.Name}s");
        sb.AppendLine($"            .FirstOrDefaultAsync(x => x.{keyName} == request.{keyName}, cancellationToken);");
        sb.AppendLine();
        sb.AppendLine("        if (entity == null) return false;");
        sb.AppendLine();
        
        foreach (var prop in entity.Properties.Where(p => !p.IsKey))
        {
            sb.AppendLine($"        entity.{prop.Name} = request.{prop.Name};");
        }
        
        if (entity.HasAuditFields)
        {
            sb.AppendLine("        entity.UpdatedAt = DateTime.UtcNow;");
        }
        
        sb.AppendLine();
        sb.AppendLine("        await _context.SaveChangesAsync(cancellationToken);");
        sb.AppendLine("        return true;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateDeleteHandler(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        var keyName = keyProp?.Name ?? "Id";
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {entity.Namespace}.Application.Common.Interfaces;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Commands.Delete{entity.Name};");
        sb.AppendLine();
        sb.AppendLine($"public class Delete{entity.Name}CommandHandler : IRequestHandler<Delete{entity.Name}Command, bool>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly IApplicationDbContext _context;");
        sb.AppendLine();
        sb.AppendLine($"    public Delete{entity.Name}CommandHandler(IApplicationDbContext context)");
        sb.AppendLine("    {");
        sb.AppendLine("        _context = context;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<bool> Handle(Delete{entity.Name}Command request, CancellationToken cancellationToken)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var entity = await _context.{entity.Name}s");
        sb.AppendLine($"            .FirstOrDefaultAsync(x => x.{keyName} == request.{keyName}, cancellationToken);");
        sb.AppendLine();
        sb.AppendLine("        if (entity == null) return false;");
        sb.AppendLine();
        
        if (entity.HasSoftDelete)
        {
            sb.AppendLine("        entity.IsDeleted = true;");
            sb.AppendLine("        entity.DeletedAt = DateTime.UtcNow;");
        }
        else
        {
            sb.AppendLine($"        _context.{entity.Name}s.Remove(entity);");
        }
        
        sb.AppendLine("        await _context.SaveChangesAsync(cancellationToken);");
        sb.AppendLine("        return true;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateGetByIdHandler(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        var keyName = keyProp?.Name ?? "Id";
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {entity.Namespace}.Application.Common.Interfaces;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Queries.Get{entity.Name}ById;");
        sb.AppendLine();
        sb.AppendLine($"public class Get{entity.Name}ByIdQueryHandler : IRequestHandler<Get{entity.Name}ByIdQuery, {entity.Name}Dto?>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly IApplicationDbContext _context;");
        sb.AppendLine();
        sb.AppendLine($"    public Get{entity.Name}ByIdQueryHandler(IApplicationDbContext context)");
        sb.AppendLine("    {");
        sb.AppendLine("        _context = context;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<{entity.Name}Dto?> Handle(Get{entity.Name}ByIdQuery request, CancellationToken cancellationToken)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var entity = await _context.{entity.Name}s");
        sb.AppendLine($"            .Where(x => x.{keyName} == request.{keyName})");
        
        if (entity.HasSoftDelete)
        {
            sb.AppendLine("            .Where(x => !x.IsDeleted)");
        }
        
        sb.AppendLine("            .Select(x => new " + entity.Name + "Dto");
        sb.AppendLine("            {");
        
        foreach (var prop in entity.Properties)
        {
            sb.AppendLine($"                {prop.Name} = x.{prop.Name},");
        }
        
        sb.AppendLine("            })");
        sb.AppendLine("            .FirstOrDefaultAsync(cancellationToken);");
        sb.AppendLine();
        sb.AppendLine("        return entity;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateGetAllHandler(EntityModel entity)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {entity.Namespace}.Application.Common.Interfaces;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.{entity.Name}s.Queries.GetAll{entity.Name}s;");
        sb.AppendLine();
        sb.AppendLine($"public class GetAll{entity.Name}sQueryHandler : IRequestHandler<GetAll{entity.Name}sQuery, List<{entity.Name}Dto>>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly IApplicationDbContext _context;");
        sb.AppendLine();
        sb.AppendLine($"    public GetAll{entity.Name}sQueryHandler(IApplicationDbContext context)");
        sb.AppendLine("    {");
        sb.AppendLine("        _context = context;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<List<{entity.Name}Dto>> Handle(GetAll{entity.Name}sQuery request, CancellationToken cancellationToken)");
        sb.AppendLine("    {");
        sb.AppendLine($"        return await _context.{entity.Name}s");
        
        if (entity.HasSoftDelete)
        {
            sb.AppendLine("            .Where(x => !x.IsDeleted)");
        }
        
        sb.AppendLine("            .Select(x => new " + entity.Name + "Dto");
        sb.AppendLine("            {");
        
        foreach (var prop in entity.Properties)
        {
            sb.AppendLine($"                {prop.Name} = x.{prop.Name},");
        }
        
        sb.AppendLine("            })");
        sb.AppendLine("            .ToListAsync(cancellationToken);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
}
