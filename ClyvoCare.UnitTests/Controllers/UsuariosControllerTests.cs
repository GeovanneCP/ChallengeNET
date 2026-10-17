using ClyvoCare.API.Controllers;
using ClyvoCare.API.Data;
using ClyvoCare.API.DTOs;
using ClyvoCare.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClyvoCare.UnitTests.Controllers;

public class UsuariosControllerTests
{
    private readonly Mock<ILogger<UsuariosController>> _loggerMock = new();

    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task PostUsuario_DadosValidos_RetornaCreatedAtAction()
    {
        // Arrange
        using var context = CreateDbContext();
        var controller = new UsuariosController(context, _loggerMock.Object);
        var dto = new UsuarioCreateDTO
        {
            Nome = "Geovanne",
            Email = "geovanne@teste.com",
            Senha = "senhaForte123"
        };

        // Act
        var result = await controller.PostUsuario(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        var usuario = Assert.IsType<Usuario>(createdResult.Value);
        Assert.Equal(dto.Email, usuario.Email);
    }
}