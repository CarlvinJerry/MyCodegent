using MyCodeGent.Templates.Models;
using System.Text;

namespace MyCodeGent.Templates;

public static class TestTemplate
{
    public static string GenerateHandlerTests(EntityModel entity)
    {
        var sb = new StringBuilder();
        var keyProp = entity.Properties.FirstOrDefault(p => p.IsKey);
        var keyType = keyProp?.Type ?? "int";
        var keyName = keyProp?.Name ?? "Id";
        
        sb.AppendLine("using Xunit;");
        sb.AppendLine("using Moq;");
        sb.AppendLine("using FluentAssertions;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine($"using {entity.Namespace}.Application.Common.Interfaces;");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Commands.Create{entity.Name};");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Commands.Update{entity.Name};");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Commands.Delete{entity.Name};");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Queries.Get{entity.Name}ById;");
        sb.AppendLine($"using {entity.Namespace}.Domain.Entities;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.Tests.{entity.Name}s;");
        sb.AppendLine();
        
        // Create Command Tests
        sb.AppendLine($"public class Create{entity.Name}CommandHandlerTests");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly Mock<IApplicationDbContext> _mockContext;");
        sb.AppendLine($"    private readonly Mock<DbSet<{entity.Name}>> _mockDbSet;");
        sb.AppendLine($"    private readonly Create{entity.Name}CommandHandler _handler;");
        sb.AppendLine();
        sb.AppendLine($"    public Create{entity.Name}CommandHandlerTests()");
        sb.AppendLine("    {");
        sb.AppendLine("        _mockContext = new Mock<IApplicationDbContext>();");
        sb.AppendLine($"        _mockDbSet = new Mock<DbSet<{entity.Name}>>();");
        sb.AppendLine($"        _mockContext.Setup(x => x.{entity.Name}s).Returns(_mockDbSet.Object);");
        sb.AppendLine($"        _handler = new Create{entity.Name}CommandHandler(_mockContext.Object);");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    [Fact]");
        sb.AppendLine("    public async Task Handle_ValidCommand_ReturnsId()");
        sb.AppendLine("    {");
        sb.AppendLine("        // Arrange");
        sb.AppendLine($"        var command = new Create{entity.Name}Command");
        sb.AppendLine("        {");
        
        foreach (var prop in entity.Properties.Where(p => !p.IsKey).Take(3))
        {
            var testValue = GetTestValue(prop.Type);
            sb.AppendLine($"            {prop.Name} = {testValue},");
        }
        
