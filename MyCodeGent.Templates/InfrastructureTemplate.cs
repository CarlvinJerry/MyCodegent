using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class InfrastructureTemplate
{
    public static string GenerateDbContext(List<EntityModel> entities, string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {rootNamespace}.Domain.Entities;");
        sb.AppendLine($"using {rootNamespace}.Application.Common.Interfaces;");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNamespace}.Infrastructure.Persistence;");
        sb.AppendLine();
        sb.AppendLine("public class ApplicationDbContext : DbContext, IApplicationDbContext");
        sb.AppendLine("{");
        sb.AppendLine("    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)");
        sb.AppendLine("        : base(options)");
        sb.AppendLine("    {");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        foreach (var entity in entities)
        {
            sb.AppendLine($"    public DbSet<{entity.Name}> {entity.Name}s => Set<{entity.Name}>();");
        }
        
        sb.AppendLine();
        sb.AppendLine("    protected override void OnModelCreating(ModelBuilder modelBuilder)");
        sb.AppendLine("    {");
        sb.AppendLine("        base.OnModelCreating(modelBuilder);");
        sb.AppendLine();
        
        foreach (var entity in entities)
        {
            sb.AppendLine($"        modelBuilder.ApplyConfiguration(new {entity.Name}Configuration());");
        }
        
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)");
        sb.AppendLine("    {");
        sb.AppendLine("        return await base.SaveChangesAsync(cancellationToken);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateEntityConfiguration(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
        sb.AppendLine($"using {entity.Namespace}.Domain.Entities;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Infrastructure.Persistence.Configurations;");
        sb.AppendLine();
        sb.AppendLine($"public class {entity.Name}Configuration : IEntityTypeConfiguration<{entity.Name}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public void Configure(EntityTypeBuilder<{entity.Name}> builder)");
        sb.AppendLine("    {");
        sb.AppendLine($"        builder.ToTable(\"{entity.Name}s\");");
        sb.AppendLine();
        
        if (keyProp != null)
        {
            sb.AppendLine($"        builder.HasKey(x => x.{keyProp.Name});");
            sb.AppendLine();
        }
        
        foreach (var prop in entity.Properties)
        {
            if (prop.IsRequired && !prop.IsNullable)
            {
                sb.AppendLine($"        builder.Property(x => x.{prop.Name})");
                sb.AppendLine("            .IsRequired();");
                sb.AppendLine();
            }
            
            if (prop.MaxLength.HasValue)
            {
                sb.AppendLine($"        builder.Property(x => x.{prop.Name})");
                sb.AppendLine($"            .HasMaxLength({prop.MaxLength.Value});");
                sb.AppendLine();
            }
        }
        
        if (entity.HasSoftDelete)
        {
            sb.AppendLine("        builder.HasQueryFilter(x => !x.IsDeleted);");
        }
        
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateApplicationDbContextInterface(List<EntityModel> entities, string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {rootNamespace}.Domain.Entities;");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNamespace}.Application.Common.Interfaces;");
        sb.AppendLine();
        sb.AppendLine("public interface IApplicationDbContext");
        sb.AppendLine("{");
        
        foreach (var entity in entities)
        {
            sb.AppendLine($"    DbSet<{entity.Name}> {entity.Name}s {{ get; }}");
        }
        
        sb.AppendLine();
        sb.AppendLine("    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GeneratePagedResult(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"namespace {rootNamespace}.Application.Common.Models;");
        sb.AppendLine();
        sb.AppendLine("public class PagedResult<T>");
        sb.AppendLine("{");
        sb.AppendLine("    public List<T> Items { get; set; } = new();");
        sb.AppendLine("    public int PageNumber { get; set; }");
        sb.AppendLine("    public int PageSize { get; set; }");
        sb.AppendLine("    public int TotalCount { get; set; }");
        sb.AppendLine("    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);");
        sb.AppendLine("    public bool HasPreviousPage => PageNumber > 1;");
        sb.AppendLine("    public bool HasNextPage => PageNumber < TotalPages;");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
}
