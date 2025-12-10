# Phase 5: Backend Integration - Complete Guide

## ✅ **Frontend Complete - Backend Ready**

All UI features are now complete and sending comprehensive data to the backend. This document outlines what the backend receives and how to process it.

---

## 📊 **Complete Data Structure**

### **Entity Payload Structure**

```json
{
  "config": {
    "rootNamespace": "MyApp",
    "databaseProvider": "SqlServer",
    "connectionString": "...",
    // ... other config
  },
  "entities": [
    {
      "name": "Customer",
      "hasAuditFields": true,
      "hasSoftDelete": true,
      "properties": [
        {
          "name": "Email",
          "type": "string",
          "isKey": false,
          "isRequired": true,
          "maxLength": 100,
          "constraints": {
            "minLength": "5",
            "maxLength": "100",
            "regexPattern": "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$",
            "stringDefaultValue": null,
            "isUnique": true,
            "isIndexed": true,
            "isComputed": false
          }
        },
        {
          "name": "Age",
          "type": "int",
          "isKey": false,
          "isRequired": false,
          "constraints": {
            "minValue": "18",
            "maxValue": "120",
            "numericDefaultValue": "0",
            "isUnique": false,
            "isIndexed": false
          }
        }
      ],
      "relationships": [
        {
          "type": "OneToMany",
          "relatedEntity": "Order",
          "relatedEntityId": 2,
          "foreignKeyProperty": "CustomerId",
          "principalKey": "Id",
          "navigationProperty": "Orders",
          "inverseNavigationProperty": "Customer",
          "onDeleteBehavior": "Cascade",
          "joinTableName": ""
        }
      ],
      "businessKeys": ["Email"]
    }
  ]
}
```

---

## 🔧 **Backend Processing Requirements**

### **1. Property Constraints → EF Core Configuration**

#### **String Constraints**

```csharp
// Frontend sends:
{
  "minLength": "5",
  "maxLength": "100",
  "regexPattern": "^[a-zA-Z0-9._%+-]+@...",
  "isUnique": true,
  "isIndexed": true
}

// Backend generates:
entity.Property(e => e.Email)
    .HasMaxLength(100)
    .IsRequired()
    .HasAnnotation("MinLength", 5)
    .HasAnnotation("RegularExpression", "^[a-zA-Z0-9._%+-]+@...");

entity.HasIndex(e => e.Email)
    .IsUnique()
    .HasDatabaseName("IX_Customer_Email");
```

#### **Numeric Constraints**

```csharp
// Frontend sends:
{
  "minValue": "0",
  "maxValue": "999.99",
  "precision": "18",
  "scale": "2"
}

// Backend generates:
entity.Property(e => e.Price)
    .HasPrecision(18, 2)
    .HasAnnotation("Range", "0-999.99");
```

#### **Date Constraints**

```csharp
// Frontend sends:
{
  "dateDefaultValue": "DateTime.UtcNow"
}

// Backend generates:
entity.Property(e => e.CreatedAt)
    .HasDefaultValueSql("GETUTCDATE()");  // SQL Server
    // or
    .HasDefaultValueSql("NOW()");  // PostgreSQL
```

---

### **2. Relationships → EF Core Configuration**

#### **One-to-Many**

```csharp
// Frontend sends:
{
  "type": "OneToMany",
  "relatedEntity": "Order",
  "foreignKeyProperty": "CustomerId",
  "navigationProperty": "Orders",
  "inverseNavigationProperty": "Customer",
  "onDeleteBehavior": "Cascade"
}

// Backend generates:
entity.HasMany(e => e.Orders)
    .WithOne(o => o.Customer)
    .HasForeignKey(o => o.CustomerId)
    .OnDelete(DeleteBehavior.Cascade);
```

#### **Many-to-One**

```csharp
// Frontend sends:
{
  "type": "ManyToOne",
  "relatedEntity": "Customer",
  "foreignKeyProperty": "CustomerId",
  "navigationProperty": "Customer",
  "inverseNavigationProperty": "Orders"
}

// Backend generates:
entity.HasOne(e => e.Customer)
    .WithMany(c => c.Orders)
    .HasForeignKey(e => e.CustomerId)
    .OnDelete(DeleteBehavior.Cascade);
```

