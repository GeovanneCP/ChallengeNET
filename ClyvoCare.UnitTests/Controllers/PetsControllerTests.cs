using System.Linq.Expressions;
using ClyvoCare.API.Controllers;
using ClyvoCare.API.Data.Repositories;
using ClyvoCare.API.DTOs;
using ClyvoCare.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClyvoCare.UnitTests.Controllers;

public class PetsControllerTests
{
    private readonly Mock<IRepository<Pet>> _petRepoMock = new();
    private readonly Mock<ILogger<PetsController>> _loggerMock = new();
    private readonly Mock<IUrlHelper> _urlHelperMock = new();

    private PetsController CriarController()
    {
        var controller = new PetsController(_petRepoMock.Object, _loggerMock.Object);
        _urlHelperMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
                      .Returns("http://localhost/api/pets/1");

        controller.Url = _urlHelperMock.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        return controller;
    }

    [Fact]
    public async Task PostPet_DadosValidos_SalvaComSucessoERetornaCreatedComHateoas()
    {
        // Arrange
        var controller = CriarController();
        var dto = new PetCreateDTO
        {
            Nome = "Rex",
            Especie = "Cachorro",
            DataNascimento = DateTime.Now.AddYears(-2),
            UsuarioId = 1
        };

        _petRepoMock.Setup(r => r.AddAsync(It.IsAny<Pet>()))
                    .Callback<Pet>(p => p.Id = 1)
                    .Returns(Task.CompletedTask);

        // Act
        var result = await controller.PostPet(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        var response = Assert.IsType<HateoasResponse<Pet>>(createdResult.Value);

        Assert.Equal("Rex", response.Data.Nome);
        Assert.Equal(1, response.Data.UsuarioId);
        Assert.NotEmpty(response.Links);
        _petRepoMock.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Once);
    }

    [Fact]
    public async Task GetPet_IdExistente_RetornaOkComHateoas()
    {
        // Arrange
        var controller = CriarController();
        var petExistente = new Pet { Id = 1, Nome = "Mingau", Especie = "Gato", UsuarioId = 1 };

        _petRepoMock.Setup(r => r.GetByIdAsync(1))
                    .ReturnsAsync(petExistente);

        // Act
        var result = await controller.GetPet(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<HateoasResponse<Pet>>(okResult.Value);

        Assert.Equal(1, response.Data.Id);
        Assert.Equal("Mingau", response.Data.Nome);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "telemetria_nosql");
    }

    [Fact]
    public async Task GetPet_IdInexistente_RetornaNotFound()
    {
        // Arrange
        var controller = CriarController();
        _petRepoMock.Setup(r => r.GetByIdAsync(999))
                    .ReturnsAsync((Pet?)null);

        // Act
        var result = await controller.GetPet(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetPets_PaginacaoEFiltro_RetornaPagedResult()
    {
        // Arrange
        var controller = CriarController();
        var filtro = new PaginationFilter { PageNumber = 1, PageSize = 10, SearchTerm = "Rex" };
        var listaPets = new List<Pet>
        {
            new Pet { Id = 1, Nome = "Rex", Especie = "Cachorro", UsuarioId = 1 }
        };

        var pagedResult = new PagedResult<Pet>
        {
            Items = listaPets,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _petRepoMock.Setup(r => r.GetPagedAsync(filtro, It.IsAny<Expression<Func<Pet, bool>>>()))
                    .ReturnsAsync(pagedResult);

        // Act
        var result = await controller.GetPets(filtro);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var pagedResponse = Assert.IsType<PagedResult<HateoasResponse<Pet>>>(okResult.Value);

        Assert.Equal(1, pagedResponse.TotalCount);
        Assert.Single(pagedResponse.Items);
    }
}