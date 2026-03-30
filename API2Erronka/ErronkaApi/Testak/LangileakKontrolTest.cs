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
                ezabatua = false
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
    }
}
