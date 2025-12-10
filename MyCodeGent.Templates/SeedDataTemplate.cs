using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class SeedDataTemplate
{
    public static string Generate(List<EntityModel> entities, string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {rootNamespace}.Domain.Entities;");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNamespace}.Infrastructure.Persistence;");
        sb.AppendLine();
        sb.AppendLine("public static class ApplicationDbContextSeed");
        sb.AppendLine("{");
        sb.AppendLine("    public static void SeedData(ModelBuilder modelBuilder)");
        sb.AppendLine("    {");
        
        // Sort entities by dependency (entities with no relationships first)
        var sortedEntities = TopologicalSort(entities);
        
        foreach (var entity in sortedEntities)
        {
            GenerateEntitySeedData(sb, entity, entities);
        }
        
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    private static void GenerateEntitySeedData(StringBuilder sb, EntityModel entity, List<EntityModel> allEntities)
    {
        sb.AppendLine();
        sb.AppendLine($"        // Seed data for {entity.Name}");
        sb.AppendLine($"        modelBuilder.Entity<{entity.Name}>().HasData(");
        
        // Generate 3-5 sample records
        int recordCount = 3;
        for (int i = 1; i <= recordCount; i++)
        {
            sb.Append($"            new {entity.Name} {{ ");
            
            var properties = new List<string>();
            
            foreach (var prop in entity.Properties)
            {
                var value = GeneratePropertyValue(prop, i, entity, allEntities);
                if (value != null)
                {
                    properties.Add($"{prop.Name} = {value}");
                }
            }
            
            // Add audit fields if enabled
            if (entity.HasAuditFields)
            {
                properties.Add($"CreatedAt = DateTime.UtcNow.AddDays(-{30 - i})");
                properties.Add("CreatedBy = \"System\"");
            }
            
            // Add soft delete fields if enabled
            if (entity.HasSoftDelete)
            {
                properties.Add("IsDeleted = false");
            }
            
            sb.Append(string.Join(", ", properties));
            sb.Append(" }");
            
            if (i < recordCount)
            {
                sb.AppendLine(",");
            }
            else
            {
                sb.AppendLine();
            }
        }
        
        sb.AppendLine("        );");
    }
    
    private static string? GeneratePropertyValue(PropertyModel prop, int index, EntityModel currentEntity, List<EntityModel> allEntities)
    {
        // Skip navigation properties
        if (prop.Name.EndsWith("Navigation") || prop.Name.Contains("Collection"))
        {
            return null;
        }
        
        // Primary key
        if (prop.IsKey)
        {
            return index.ToString();
        }
        
        // Foreign key - check if this property is a foreign key
        var relationship = currentEntity.Relationships?.FirstOrDefault(r => 
            r.ForeignKeyProperty == prop.Name);
        
        if (relationship != null)
        {
            // Reference an existing entity (use index to create valid references)
            return index.ToString();
        }
        
        // Generate value based on type and name
        return prop.Type switch
        {
            "string" => GenerateStringValue(prop.Name, index, prop.MaxLength),
            "int" => GenerateIntValue(prop.Name, index, prop.Constraints),
            "long" => GenerateLongValue(prop.Name, index),
            "decimal" => GenerateDecimalValue(prop.Name, index, prop.Constraints),
            "double" => GenerateDoubleValue(prop.Name, index),
            "float" => $"{index * 10.5}f",
            "bool" => (index % 2 == 0).ToString().ToLower(),
            "DateTime" => GenerateDateTimeValue(prop.Name, index),
            "DateTimeOffset" => $"DateTimeOffset.UtcNow.AddDays(-{index})",
            "Guid" => $"Guid.Parse(\"{Guid.NewGuid()}\")",
            _ => prop.IsRequired ? "default" : "null"
        };
    }
    
    private static string GenerateStringValue(string propName, int index, int? maxLength)
    {
        var name = propName.ToLower();
        
        // Email
        if (name.Contains("email"))
        {
            return $"\"user{index}@example.com\"";
        }
        
        // Username
        if (name.Contains("username") || name == "name")
        {
            return $"\"User{index}\"";
        }
        
        // First Name
        if (name.Contains("firstname"))
        {
            var names = new[] { "John", "Jane", "Bob", "Alice", "Charlie" };
            return $"\"{names[index % names.Length]}\"";
        }
        
        // Last Name
        if (name.Contains("lastname"))
        {
            var names = new[] { "Doe", "Smith", "Johnson", "Williams", "Brown" };
            return $"\"{names[index % names.Length]}\"";
        }
        
        // Phone
        if (name.Contains("phone"))
        {
            return $"\"+1-555-{1000 + index:D4}\"";
        }
        
        // Address
        if (name.Contains("address"))
        {
            return $"\"{index * 100} Main Street, City, State {10000 + index}\"";
        }
        
        // Description
        if (name.Contains("description") || name.Contains("notes"))
        {
            return $"\"Sample description for {propName} {index}\"";
        }
        
        // Title
        if (name.Contains("title"))
        {
            return $"\"Title {index}\"";
        }
        
        // Code/SKU
        if (name.Contains("code") || name.Contains("sku"))
        {
            return $"\"CODE-{index:D4}\"";
        }
        
        // URL/Slug
        if (name.Contains("url") || name.Contains("slug"))
        {
            return $"\"item-{index}\"";
        }
        
        // Status
        if (name.Contains("status"))
        {
            var statuses = new[] { "Active", "Pending", "Completed", "Cancelled" };
            return $"\"{statuses[index % statuses.Length]}\"";
        }
        
        // Generic string
        var maxLen = maxLength ?? 50;
        var value = $"{propName} {index}";
        if (value.Length > maxLen)
        {
            value = value.Substring(0, maxLen);
        }
        return $"\"{value}\"";
    }
    
    private static string GenerateIntValue(string propName, int index, PropertyConstraints? constraints)
    {
        var name = propName.ToLower();
        
        // Age
        if (name.Contains("age"))
        {
            return (20 + index * 5).ToString();
        }
        
        // Quantity/Stock
        if (name.Contains("quantity") || name.Contains("stock"))
        {
            return (100 + index * 10).ToString();
        }
        
        // Count
        if (name.Contains("count"))
        {
            return (index * 5).ToString();
        }
        
        // Year
        if (name.Contains("year"))
        {
            return (2020 + index).ToString();
        }
        
        // Apply constraints if present
        if (constraints != null)
        {
            if (!string.IsNullOrEmpty(constraints.MinValue))
            {
                var min = int.Parse(constraints.MinValue);
                return Math.Max(min, index * 10).ToString();
            }
        }
        
        return (index * 10).ToString();
    }
    
    private static string GenerateLongValue(string propName, int index)
    {
        return $"{index * 1000}L";
    }
    
    private static string GenerateDecimalValue(string propName, int index, PropertyConstraints? constraints)
    {
        var name = propName.ToLower();
        
        // Price
        if (name.Contains("price") || name.Contains("amount") || name.Contains("cost"))
        {
            var price = 10.00m + (index * 5.50m);
            
            // Apply min value constraint
            if (constraints != null && !string.IsNullOrEmpty(constraints.MinValue))
            {
                var min = decimal.Parse(constraints.MinValue);
                price = Math.Max(min, price);
            }
            
            return $"{price}m";
        }
        
        // Rate/Percentage
        if (name.Contains("rate") || name.Contains("percentage"))
        {
            return $"{index * 2.5}m";
        }
        
        return $"{index * 10.5}m";
    }
    
    private static string GenerateDoubleValue(string propName, int index)
    {
        var name = propName.ToLower();
        
        // Latitude
        if (name.Contains("latitude") || name.Contains("lat"))
        {
            return $"{40.0 + index * 0.1}";
        }
        
        // Longitude
        if (name.Contains("longitude") || name.Contains("lon") || name.Contains("lng"))
        {
            return $"{-74.0 + index * 0.1}";
        }
        
        return $"{index * 10.5}";
    }
    
    private static string GenerateDateTimeValue(string propName, int index)
    {
        var name = propName.ToLower();
        
        // Birth date
        if (name.Contains("birth"))
        {
            return $"new DateTime(1990 + {index}, 1, 1)";
        }
        
        // Published/Created date
        if (name.Contains("published") || name.Contains("created"))
        {
            return $"DateTime.UtcNow.AddDays(-{30 - index})";
        }
        
        // Updated date
        if (name.Contains("updated") || name.Contains("modified"))
        {
            return $"DateTime.UtcNow.AddDays(-{index})";
        }
        
        // Expiry/End date
        if (name.Contains("expiry") || name.Contains("end") || name.Contains("expires"))
        {
            return $"DateTime.UtcNow.AddDays({30 + index})";
        }
        
        // Generic date
        return $"DateTime.UtcNow.AddDays(-{index})";
    }
    
    private static List<EntityModel> TopologicalSort(List<EntityModel> entities)
    {
        var sorted = new List<EntityModel>();
        var visited = new HashSet<string>();
        var visiting = new HashSet<string>();
        
        foreach (var entity in entities)
        {
            Visit(entity, entities, visited, visiting, sorted);
        }
        
        return sorted;
    }
    
    private static void Visit(EntityModel entity, List<EntityModel> allEntities, 
        HashSet<string> visited, HashSet<string> visiting, List<EntityModel> sorted)
    {
        if (visited.Contains(entity.Name))
        {
            return;
        }
        
        if (visiting.Contains(entity.Name))
        {
            // Circular dependency - just add it
            sorted.Add(entity);
            visited.Add(entity.Name);
            return;
        }
        
        visiting.Add(entity.Name);
        
        // Visit dependencies first (entities this entity references)
        if (entity.Relationships != null)
        {
            foreach (var relationship in entity.Relationships)
            {
                // For ManyToOne and OneToOne, we need the related entity first
                if (relationship.Type == "ManyToOne" || relationship.Type == "OneToOne")
                {
                    var relatedEntity = allEntities.FirstOrDefault(e => e.Name == relationship.RelatedEntity);
                    if (relatedEntity != null && relatedEntity.Name != entity.Name)
                    {
                        Visit(relatedEntity, allEntities, visited, visiting, sorted);
                    }
                }
            }
        }
        
        visiting.Remove(entity.Name);
        visited.Add(entity.Name);
        sorted.Add(entity);
    }
}
