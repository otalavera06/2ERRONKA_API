using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Modeloak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ErronkaApi.Testak
{
    public class MahaiaKontrolTest
    {
        [Fact]
        public void LortuMahaiLibre_mahaiak_daude_eta_itzultzen_ditu()
        {
            var mockRepo = new Mock<IMahaiaRepository>();
            var m1 = new MahaiaDTO { Id = 1, Izena = "Mahaia 1", Erabiltzailea = "mahaia1", ChatBaimena = "true" };
            var m2 = new MahaiaDTO { Id = 2, Izena = "Mahaia 2", Erabiltzailea = "mahaia2", ChatBaimena = "false" };

            mockRepo.Setup(r => r.LortuMahaiLibre()).Returns(new List<MahaiaDTO> { m1, m2 });
            var controller = new MahaiakController(mockRepo.Object);

            var result = controller.LortuMahaiLibre();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<ErantzunaDTO<MahaiaDTO>>(ok.Value);
            Assert.Equal(200, dto.Code);
            Assert.Contains(m1, dto.Datuak);
            Assert.Contains(m2, dto.Datuak);
        }

        [Fact]
        public void LortuMahaiLibre_mahai_librerik_ez_201_itzultzen_du()
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
        public void LortuMahaiLibre_repoak_null_itzultzean_500_itzultzen_du()
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

        [Fact]
        public void Login_datuak_hutsik_badira_400_itzultzen_du()
        {
            var mockRepo = new Mock<IMahaiaRepository>();
            var controller = new MahaiakController(mockRepo.Object);

            var result = controller.Login(new MahaiakController.MahaiakLoginRequest("", ""));

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void CheckTxatBaimena_mahaia_badago_baimena_itzultzen_du(string balioa, bool esperoDenBaimena)
        {
            var mockRepo = new Mock<IMahaiaRepository>();
            mockRepo.Setup(r => r.Get(3)).Returns(new Mahaia { id = 3, chat_baimena = balioa });
            var controller = new MahaiakController(mockRepo.Object);

            var result = controller.CheckTxatBaimena(3);

            var ok = Assert.IsType<OkObjectResult>(result);
            var prop = ok.Value!.GetType().GetProperty("chatBaimena")!;
            Assert.Equal(esperoDenBaimena, prop.GetValue(ok.Value));
        }

        [Fact]
        public void CheckTxatBaimena_mahaia_ez_badago_404_itzultzen_du()
        {
            var mockRepo = new Mock<IMahaiaRepository>();
            mockRepo.Setup(r => r.Get(9)).Returns((Mahaia)null);
            var controller = new MahaiakController(mockRepo.Object);

            var result = controller.CheckTxatBaimena(9);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
