using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using ClyvoCare.API.DTOs;
using ClyvoCare.API.Models;
using ClyvoCare.IntegrationTests.Fixtures;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace ClyvoCare.IntegrationTests.Controllers;

public class EndpointsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EndpointsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        AutenticarCliente();
    }

    private void AutenticarCliente()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("ClyvoCareSecretKeyMuitoSeguraParaSprint4DotNet2026");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Tutor Integracao"),
                new Claim(ClaimTypes.Email, "integracao@teste.com")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "ClyvoCareAPI",
            Audience = "ClyvoCareClient",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);
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

    [Fact]
    public async Task PostPet_ComTokenValido_RetornaCreated()
    {
        // Arrange
        var petDto = new PetCreateDTO
        {
            Nome = "Pipoca",
            Especie = "Felino",
            DataNascimento = DateTime.Now.AddYears(-1),
            UsuarioId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Pets", petDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}