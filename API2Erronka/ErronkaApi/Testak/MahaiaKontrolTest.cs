using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ErronkaApi.Interfaces;
using ErronkaApi.DTOak;          
using ErronkaApi.Kontrollerrak;  
using System.Collections.Generic;
using System.Threading.Tasks;
using ErronkaApi.Interfaces;

public class MahaiaKontrolleraTests
{
    [Fact]
    public void LortuMahaiLibre_idMahaia_mahaiak_daude_eta_itzultzen_ditu()
    {
        var mockRepo = new Mock<IMahaiaRepository>();

        var m1 = new MahaiaDTO { Id = 1, Zenbakia = 5, kapazitatea = 4 };
        var m2 = new MahaiaDTO { Id = 2, Zenbakia = 7, kapazitatea = 2 };

        var zerrenda = new List<MahaiaDTO> { m1, m2 };

        mockRepo.Setup(r => r.LortuMahaiLibre()).Returns(zerrenda);

        var controller = new MahaiakController(mockRepo.Object);

        var result = controller.LortuMahaiLibre();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<ErantzunaDTO<MahaiaDTO>>(ok.Value);

        Assert.Equal(200, dto.Code);
        Assert.Contains(m1, dto.Datuak);
        Assert.Contains(m2, dto.Datuak);
    }

    [Fact]
    public void LortuMahaiLibre_mahai_librerik_ez_201()
    {
        var mockRepo = new Mock<IMahaiaRepository>();

        mockRepo.Setup(r => r.LortuMahaiLibre()).Returns(new List<MahaiaDTO>());

        var controller = new MahaiakController(mockRepo.Object);

        var result = controller.LortuMahaiLibre();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<ErantzunaDTO<List<MahaiaDTO>>>(ok.Value);

        Assert.Equal(201, dto.Code);
        Assert.Null(dto.Datuak);
    }

    [Fact]
    public void LortuMahaiLibre_errorea_500()
    {
        var mockRepo = new Mock<IMahaiaRepository>();

        mockRepo.Setup(r => r.LortuMahaiLibre()).Returns((List<MahaiaDTO>)null);

        var controller = new MahaiakController(mockRepo.Object);

        var result = controller.LortuMahaiLibre();

        var error = Assert.IsType<ObjectResult>(result.Result);
        var dto = Assert.IsType<ErantzunaDTO<List<MahaiaDTO>>>(error.Value);

        Assert.Equal(500, dto.Code);
        Assert.Null(dto.Datuak);
    }
}