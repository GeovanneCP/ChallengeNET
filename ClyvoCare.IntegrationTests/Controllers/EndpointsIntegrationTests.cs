using System.Net;
using System.Net.Http.Json;
using ClyvoCare.API.DTOs;
using ClyvoCare.API.Models;
using ClyvoCare.IntegrationTests.Fixtures;
using Xunit;

namespace ClyvoCare.IntegrationTests.Controllers;

public class EndpointsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EndpointsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostUsuario_PayloadValido_RetornaCreated()
    {
        // Arrange
        var usuarioDto = new UsuarioCreateDTO
        {
            Nome = "Tutor Integracao",
            Email = $"tutor_{Guid.NewGuid()}@teste.com",
            Senha = "senhaForte123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Usuarios", usuarioDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostLogSaude_PayloadValido_RetornaCreated()
    {
        // Arrange
        var logDto = new LogSaudeCreateDTO
        {
            PetId = 1,
            Peso = 10.5m,
            Temperatura = 38.8m,
            BatimentosCardiacos = 115,
            Observacoes = "Teste de integracao IoT"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/LogsSaude", logDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}