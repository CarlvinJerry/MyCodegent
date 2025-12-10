namespace MyCodeGent.Templates.Models;

public class EntityModel
{
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public List<PropertyModel> Properties { get; set; } = new();
    public bool HasAuditFields { get; set; } = true;
    public bool HasSoftDelete { get; set; } = true;
    public List<RelationshipModel> Relationships { get; set; } = new();
    public List<string> BusinessKeys { get; set; } = new();
}

public class PropertyModel
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsKey { get; set; }
    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }
    public string? DefaultValue { get; set; }
    public PropertyConstraints? Constraints { get; set; }
}

public class PropertyConstraints
{
    // String constraints
    public string? MinLength { get; set; }
    public string? MaxLength { get; set; }
    public string? RegexPattern { get; set; }
    public string? StringDefaultValue { get; set; }
    
    // Numeric constraints
    public string? MinValue { get; set; }
    public string? MaxValue { get; set; }
    public string? Precision { get; set; }
    public string? Scale { get; set; }
    public string? NumericDefaultValue { get; set; }
    
    // Date constraints
    public string? DateDefaultValue { get; set; }
    
    // Boolean constraints
    public bool BoolDefaultValue { get; set; }
    
    // General constraints
    public bool IsUnique { get; set; }
    public bool IsIndexed { get; set; }
    public bool IsComputed { get; set; }
}

public class RelationshipModel
{
    public string Type { get; set; } = string.Empty; // OneToMany, ManyToOne, OneToOne, ManyToMany
    public string RelatedEntity { get; set; } = string.Empty;
    public int RelatedEntityId { get; set; }
    public string ForeignKeyProperty { get; set; } = string.Empty;
    public string PrincipalKey { get; set; } = string.Empty;
    public string NavigationProperty { get; set; } = string.Empty;
    public string InverseNavigationProperty { get; set; } = string.Empty;
    public string OnDeleteBehavior { get; set; } = string.Empty; // Cascade, SetNull, Restrict, NoAction
    public string JoinTableName { get; set; } = string.Empty;
}
