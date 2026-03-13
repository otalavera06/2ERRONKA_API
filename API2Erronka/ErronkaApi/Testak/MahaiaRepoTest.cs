using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ErronkaApi.Interfaces;      // Para IMahaiaRepository
using ErronkaApi.DTOak;           // Para MahaiaDTO
using ErronkaApi.Kontrollerrak;  // Para MahaiaKontrollera
using System.Collections.Generic;
using System.Threading.Tasks;

public class MahaiaKontrolleraTests
{
    [Fact]
    public async Task get_mahaia_libre_ok()
    {
        // Arrange
        var mockRepo = new Mock<IMahaiaRepository>();

        mockRepo.Setup(x => x.LortuMahaiLibreAsync())
            .ReturnsAsync(new List<MahaiaDTO>
            {
                new MahaiaDTO { Id = 1, Zenbakia = 3 }
            });

        var controller = new MahaiaKontrollera(mockRepo.Object);

        // Act
        var result = await controller.GetMahaiLibre();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task get_mahaia_libre_errorea()
    {
        // Arrange
        var mockRepo = new Mock<IMahaiaRepository>();

        mockRepo.Setup(x => x.LortuMahaiLibreAsync())
            .ReturnsAsync((List<MahaiaDTO>)null);

        var controller = new MahaiaKontrollera(mockRepo.Object);

        // Act
        var result = await controller.GetMahaiLibre();

        // Assert
        var errorResult = Assert.IsType<ObjectResult>(result);
        Assert.NotEqual(200, errorResult.StatusCode);
    }
}