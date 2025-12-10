namespace MyCodeGent.Web.Models;

public class EntityModel
{
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public List<PropertyModel> Properties { get; set; } = new();
    public bool HasAuditFields { get; set; } = true;
    public bool HasSoftDelete { get; set; } = true;
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
}
