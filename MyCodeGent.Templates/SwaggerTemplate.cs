using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class SwaggerTemplate
{
    public static string GenerateSwaggerConfiguration(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using Microsoft.OpenApi.Models;");
        sb.AppendLine("using System.Reflection;");
        sb.AppendLine();
        sb.AppendLine($"namespace {rootNamespace}.Api.Configuration;");
        sb.AppendLine();
        sb.AppendLine("public static class SwaggerConfiguration");
        sb.AppendLine("{");
        sb.AppendLine("    public static IServiceCollection AddSwaggerConfiguration(");
        sb.AppendLine("        this IServiceCollection services)");
        sb.AppendLine("    {");
        sb.AppendLine("        services.AddSwaggerGen(options =>");
        sb.AppendLine("        {");
        sb.AppendLine("            options.SwaggerDoc(\"v1\", new OpenApiInfo");
        sb.AppendLine("            {");
        sb.AppendLine($"                Title = \"{rootNamespace} API\",");
        sb.AppendLine("                Version = \"v1\",");
        sb.AppendLine($"                Description = \"API for {rootNamespace} application\",");
        sb.AppendLine("                Contact = new OpenApiContact");
        sb.AppendLine("                {");
        sb.AppendLine("                    Name = \"API Support\",");
        sb.AppendLine("                    Email = \"support@example.com\"");
        sb.AppendLine("                }");
        sb.AppendLine("            });");
        sb.AppendLine();
        sb.AppendLine("            // Include XML comments");
        sb.AppendLine("            var xmlFile = $\"{Assembly.GetExecutingAssembly().GetName().Name}.xml\";");
        sb.AppendLine("            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);");
        sb.AppendLine("            if (File.Exists(xmlPath))");
        sb.AppendLine("            {");
        sb.AppendLine("                options.IncludeXmlComments(xmlPath);");
        sb.AppendLine("            }");
        sb.AppendLine();
        sb.AppendLine("            // Add JWT authentication");
        sb.AppendLine("            options.AddSecurityDefinition(\"Bearer\", new OpenApiSecurityScheme");
        sb.AppendLine("            {");
        sb.AppendLine("                Description = \"JWT Authorization header using the Bearer scheme. Example: \\\"Bearer {token}\\\"\",");
        sb.AppendLine("                Name = \"Authorization\",");
        sb.AppendLine("                In = ParameterLocation.Header,");
        sb.AppendLine("                Type = SecuritySchemeType.ApiKey,");
        sb.AppendLine("                Scheme = \"Bearer\"");
        sb.AppendLine("            });");
        sb.AppendLine();
        sb.AppendLine("            options.AddSecurityRequirement(new OpenApiSecurityRequirement");
        sb.AppendLine("            {");
        sb.AppendLine("                {");
        sb.AppendLine("                    new OpenApiSecurityScheme");
        sb.AppendLine("                    {");
        sb.AppendLine("                        Reference = new OpenApiReference");
        sb.AppendLine("                        {");
        sb.AppendLine("                            Type = ReferenceType.SecurityScheme,");
        sb.AppendLine("                            Id = \"Bearer\"");
        sb.AppendLine("                        }");
        sb.AppendLine("                    },");
        sb.AppendLine("                    Array.Empty<string>()");
        sb.AppendLine("                }");
        sb.AppendLine("            });");
        sb.AppendLine("        });");
        sb.AppendLine();
        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateControllerWithXmlComments(EntityModel entity)
    {
        var sb = new StringBuilder();
        var entityLower = entity.Name.ToLower();
        
        sb.AppendLine("using MediatR;");
        sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s;");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Commands.Create{entity.Name};");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Commands.Update{entity.Name};");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Commands.Delete{entity.Name};");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Queries.Get{entity.Name}ById;");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Queries.GetAll{entity.Name}s;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Api.Controllers;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Manages {entity.Name} operations");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("[ApiController]");
        sb.AppendLine($"[Route(\"api/[controller]\")]");
        sb.AppendLine($"[Produces(\"application/json\")]");
        sb.AppendLine($"public class {entity.Name}sController : ControllerBase");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly IMediator _mediator;");
        sb.AppendLine();
        sb.AppendLine($"    public {entity.Name}sController(IMediator mediator)");
        sb.AppendLine("    {");
        sb.AppendLine("        _mediator = mediator;");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        // GET All
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Gets all {entity.Name}s");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    /// <returns>List of {entity.Name}s</returns>");
        sb.AppendLine($"    /// <response code=\"200\">Returns the list of {entityLower}s</response>");
        sb.AppendLine("    [HttpGet]");
        sb.AppendLine($"    [ProducesResponseType(typeof(List<{entity.Name}Dto>), StatusCodes.Status200OK)]");
        sb.AppendLine("    public async Task<IActionResult> GetAll()");
        sb.AppendLine("    {");
        sb.AppendLine($"        var result = await _mediator.Send(new GetAll{entity.Name}sQuery());");
        sb.AppendLine("        return Ok(result);");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        // GET By Id
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Gets a specific {entity.Name} by ID");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    /// <param name=\"id\">The {entity.Name} ID</param>");
        sb.AppendLine($"    /// <returns>The {entity.Name}</returns>");
        sb.AppendLine($"    /// <response code=\"200\">Returns the {entityLower}</response>");
        sb.AppendLine($"    /// <response code=\"404\">{entity.Name} not found</response>");
        sb.AppendLine("    [HttpGet(\"{id}\")]");
        sb.AppendLine($"    [ProducesResponseType(typeof({entity.Name}Dto), StatusCodes.Status200OK)]");
        sb.AppendLine("    [ProducesResponseType(StatusCodes.Status404NotFound)]");
        sb.AppendLine("    public async Task<IActionResult> GetById(int id)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var result = await _mediator.Send(new Get{entity.Name}ByIdQuery {{ Id = id }});");
        sb.AppendLine("        return Ok(result);");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        // POST
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Creates a new {entity.Name}");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    /// <param name=\"command\">The {entity.Name} data</param>");
        sb.AppendLine($"    /// <returns>The created {entity.Name} ID</returns>");
        sb.AppendLine($"    /// <response code=\"201\">{entity.Name} created successfully</response>");
        sb.AppendLine("    /// <response code=\"400\">Invalid request data</response>");
        sb.AppendLine("    [HttpPost]");
        sb.AppendLine("    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]");
        sb.AppendLine("    [ProducesResponseType(StatusCodes.Status400BadRequest)]");
        sb.AppendLine($"    public async Task<IActionResult> Create([FromBody] Create{entity.Name}Command command)");
        sb.AppendLine("    {");
        sb.AppendLine("        var id = await _mediator.Send(command);");
        sb.AppendLine("        return CreatedAtAction(nameof(GetById), new { id }, id);");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        // PUT
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Updates an existing {entity.Name}");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    /// <param name=\"id\">The ID</param>");
        sb.AppendLine($"    /// <param name=\"command\">The updated {entity.Name} data</param>");
        sb.AppendLine("    /// <response code=\"204\">Update successful</response>");
        sb.AppendLine("    /// <response code=\"404\">Not found</response>");
        sb.AppendLine("    [HttpPut(\"{id}\")]");
        sb.AppendLine("    [ProducesResponseType(StatusCodes.Status204NoContent)]");
        sb.AppendLine("    [ProducesResponseType(StatusCodes.Status404NotFound)]");
        sb.AppendLine($"    public async Task<IActionResult> Update(int id, [FromBody] Update{entity.Name}Command command)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (id != command.Id) return BadRequest();");
        sb.AppendLine("        await _mediator.Send(command);");
        sb.AppendLine("        return NoContent();");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        // DELETE
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Deletes a {entity.Name}");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    /// <param name=\"id\">The ID</param>");
        sb.AppendLine("    /// <response code=\"204\">Delete successful</response>");
        sb.AppendLine("    /// <response code=\"404\">Not found</response>");
        sb.AppendLine("    [HttpDelete(\"{id}\")]");
        sb.AppendLine("    [ProducesResponseType(StatusCodes.Status204NoContent)]");
        sb.AppendLine("    [ProducesResponseType(StatusCodes.Status404NotFound)]");
        sb.AppendLine("    public async Task<IActionResult> Delete(int id)");
        sb.AppendLine("    {");
        sb.AppendLine($"        await _mediator.Send(new Delete{entity.Name}Command {{ Id = id }});");
        sb.AppendLine("        return NoContent();");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
}