        sb.AppendLine("        };");
        sb.AppendLine();
        sb.AppendLine("        _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))");
        sb.AppendLine("            .ReturnsAsync(1);");
        sb.AppendLine();
        sb.AppendLine("        // Act");
        sb.AppendLine("        var result = await _handler.Handle(command, CancellationToken.None);");
        sb.AppendLine();
        sb.AppendLine("        // Assert");
        sb.AppendLine($"        result.Should().Be{(keyType == "int" ? "GreaterThan" : "NotBe")}({(keyType == "int" ? "0" : "default")});");
        sb.AppendLine($"        _mockDbSet.Verify(x => x.Add(It.IsAny<{entity.Name}>()), Times.Once);");
        sb.AppendLine("        _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();
        
        // Update Command Tests
        sb.AppendLine($"public class Update{entity.Name}CommandHandlerTests");
        sb.AppendLine("{");
        sb.AppendLine("    [Fact]");
        sb.AppendLine("    public async Task Handle_ExistingEntity_ReturnsTrue()");
        sb.AppendLine("    {");
        sb.AppendLine("        // Arrange");
        sb.AppendLine("        var mockContext = new Mock<IApplicationDbContext>();");
        sb.AppendLine($"        var entity = new {entity.Name} {{ {keyName} = 1 }};");
        sb.AppendLine($"        var mockDbSet = CreateMockDbSet(new List<{entity.Name}> {{ entity }});");
        sb.AppendLine($"        mockContext.Setup(x => x.{entity.Name}s).Returns(mockDbSet.Object);");
        sb.AppendLine($"        var handler = new Update{entity.Name}CommandHandler(mockContext.Object);");
        sb.AppendLine();
        sb.AppendLine($"        var command = new Update{entity.Name}Command {{ {keyName} = 1 }};");
        sb.AppendLine();
        sb.AppendLine("        // Act");
        sb.AppendLine("        var result = await handler.Handle(command, CancellationToken.None);");
        sb.AppendLine();
        sb.AppendLine("        // Assert");
        sb.AppendLine("        result.Should().BeTrue();");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    [Fact]");
        sb.AppendLine("    public async Task Handle_NonExistingEntity_ReturnsFalse()");
        sb.AppendLine("    {");
        sb.AppendLine("        // Arrange");
        sb.AppendLine("        var mockContext = new Mock<IApplicationDbContext>();");
        sb.AppendLine($"        var mockDbSet = CreateMockDbSet(new List<{entity.Name}>());");
        sb.AppendLine($"        mockContext.Setup(x => x.{entity.Name}s).Returns(mockDbSet.Object);");
        sb.AppendLine($"        var handler = new Update{entity.Name}CommandHandler(mockContext.Object);");
        sb.AppendLine();
        sb.AppendLine($"        var command = new Update{entity.Name}Command {{ {keyName} = 999 }};");
        sb.AppendLine();
        sb.AppendLine("        // Act");
        sb.AppendLine("        var result = await handler.Handle(command, CancellationToken.None);");
        sb.AppendLine();
        sb.AppendLine("        // Assert");
        sb.AppendLine("        result.Should().BeFalse();");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    private static Mock<DbSet<{entity.Name}>> CreateMockDbSet(List<{entity.Name}> data)");
        sb.AppendLine("    {");
        sb.AppendLine("        var queryable = data.AsQueryable();");
        sb.AppendLine($"        var mockSet = new Mock<DbSet<{entity.Name}>>();");
        sb.AppendLine("        mockSet.As<IQueryable<" + entity.Name + ">>().Setup(m => m.Provider).Returns(queryable.Provider);");
        sb.AppendLine("        mockSet.As<IQueryable<" + entity.Name + ">>().Setup(m => m.Expression).Returns(queryable.Expression);");
        sb.AppendLine("        mockSet.As<IQueryable<" + entity.Name + ">>().Setup(m => m.ElementType).Returns(queryable.ElementType);");
        sb.AppendLine("        mockSet.As<IQueryable<" + entity.Name + ">>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());");
        sb.AppendLine("        return mockSet;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateValidatorTests(EntityModel entity)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using Xunit;");
        sb.AppendLine("using FluentAssertions;");
        sb.AppendLine($"using {entity.Namespace}.Application.{entity.Name}s.Commands.Create{entity.Name};");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Application.Tests.{entity.Name}s;");
        sb.AppendLine();
        sb.AppendLine($"public class Create{entity.Name}CommandValidatorTests");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly Create{entity.Name}CommandValidator _validator;");
        sb.AppendLine();
        sb.AppendLine($"    public Create{entity.Name}CommandValidatorTests()");
        sb.AppendLine("    {");
        sb.AppendLine($"        _validator = new Create{entity.Name}CommandValidator();");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        // Test for required fields
        var requiredProps = entity.Properties.Where(p => p.IsRequired && !p.IsKey).Take(2).ToList();
        foreach (var prop in requiredProps)
        {
            sb.AppendLine("    [Fact]");
            sb.AppendLine($"    public void Validate_{prop.Name}Required_ShouldHaveError()");
            sb.AppendLine("    {");
            sb.AppendLine("        // Arrange");
            sb.AppendLine($"        var command = new Create{entity.Name}Command {{ {prop.Name} = {GetInvalidValue(prop.Type)} }};");
            sb.AppendLine();
            sb.AppendLine("        // Act");
            sb.AppendLine("        var result = _validator.Validate(command);");
            sb.AppendLine();
            sb.AppendLine("        // Assert");
            sb.AppendLine("        result.IsValid.Should().BeFalse();");
            sb.AppendLine($"        result.Errors.Should().Contain(x => x.PropertyName == \"{prop.Name}\");");
            sb.AppendLine("    }");
            sb.AppendLine();
        }
        
        sb.AppendLine("    [Fact]");
        sb.AppendLine("    public void Validate_ValidCommand_ShouldNotHaveError()");
        sb.AppendLine("    {");
        sb.AppendLine("        // Arrange");
        sb.AppendLine($"        var command = new Create{entity.Name}Command");
        sb.AppendLine("        {");
        
        foreach (var prop in entity.Properties.Where(p => !p.IsKey).Take(3))
        {
            var testValue = GetTestValue(prop.Type);
            sb.AppendLine($"            {prop.Name} = {testValue},");
        }
        
        sb.AppendLine("        };");
        sb.AppendLine();
        sb.AppendLine("        // Act");
        sb.AppendLine("        var result = _validator.Validate(command);");
        sb.AppendLine();
        sb.AppendLine("        // Assert");
        sb.AppendLine("        result.IsValid.Should().BeTrue();");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    public static string GenerateTestProject(string rootNamespace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        sb.AppendLine();
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <TargetFramework>net9.0</TargetFramework>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("    <IsPackable>false</IsPackable>");
        sb.AppendLine("    <IsTestProject>true</IsTestProject>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine();
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine("    <PackageReference Include=\"coverlet.collector\" Version=\"6.0.0\" />");
        sb.AppendLine("    <PackageReference Include=\"FluentAssertions\" Version=\"6.12.0\" />");
        sb.AppendLine("    <PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"17.8.0\" />");
        sb.AppendLine("    <PackageReference Include=\"Moq\" Version=\"4.20.70\" />");
        sb.AppendLine("    <PackageReference Include=\"xunit\" Version=\"2.6.2\" />");
        sb.AppendLine("    <PackageReference Include=\"xunit.runner.visualstudio\" Version=\"2.5.4\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine();
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine($"    <ProjectReference Include=\"..\\Application\\{rootNamespace}.Application.csproj\" />");
        sb.AppendLine($"    <ProjectReference Include=\"..\\Domain\\{rootNamespace}.Domain.csproj\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine();
        sb.AppendLine("</Project>");
        
        return sb.ToString();
    }
    
    private static string GetTestValue(string type)
    {
        return type.ToLower() switch
        {
            "string" => "\"Test Value\"",
            "int" => "1",
            "long" => "1L",
            "decimal" => "10.50m",
            "double" => "10.50",
            "float" => "10.50f",
            "bool" => "true",
            "datetime" => "DateTime.UtcNow",
            "guid" => "Guid.NewGuid()",
            _ => "default"
        };
    }
    
    private static string GetInvalidValue(string type)
    {
        return type.ToLower() switch
        {
            "string" => "null",
            "int" => "0",
            "long" => "0L",
            "decimal" => "0m",
            "double" => "0.0",
            "float" => "0f",
            "datetime" => "default",
            "guid" => "Guid.Empty",
            _ => "default"
        };
    }
}
