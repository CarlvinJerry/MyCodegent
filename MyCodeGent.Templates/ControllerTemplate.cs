using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class ControllerTemplate
{
    public static string Generate(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        var keyType = keyProp?.Type ?? "int";
        var keyName = keyProp?.Name ?? "Id";
        var keyNameLower = string.IsNullOrEmpty(keyName) ? "id" : keyName.ToLower();
        
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
        sb.AppendLine("[ApiController]");
        sb.AppendLine($"[Route(\"api/[controller]\")]");
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
        sb.AppendLine("    [HttpGet]");
        sb.AppendLine($"    public async Task<ActionResult<List<{entity.Name}Dto>>> GetAll()");
        sb.AppendLine("    {");
        sb.AppendLine($"        var result = await _mediator.Send(new GetAll{entity.Name}sQuery());");
        sb.AppendLine("        return Ok(result);");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        // GET By Id
        sb.AppendLine($"    [HttpGet(\"{{{keyNameLower}}}\")]");
        sb.AppendLine($"    public async Task<ActionResult<{entity.Name}Dto>> GetById({keyType} {keyNameLower})");
        sb.AppendLine("    {");
        sb.AppendLine($"        var result = await _mediator.Send(new Get{entity.Name}ByIdQuery({keyNameLower}));");
        sb.AppendLine("        if (result == null) return NotFound();");
        sb.AppendLine("        return Ok(result);");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        // POST Create
        sb.AppendLine("    [HttpPost]");
        sb.AppendLine($"    public async Task<ActionResult<{keyType}>> Create(Create{entity.Name}Command command)");
        sb.AppendLine("    {");
        sb.AppendLine("        var result = await _mediator.Send(command);");
        sb.AppendLine($"        return CreatedAtAction(nameof(GetById), new {{ {keyNameLower} = result }}, result);");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        // PUT Update
        sb.AppendLine($"    [HttpPut(\"{{{keyNameLower}}}\")]");
        sb.AppendLine($"    public async Task<ActionResult> Update({keyType} {keyNameLower}, Update{entity.Name}Command command)");
        sb.AppendLine("    {");
        sb.AppendLine($"        if ({keyNameLower} != command.{keyName}) return BadRequest();");
        sb.AppendLine("        var result = await _mediator.Send(command);");
        sb.AppendLine("        if (!result) return NotFound();");
        sb.AppendLine("        return NoContent();");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        // DELETE
        sb.AppendLine($"    [HttpDelete(\"{{{keyNameLower}}}\")]");
        sb.AppendLine($"    public async Task<ActionResult> Delete({keyType} {keyNameLower})");
        sb.AppendLine("    {");
        sb.AppendLine($"        var result = await _mediator.Send(new Delete{entity.Name}Command({keyNameLower}));");
        sb.AppendLine("        if (!result) return NotFound();");
        sb.AppendLine("        return NoContent();");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
}