#### **One-to-One**

```csharp
// Frontend sends:
{
  "type": "OneToOne",
  "relatedEntity": "Profile",
  "foreignKeyProperty": "UserId",
  "navigationProperty": "Profile",
  "inverseNavigationProperty": "User"
}

// Backend generates:
entity.HasOne(e => e.Profile)
    .WithOne(p => p.User)
    .HasForeignKey<Profile>(p => p.UserId)
    .OnDelete(DeleteBehavior.Cascade);
```

#### **Many-to-Many**

```csharp
// Frontend sends:
{
  "type": "ManyToMany",
  "relatedEntity": "Course",
  "navigationProperty": "Courses",
  "inverseNavigationProperty": "Students",
  "joinTableName": "StudentCourse"
}

// Backend generates:
entity.HasMany(e => e.Courses)
    .WithMany(c => c.Students)
    .UsingEntity(
        "StudentCourse",
        l => l.HasOne(typeof(Course)).WithMany().HasForeignKey("CourseId"),
        r => r.HasOne(typeof(Student)).WithMany().HasForeignKey("StudentId")
    );
```

---

### **3. Business Keys → Unique Indexes & Lookup Methods**

#### **Single Business Key**

```csharp
// Frontend sends:
{
  "businessKeys": ["Email"]
}

// Backend generates:

// 1. Unique Index in DbContext
entity.HasIndex(e => e.Email)
    .IsUnique()
    .HasDatabaseName("IX_Customer_Email_BusinessKey");

// 2. Repository Method
public async Task<Customer> GetByEmailAsync(string email)
{
    return await _context.Customers
        .FirstOrDefaultAsync(c => c.Email == email);
}

// 3. CQRS Query
public class GetCustomerByEmailQuery : IRequest<CustomerDto>
{
    public string Email { get; set; }
}
```

#### **Composite Business Key**

```csharp
// Frontend sends:
{
  "businessKeys": ["FirstName", "LastName"]
}

// Backend generates:

// 1. Composite Unique Index
entity.HasIndex(e => new { e.FirstName, e.LastName })
    .IsUnique()
    .HasDatabaseName("IX_Customer_FirstName_LastName_BusinessKey");

// 2. Repository Method
public async Task<Customer> GetByFirstNameAndLastNameAsync(
    string firstName, 
    string lastName)
{
    return await _context.Customers
        .FirstOrDefaultAsync(c => 
            c.FirstName == firstName && 
            c.LastName == lastName);
}
```

---

## 🎯 **FluentValidation Integration**

### **Generate Validators from Constraints**

```csharp
// Frontend sends constraints, backend generates:

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        // From minLength/maxLength
        RuleFor(x => x.Email)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(100)
            .EmailAddress();  // From regexPattern

        // From minValue/maxValue
        RuleFor(x => x.Age)
            .InclusiveBetween(18, 120);

        // From isRequired
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull();
    }
}
```

---

## 📋 **Migration Generation**

### **Complete Migration Example**

```csharp
public partial class AddCustomerWithConstraints : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                    
                // String with constraints
                Email = table.Column<string>(
                    maxLength: 100, 
                    nullable: false),
                    
                // Numeric with precision
                Price = table.Column<decimal>(
                    type: "decimal(18,2)", 
                    nullable: false),
                    
                // Date with default
                CreatedAt = table.Column<DateTime>(
                    nullable: false,
                    defaultValueSql: "GETUTCDATE()"),
                    
                // Foreign key
                CustomerId = table.Column<int>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
                
                // Foreign key constraint
                table.ForeignKey(
                    name: "FK_Orders_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Unique index for business key
        migrationBuilder.CreateIndex(
            name: "IX_Customer_Email_BusinessKey",
            table: "Customers",
            column: "Email",
            unique: true);
            
        // Regular index
        migrationBuilder.CreateIndex(
            name: "IX_Customer_Age",
            table: "Customers",
            column: "Age");
    }
}
```

---

## 🔄 **Backend Model Classes**

### **Update C# Models to Match Frontend**

