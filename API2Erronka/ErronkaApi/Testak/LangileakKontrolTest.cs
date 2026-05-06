using ErronkaApi.DTOak;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ErronkaApi.Interfaces;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Modeloak;

namespace ErronkaApi.Testak
{
    public class LangileakKontrolTest
    {
        [Fact]
        public void GetAll_langileak_daudenean_erantzun_ok_itzultzen_du()
        {
            var mockRepo = new Mock<IErabiltzaileaRepository>();
            var langileak = new List<LangileaDTO>
            {
                new LangileaDTO
                {
                    Id = 1,
                    Izena = "Ander",
                    Erabiltzailea = "ander",
                    Email = "ander@example.com",
                    Baimena = true,
                    ChatBaimena = true
                }
            };

            mockRepo.Setup(r => r.GetAll()).Returns(langileak);

            var controller = new LangileakController(mockRepo.Object);

            var result = controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ErantzunaDTO<LangileaDTO>>(ok.Value);
            Assert.Equal(200, dto.Code);
            Assert.Single(dto.Datuak);
            Assert.Equal("Ander", dto.Datuak[0].Izena);
        }

        [Fact]
        public void GetAll_repoak_salbuespena_botatzen_duenean_500_itzultzen_du()
        {
            var mockRepo = new Mock<IErabiltzaileaRepository>();
            mockRepo.Setup(r => r.GetAll()).Throws(new Exception("boom"));

            var controller = new LangileakController(mockRepo.Object);

            var result = controller.GetAll();

            var error = Assert.IsType<ObjectResult>(result);
            var dto = Assert.IsType<ErantzunaDTO<LangileaDTO>>(error.Value);
            Assert.Equal(500, error.StatusCode);
            Assert.Equal(500, dto.Code);
            Assert.Empty(dto.Datuak);
        }

        [Fact]
        public void Login_ondo_200_itzultzen_du()
        {
            var mockRepo = new Mock<IErabiltzaileaRepository>();
            var erabiltzailea = new Erabiltzailea
            {
                id = 10,
                erabiltzailea = "user",
                emaila = "user@example.com",
                pasahitza = "pass",
                rola = new Rola { id = 1, izena = "admin" },
                ezabatua = false,
                txat = true
            };
            mockRepo.Setup(r => r.Login("user", "pass")).Returns(erabiltzailea);
            var controller = new LangileakController(mockRepo.Object);
            var result = controller.Login(new LangileakController.LoginRequest("user", "pass"));
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public void Login_oker_401_itzultzen_du()
        {
            var mockRepo = new Mock<IErabiltzaileaRepository>();
            mockRepo.Setup(r => r.Login("user", "bad")).Returns((Erabiltzailea)null);
            var controller = new LangileakController(mockRepo.Object);
            var result = controller.Login(new LangileakController.LoginRequest("user", "bad"));
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void Login_txat_baimenik_gabe_chatBaimena_false_itzultzen_du()
        {
            var mockRepo = new Mock<IErabiltzaileaRepository>();
            var erabiltzailea = new Erabiltzailea
            {
                id = 11,
                erabiltzailea = "user",
                emaila = "user@example.com",
                pasahitza = "pass",
                rola = new Rola { id = 1, izena = "admin" },
                ezabatua = false,
                txat = false
            };

            mockRepo.Setup(r => r.Login("user", "pass")).Returns(erabiltzailea);
            var controller = new LangileakController(mockRepo.Object);

            var result = controller.Login(new LangileakController.LoginRequest("user", "pass"));

            var ok = Assert.IsType<OkObjectResult>(result);
            var prop = ok.Value!.GetType().GetProperty("chatBaimena")!;
            Assert.Equal(false, prop.GetValue(ok.Value));
        }
    }
}
