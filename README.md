# MyCodeGent - .NET CRUD Code Generator

**MyCodeGent** is a powerful code generator that automates the creation of CRUD functionality for .NET applications following **Clean Architecture** and **CQRS** design patterns.

## 🚀 Features

- ✅ **Clean Architecture** - Generates code organized in Domain, Application, Infrastructure, and API layers
- ✅ **CQRS Pattern** - Separates Commands and Queries using MediatR
- ✅ **Complete CRUD Operations** - Create, Read, Update, Delete with full implementation
- ✅ **Entity Framework Core** - Database context and entity configurations
- ✅ **FluentValidation** - Input validation for commands
- ✅ **RESTful API Controllers** - Ready-to-use API endpoints
- ✅ **Soft Delete Support** - Optional soft delete functionality
- ✅ **Audit Fields** - Automatic CreatedAt, UpdatedAt tracking
- ✅ **DTOs** - Data Transfer Objects for clean API responses
- ✅ **Configurable** - JSON-based configuration for easy customization

## 📁 Project Structure

```
mycodegent/
├── MyCodeGent.CLI/          # Command-line interface
├── MyCodeGent.Core/         # Core models, interfaces, and services
│   ├── Interfaces/
│   ├── Models/
│   └── Services/
└── MyCodeGent.Templates/    # Code generation templates
    ├── EntityTemplate.cs
    ├── CommandTemplate.cs
    ├── QueryTemplate.cs
    ├── HandlerTemplate.cs
    ├── DtoTemplate.cs
    ├── ControllerTemplate.cs
    ├── ValidatorTemplate.cs
    └── InfrastructureTemplate.cs
```

## 🛠️ Installation

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code

### Build from Source

```bash
cd mycodegent
dotnet build
```

## 📖 Usage

### 1. Run the Generator

```bash
cd MyCodeGent.CLI
dotnet run
```

On first run, it will create a sample configuration file: `codegen-config.json`

### 2. Configure Your Entities

Edit `codegen-config.json` to define your entities:

```json
{
  "config": {
    "outputPath": "./Generated",
    "rootNamespace": "MyApp",
    "generateApi": true,
    "generateApplication": true,
    "generateDomain": true,
    "generateInfrastructure": true,
    "useMediator": true,
    "useFluentValidation": true,
    "useAutoMapper": true,
    "databaseProvider": "SqlServer"
  },
  "entities": [
    {
      "name": "Product",
      "hasAuditFields": true,
      "hasSoftDelete": true,
      "properties": [
        {
          "name": "Id",
          "type": "int",
          "isKey": true,
          "isRequired": true
        },
        {
          "name": "Name",
          "type": "string",
          "isRequired": true,
          "maxLength": 200
        },
        {
          "name": "Price",
          "type": "decimal",
          "isRequired": true
        }
      ]
    }
  ]
}
```

### 3. Generate Code

```bash
dotnet run
```

Or with a custom config file:

```bash
dotnet run path/to/your-config.json
```

## 📦 Generated Output

The generator creates a complete Clean Architecture solution:

```
Generated/
├── Domain/
│   └── Entities/
│       ├── Product.cs
│       └── Customer.cs
├── Application/
│   ├── Products/
│   │   ├── ProductDto.cs
│   │   ├── Commands/
│   │   │   ├── CreateProduct/
│   │   │   │   ├── CreateProductCommand.cs
│   │   │   │   ├── CreateProductCommandHandler.cs
│   │   │   │   └── CreateProductCommandValidator.cs
│   │   │   ├── UpdateProduct/
│   │   │   └── DeleteProduct/
│   │   └── Queries/
│   │       ├── GetProductById/
│   │       └── GetAllProducts/
│   └── Common/
│       ├── Interfaces/
│       │   └── IApplicationDbContext.cs
│       └── Models/
│           └── PagedResult.cs
├── Infrastructure/
│   └── Persistence/
│       ├── ApplicationDbContext.cs
│       └── Configurations/
│           ├── ProductConfiguration.cs
│           └── CustomerConfiguration.cs
└── Api/
    └── Controllers/
        ├── ProductsController.cs
        └── CustomersController.cs
```