```csharp
// MyCodeGent.Web/Models/EntityModel.cs
public class EntityModel
{
    public string Name { get; set; }
    public bool HasAuditFields { get; set; }
    public bool HasSoftDelete { get; set; }
    public List<PropertyModel> Properties { get; set; }
    public List<RelationshipModel> Relationships { get; set; }  // NEW
    public List<string> BusinessKeys { get; set; }  // NEW
}

// MyCodeGent.Web/Models/PropertyModel.cs
public class PropertyModel
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsKey { get; set; }
    public bool IsRequired { get; set; }
    public int? MaxLength { get; set; }
    public PropertyConstraints Constraints { get; set; }  // NEW
}

// MyCodeGent.Web/Models/PropertyConstraints.cs (NEW)
public class PropertyConstraints
{
    // String
    public string MinLength { get; set; }
    public string MaxLength { get; set; }
    public string RegexPattern { get; set; }
    public string StringDefaultValue { get; set; }
    
    // Numeric
    public string MinValue { get; set; }
    public string MaxValue { get; set; }
    public string Precision { get; set; }
    public string Scale { get; set; }
    public string NumericDefaultValue { get; set; }
    
    // Date
    public string DateDefaultValue { get; set; }
    
    // Boolean
    public bool BoolDefaultValue { get; set; }
    
    // General
    public bool IsUnique { get; set; }
    public bool IsIndexed { get; set; }
    public bool IsComputed { get; set; }
}

// MyCodeGent.Web/Models/RelationshipModel.cs (NEW)
public class RelationshipModel
{
    public string Type { get; set; }  // OneToMany, ManyToOne, OneToOne, ManyToMany
    public string RelatedEntity { get; set; }
    public int RelatedEntityId { get; set; }
    public string ForeignKeyProperty { get; set; }
    public string PrincipalKey { get; set; }
    public string NavigationProperty { get; set; }
    public string InverseNavigationProperty { get; set; }
    public string OnDeleteBehavior { get; set; }  // Cascade, SetNull, Restrict, NoAction
    public string JoinTableName { get; set; }  // For ManyToMany
}
```

---

## 🎨 **Template Updates Required**

### **1. DbContext Configuration Template**

```csharp
// Add to DbContextTemplate.cs

foreach (var relationship in entity.Relationships)
{
    switch (relationship.Type)
    {
        case "OneToMany":
            sb.AppendLine($"            entity.HasMany(e => e.{relationship.NavigationProperty})");
            sb.AppendLine($"                .WithOne(x => x.{relationship.InverseNavigationProperty})");
            sb.AppendLine($"                .HasForeignKey(x => x.{relationship.ForeignKeyProperty})");
            sb.AppendLine($"                .OnDelete(DeleteBehavior.{relationship.OnDeleteBehavior});");
            break;
            
        case "ManyToOne":
            sb.AppendLine($"            entity.HasOne(e => e.{relationship.NavigationProperty})");
            sb.AppendLine($"                .WithMany(x => x.{relationship.InverseNavigationProperty})");
            sb.AppendLine($"                .HasForeignKey(e => e.{relationship.ForeignKeyProperty})");
            sb.AppendLine($"                .OnDelete(DeleteBehavior.{relationship.OnDeleteBehavior});");
            break;
            
        // ... handle OneToOne and ManyToMany
    }
}

// Business Keys
if (entity.BusinessKeys != null && entity.BusinessKeys.Any())
{
    if (entity.BusinessKeys.Count == 1)
    {
        sb.AppendLine($"            entity.HasIndex(e => e.{entity.BusinessKeys[0]})");
        sb.AppendLine($"                .IsUnique()");
        sb.AppendLine($"                .HasDatabaseName(\"IX_{entity.Name}_{entity.BusinessKeys[0]}_BusinessKey\");");
    }
    else
    {
        var keys = string.Join(", ", entity.BusinessKeys.Select(k => $"e.{k}"));
        var keyNames = string.Join("_", entity.BusinessKeys);
        sb.AppendLine($"            entity.HasIndex(e => new {{ {keys} }})");
        sb.AppendLine($"                .IsUnique()");
        sb.AppendLine($"                .HasDatabaseName(\"IX_{entity.Name}_{keyNames}_BusinessKey\");");
    }
}
```

