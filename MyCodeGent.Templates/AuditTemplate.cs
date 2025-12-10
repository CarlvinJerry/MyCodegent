using System.Text;

namespace MyCodeGent.Templates;

public static class AuditTemplate
{
    public static string GenerateAuditLogEntity(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"namespace {rootNamespace}.Domain.Entities;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Tracks all changes made to entities");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public class AuditLog");
        sb.AppendLine("{");
        sb.AppendLine("    public int Id { get; set; }");
        sb.AppendLine("    public string EntityName { get; set; } = string.Empty;");
        sb.AppendLine("    public string EntityId { get; set; } = string.Empty;");
        sb.AppendLine("    public string Action { get; set; } = string.Empty; // Create, Update, Delete");
        sb.AppendLine("    public string? OldValues { get; set; }");
        sb.AppendLine("    public string? NewValues { get; set; }");
        sb.AppendLine("    public string? ChangedProperties { get; set; }");
        sb.AppendLine("    public string UserId { get; set; } = string.Empty;");
        sb.AppendLine("    public string UserName { get; set; } = string.Empty;");
        sb.AppendLine("    public DateTime Timestamp { get; set; }");
        sb.AppendLine("    public string? IpAddress { get; set; }");
        sb.AppendLine("    public string? UserAgent { get; set; }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateAuditLogConfiguration(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
        sb.AppendLine($"using {rootNamespace}.Domain.Entities;");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNamespace}.Infrastructure.Persistence.Configurations;");
        sb.AppendLine();
        sb.AppendLine("public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>");
        sb.AppendLine("{");
        sb.AppendLine("    public void Configure(EntityTypeBuilder<AuditLog> builder)");
        sb.AppendLine("    {");
        sb.AppendLine("        builder.ToTable(\"AuditLogs\");");
        sb.AppendLine();
        sb.AppendLine("        builder.HasKey(x => x.Id);");
        sb.AppendLine();
        sb.AppendLine("        builder.Property(x => x.EntityName)");
        sb.AppendLine("            .IsRequired()");
        sb.AppendLine("            .HasMaxLength(100);");
        sb.AppendLine();
        sb.AppendLine("        builder.Property(x => x.EntityId)");
        sb.AppendLine("            .IsRequired()");
        sb.AppendLine("            .HasMaxLength(50);");
        sb.AppendLine();
        sb.AppendLine("        builder.Property(x => x.Action)");
        sb.AppendLine("            .IsRequired()");
        sb.AppendLine("            .HasMaxLength(50);");
        sb.AppendLine();
        sb.AppendLine("        builder.Property(x => x.UserId)");
        sb.AppendLine("            .IsRequired()");
        sb.AppendLine("            .HasMaxLength(50);");
        sb.AppendLine();
        sb.AppendLine("        builder.Property(x => x.UserName)");
        sb.AppendLine("            .IsRequired()");
        sb.AppendLine("            .HasMaxLength(100);");
        sb.AppendLine();
        sb.AppendLine("        builder.Property(x => x.IpAddress)");
        sb.AppendLine("            .HasMaxLength(50);");
        sb.AppendLine();
        sb.AppendLine("        builder.Property(x => x.UserAgent)");
        sb.AppendLine("            .HasMaxLength(500);");
        sb.AppendLine();
        sb.AppendLine("        // Index for faster queries");
        sb.AppendLine("        builder.HasIndex(x => new { x.EntityName, x.EntityId })");
        sb.AppendLine("            .HasDatabaseName(\"IX_AuditLogs_Entity\");");
        sb.AppendLine();
        sb.AppendLine("        builder.HasIndex(x => x.Timestamp)");
        sb.AppendLine("            .HasDatabaseName(\"IX_AuditLogs_Timestamp\");");
        sb.AppendLine();
        sb.AppendLine("        builder.HasIndex(x => x.UserId)");
        sb.AppendLine("            .HasDatabaseName(\"IX_AuditLogs_UserId\");");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateAuditService(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine("using System.Text.Json;");
        sb.AppendLine($"using {rootNamespace}.Domain.Entities;");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNamespace}.Infrastructure.Services;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Service for tracking entity changes");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public class AuditService");
        sb.AppendLine("{");
        sb.AppendLine("    public static List<AuditLog> GetAuditLogs(DbContext context, string? userId = null, string? userAgent = null, string? ipAddress = null)");
        sb.AppendLine("    {");
        sb.AppendLine("        var auditLogs = new List<AuditLog>();");
        sb.AppendLine("        var entries = context.ChangeTracker.Entries()");
        sb.AppendLine("            .Where(e => e.State == EntityState.Added ||");
        sb.AppendLine("                       e.State == EntityState.Modified ||");
        sb.AppendLine("                       e.State == EntityState.Deleted)");
        sb.AppendLine("            .Where(e => e.Entity.GetType() != typeof(AuditLog)) // Don't audit the audit log itself");
        sb.AppendLine("            .ToList();");
        sb.AppendLine();
        sb.AppendLine("        foreach (var entry in entries)");
        sb.AppendLine("        {");
        sb.AppendLine("            var entityName = entry.Entity.GetType().Name;");
        sb.AppendLine("            var entityId = GetEntityId(entry);");
        sb.AppendLine();
        sb.AppendLine("            var auditLog = new AuditLog");
        sb.AppendLine("            {");
        sb.AppendLine("                EntityName = entityName,");
        sb.AppendLine("                EntityId = entityId,");
        sb.AppendLine("                Action = entry.State.ToString(),");
        sb.AppendLine("                UserId = userId ?? \"System\",");
        sb.AppendLine("                UserName = userId ?? \"System\",");
        sb.AppendLine("                Timestamp = DateTime.UtcNow,");
        sb.AppendLine("                IpAddress = ipAddress,");
        sb.AppendLine("                UserAgent = userAgent");
        sb.AppendLine("            };");
        sb.AppendLine();
        sb.AppendLine("            if (entry.State == EntityState.Added)");
        sb.AppendLine("            {");
        sb.AppendLine("                auditLog.NewValues = SerializeEntity(entry.CurrentValues);");
        sb.AppendLine("            }");
        sb.AppendLine("            else if (entry.State == EntityState.Deleted)");
        sb.AppendLine("            {");
        sb.AppendLine("                auditLog.OldValues = SerializeEntity(entry.OriginalValues);");
        sb.AppendLine("            }");
        sb.AppendLine("            else if (entry.State == EntityState.Modified)");
        sb.AppendLine("            {");
        sb.AppendLine("                auditLog.OldValues = SerializeEntity(entry.OriginalValues);");
        sb.AppendLine("                auditLog.NewValues = SerializeEntity(entry.CurrentValues);");
        sb.AppendLine();
        sb.AppendLine("                var changedProperties = entry.Properties");
        sb.AppendLine("                    .Where(p => p.IsModified)");
        sb.AppendLine("                    .Select(p => p.Metadata.Name)");
        sb.AppendLine("                    .ToList();");
        sb.AppendLine();
        sb.AppendLine("                auditLog.ChangedProperties = string.Join(\", \", changedProperties);");
        sb.AppendLine("            }");
        sb.AppendLine();
        sb.AppendLine("            auditLogs.Add(auditLog);");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        return auditLogs;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    private static string GetEntityId(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)");
        sb.AppendLine("    {");
        sb.AppendLine("        var keyProperty = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());");
        sb.AppendLine("        return keyProperty?.CurrentValue?.ToString() ?? \"Unknown\";");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    private static string SerializeEntity(Microsoft.EntityFrameworkCore.ChangeTracking.PropertyValues values)");
        sb.AppendLine("    {");
        sb.AppendLine("        var dict = new Dictionary<string, object?>();");
        sb.AppendLine();
        sb.AppendLine("        foreach (var property in values.Properties)");
        sb.AppendLine("        {");
        sb.AppendLine("            var value = values[property];");
        sb.AppendLine("            dict[property.Name] = value;");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        return JsonSerializer.Serialize(dict);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateEnhancedDbContextWithAudit(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("// Add this to your ApplicationDbContext SaveChangesAsync method:");
        sb.AppendLine();
        sb.AppendLine("public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)");
        sb.AppendLine("{");
        sb.AppendLine("    // Performance: Disable automatic detection of changes");
        sb.AppendLine("    ChangeTracker.AutoDetectChangesEnabled = false;");
        sb.AppendLine();
        sb.AppendLine("    try");
        sb.AppendLine("    {");
        sb.AppendLine("        // Get audit logs before saving");
        sb.AppendLine("        var auditLogs = AuditService.GetAuditLogs(");
        sb.AppendLine("            this,");
        sb.AppendLine("            userId: _currentUserService?.UserId,");
        sb.AppendLine("            userAgent: _httpContextAccessor?.HttpContext?.Request.Headers[\"User-Agent\"],");
        sb.AppendLine("            ipAddress: _httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString()");
        sb.AppendLine("        );");
        sb.AppendLine();
        sb.AppendLine("        var result = await base.SaveChangesAsync(cancellationToken);");
        sb.AppendLine();
        sb.AppendLine("        // Save audit logs");
        sb.AppendLine("        if (auditLogs.Any())");
        sb.AppendLine("        {");
        sb.AppendLine("            await AuditLogs.AddRangeAsync(auditLogs, cancellationToken);");
        sb.AppendLine("            await base.SaveChangesAsync(cancellationToken);");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        return result;");
        sb.AppendLine("    }");
        sb.AppendLine("    finally");
        sb.AppendLine("    {");
        sb.AppendLine("        ChangeTracker.AutoDetectChangesEnabled = true;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateAuditController(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {rootNamespace}.Application.Common.Interfaces;");
        sb.AppendLine($"using {rootNamespace}.Domain.Entities;");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNamespace}.Api.Controllers;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Audit log queries");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("[ApiController]");
        sb.AppendLine("[Route(\"api/[controller]\")]");
        sb.AppendLine("public class AuditController : ControllerBase");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly IApplicationDbContext _context;");
        sb.AppendLine();
        sb.AppendLine("    public AuditController(IApplicationDbContext context)");
        sb.AppendLine("    {");
        sb.AppendLine("        _context = context;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Get audit logs for a specific entity");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    [HttpGet(\"entity/{entityName}/{entityId}\")]");
        sb.AppendLine("    public async Task<IActionResult> GetEntityAuditLogs(string entityName, string entityId)");
        sb.AppendLine("    {");
        sb.AppendLine("        var logs = await _context.AuditLogs");
        sb.AppendLine("            .Where(x => x.EntityName == entityName && x.EntityId == entityId)");
        sb.AppendLine("            .OrderByDescending(x => x.Timestamp)");
        sb.AppendLine("            .ToListAsync();");
        sb.AppendLine();
        sb.AppendLine("        return Ok(logs);");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Get audit logs by user");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    [HttpGet(\"user/{userId}\")]");
        sb.AppendLine("    public async Task<IActionResult> GetUserAuditLogs(string userId)");
        sb.AppendLine("    {");
        sb.AppendLine("        var logs = await _context.AuditLogs");
        sb.AppendLine("            .Where(x => x.UserId == userId)");
        sb.AppendLine("            .OrderByDescending(x => x.Timestamp)");
        sb.AppendLine("            .Take(100)");
        sb.AppendLine("            .ToListAsync();");
        sb.AppendLine();
        sb.AppendLine("        return Ok(logs);");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Get recent audit logs");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    [HttpGet(\"recent\")]");
        sb.AppendLine("    public async Task<IActionResult> GetRecentAuditLogs([FromQuery] int count = 50)");
        sb.AppendLine("    {");
        sb.AppendLine("        var logs = await _context.AuditLogs");
        sb.AppendLine("            .OrderByDescending(x => x.Timestamp)");
        sb.AppendLine("            .Take(count)");
        sb.AppendLine("            .ToListAsync();");
        sb.AppendLine();
        sb.AppendLine("        return Ok(logs);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
}
