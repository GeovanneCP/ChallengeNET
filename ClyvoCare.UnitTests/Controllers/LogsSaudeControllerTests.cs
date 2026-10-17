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

public class LogsSaudeControllerTests
{
    private readonly Mock<ILogger<LogsSaudeController>> _loggerMock = new();

    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task PostLogSaude_DadosValidos_RegistraMetricaERetornaCreated()
    {
        // Arrange
        using var context = CreateDbContext();
        var controller = new LogsSaudeController(context, _loggerMock.Object);
        var dto = new LogSaudeCreateDTO
        {
            PetId = 1,
            Peso = 14.2m,
            Temperatura = 38.6m,
            BatimentosCardiacos = 105,
            Observacoes = "Coleta IoT automatica"
        };

        // Act
        var result = await controller.PostLogSaude(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        var log = Assert.IsType<LogSaude>(createdResult.Value);
        Assert.Equal(dto.PetId, log.PetId);
        Assert.Equal(dto.BatimentosCardiacos, log.BatimentosCardiacos);
    }
}