### **2. Entity Template**

```csharp
// Add navigation properties based on relationships

foreach (var relationship in entity.Relationships)
{
    if (relationship.Type == "OneToMany")
    {
        sb.AppendLine($"    public virtual ICollection<{relationship.RelatedEntity}> {relationship.NavigationProperty} {{ get; set; }}");
    }
    else if (relationship.Type == "ManyToOne" || relationship.Type == "OneToOne")
    {
        sb.AppendLine($"    public virtual {relationship.RelatedEntity} {relationship.NavigationProperty} {{ get; set; }}");
    }
    else if (relationship.Type == "ManyToMany")
    {
        sb.AppendLine($"    public virtual ICollection<{relationship.RelatedEntity}> {relationship.NavigationProperty} {{ get; set; }}");
    }
}
```

### **3. Validator Template**

```csharp
// Generate FluentValidation rules from constraints

foreach (var property in entity.Properties)
{
    if (property.Constraints != null)
    {
        sb.AppendLine($"        RuleFor(x => x.{property.Name})");
        
        if (property.IsRequired)
        {
            sb.AppendLine($"            .NotEmpty()");
        }
        
        if (!string.IsNullOrEmpty(property.Constraints.MinLength))
        {
            sb.AppendLine($"            .MinimumLength({property.Constraints.MinLength})");
        }
        
        if (!string.IsNullOrEmpty(property.Constraints.MaxLength))
        {
            sb.AppendLine($"            .MaximumLength({property.Constraints.MaxLength})");
        }
        
        if (!string.IsNullOrEmpty(property.Constraints.RegexPattern))
        {
            sb.AppendLine($"            .Matches(@\"{property.Constraints.RegexPattern}\")");
        }
        
        if (!string.IsNullOrEmpty(property.Constraints.MinValue) && 
            !string.IsNullOrEmpty(property.Constraints.MaxValue))
        {
            sb.AppendLine($"            .InclusiveBetween({property.Constraints.MinValue}, {property.Constraints.MaxValue})");
        }
        
        sb.AppendLine($"            ;");
    }
}
```

---

## ✅ **Testing Checklist**

### **Frontend → Backend Flow**

- [ ] Property constraints sent in payload
- [ ] Relationships sent in payload
- [ ] Business keys sent in payload
- [ ] Preview shows constraints
- [ ] Preview shows relationships
- [ ] Diagram shows all relationships
- [ ] Generated DbContext has relationship configurations
- [ ] Generated migrations include indexes
- [ ] Generated validators include constraint rules
- [ ] Generated repository has business key lookup methods
- [ ] Generated entities have navigation properties

---

## 🎯 **Expected Generated Code Examples**

### **Customer Entity**

```csharp
public class Customer : BaseEntity
{
    public string Email { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    
    // Navigation Properties
    public virtual ICollection<Order> Orders { get; set; }
}
```

### **Customer Configuration**

```csharp
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> entity)
    {
        entity.Property(e => e.Email)
            .HasMaxLength(100)
            .IsRequired();
            
        entity.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_Customer_Email_BusinessKey");
            
        entity.HasMany(e => e.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### **Customer Validator**

```csharp
public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(100)
            .EmailAddress();
            
        RuleFor(x => x.Age)
            .InclusiveBetween(18, 120);
    }
}
```

### **Customer Repository**

```csharp
public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> GetByEmailAsync(string email);  // Business Key lookup
}
```

---

## 🚀 **Summary**

**Frontend is 100% complete and sends:**
- ✅ Property constraints (min/max, regex, defaults, unique, indexed)
- ✅ Relationships (OneToMany, ManyToOne, OneToOne, ManyToMany)
- ✅ Business keys (single and composite)
- ✅ All data flows to preview
- ✅ All data visualized in diagram

**Backend needs to:**
1. Update C# models to match frontend payload
2. Update templates to process constraints
3. Update templates to generate relationships
4. Update templates to generate business key indexes
5. Update validators to include constraint rules
6. Generate repository methods for business keys

**The UI is enterprise-ready. Backend integration will make it production-ready!** 🎉
