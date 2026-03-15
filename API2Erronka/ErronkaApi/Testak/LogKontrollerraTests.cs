using ErronkaApi.DTOak;
using ErronkaApi.Kontrollerrak;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ErronkaApi.Testak
{
    public class LogKontrollerraTests
    {

        [Fact]
        public void Eraikitzailea_Deitzean_Kontrollerra_OndoSortzenDa()
        {
            var controller = new LogKontrollerra();

            Assert.NotNull(controller);
        }

        [Fact]
        public void GordeLog_LogBaliozkoa_Denean_OkItzultzenDu()
        {
            var controller = new LogKontrollerra();

            var log = new LogDTO
            {
                Erabiltzailea = "admin",
                Ekintza = "Login egin du"
            };

            var result = controller.GordeLog(log);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void GordeLog_LogBaliozkoa_Denean_EzDuSalbuespenikBotatzen()
        {
            var controller = new LogKontrollerra();

            var log = new LogDTO
            {
                Erabiltzailea = "test",
                Ekintza = "Produktua sortu du"
            };

            var result = controller.GordeLog(log);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GordeLog_LogNull_Denean_ErroreaGertaDaiteke()
        {
            var controller = new LogKontrollerra();

            var result = controller.GordeLog(null);

            Assert.IsType<ObjectResult>(result);
        }
    }
}