## 🎯 Generated Features

### Domain Layer
- Entity classes with data annotations
- Audit fields (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
- Soft delete support (IsDeleted, DeletedAt, DeletedBy)

### Application Layer
- **Commands**: CreateEntity, UpdateEntity, DeleteEntity
- **Queries**: GetEntityById, GetAllEntities
- **Handlers**: MediatR request handlers for all commands and queries
- **Validators**: FluentValidation validators for input validation
- **DTOs**: Clean data transfer objects

### Infrastructure Layer
- **DbContext**: Entity Framework Core database context
- **Entity Configurations**: Fluent API configurations
- **Query Filters**: Automatic soft delete filtering

### API Layer
- **Controllers**: RESTful API controllers with all CRUD endpoints
  - `GET /api/products` - Get all
  - `GET /api/products/{id}` - Get by ID
  - `POST /api/products` - Create
  - `PUT /api/products/{id}` - Update
  - `DELETE /api/products/{id}` - Delete

## 🔧 Configuration Options

| Option | Type | Description |
|--------|------|-------------|
| `outputPath` | string | Output directory for generated code |
| `rootNamespace` | string | Root namespace for all generated files |
| `generateApi` | bool | Generate API controllers |
| `generateApplication` | bool | Generate application layer (CQRS) |
| `generateDomain` | bool | Generate domain entities |
| `generateInfrastructure` | bool | Generate EF Core configurations |
| `useMediator` | bool | Use MediatR for CQRS |
| `useFluentValidation` | bool | Generate FluentValidation validators |
| `useAutoMapper` | bool | Use AutoMapper (future feature) |
| `databaseProvider` | enum | SqlServer, PostgreSql, MySql, Sqlite |

## 📝 Entity Configuration

### Property Options

| Property | Type | Description |
|----------|------|-------------|
| `name` | string | Property name |
| `type` | string | C# type (int, string, decimal, DateTime, etc.) |
| `isKey` | bool | Mark as primary key |
| `isRequired` | bool | Required field |
| `isNullable` | bool | Nullable type |
| `maxLength` | int? | Maximum string length |
| `defaultValue` | string? | Default value |

### Entity Options

| Property | Type | Description |
|----------|------|-------------|
| `name` | string | Entity name |
| `namespace` | string | Custom namespace (optional) |
| `hasAuditFields` | bool | Include audit fields |
| `hasSoftDelete` | bool | Enable soft delete |
| `properties` | array | List of properties |

## 🎨 Example: Generated Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllProductsQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateProductCommand command)
    {
        if (id != command.Id) return BadRequest();
        var result = await _mediator.Send(command);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }
}
```

## 🔄 Workflow

1. **Define entities** in `codegen-config.json`
2. **Run generator** with `dotnet run`
3. **Copy generated code** to your actual project
4. **Add NuGet packages**:
   - MediatR
   - FluentValidation
   - Microsoft.EntityFrameworkCore
   - Microsoft.AspNetCore.Mvc
5. **Register services** in your DI container
6. **Run migrations** and start using!

## 🚧 Future Enhancements

- [ ] AutoMapper profile generation
- [ ] Repository pattern support
- [ ] Unit test generation
- [ ] Specification pattern
- [ ] GraphQL support
- [ ] Pagination helpers
- [ ] Search and filtering
- [ ] Audit logging
- [ ] Multi-tenancy support
- [ ] Localization support

## 📄 License

MIT License - Feel free to use in your projects!

## 🤝 Contributing

Contributions are welcome! Feel free to submit issues and pull requests.

## 💡 Tips

- Start with the sample configuration to understand the structure
- Use meaningful entity and property names
- Enable audit fields and soft delete for better data tracking
- Review generated code before integrating into your project
- Customize templates in `MyCodeGent.Templates` for your specific needs

## 📞 Support

For issues, questions, or suggestions, please create an issue in the repository.

---

**Happy Coding! 🎉**
