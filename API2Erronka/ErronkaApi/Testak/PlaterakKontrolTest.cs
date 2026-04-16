using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
using ErronkaApi.Kontrollerrak;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ErronkaApi.Testak
{
    public class PlaterakKontrolTest
    {
        [Fact]
        public void GetAll_platerak_daudenean_erantzun_ok_itzultzen_du()
        {
            var mockRepo = new Mock<IPlateraRepository>();
            var platerak = new List<PlateraDTO>
            {
                new PlateraDTO
                {
                    Id = 1,
                    Izena = "Entsalada",
                    Mota = "Lehenengoa",
                    Prezioa = 6.5f
                }
            };

            mockRepo.Setup(r => r.GetAll("https://localhost:7169")).Returns(platerak);

            var controller = new PlaterakController(mockRepo.Object)
            {
                ControllerContext = SortuControllerContext()
            };

            var result = controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ErantzunaDTO<PlateraDTO>>(ok.Value);
            Assert.Equal(200, dto.Code);
            Assert.Single(dto.Datuak);
            Assert.Equal("Entsalada", dto.Datuak[0].Izena);
        }

        [Fact]
        public void GetAll_repoak_salbuespena_botatzen_duenean_500_itzultzen_du()
        {
            var mockRepo = new Mock<IPlateraRepository>();
            mockRepo.Setup(r => r.GetAll(It.IsAny<string>())).Throws(new Exception("boom"));

            var controller = new PlaterakController(mockRepo.Object)
            {
                ControllerContext = SortuControllerContext()
            };

            var result = controller.GetAll();

            var error = Assert.IsType<ObjectResult>(result);
            var dto = Assert.IsType<ErantzunaDTO<PlateraDTO>>(error.Value);
            Assert.Equal(500, error.StatusCode);
            Assert.Equal(500, dto.Code);
            Assert.Empty(dto.Datuak);
        }

        private static ControllerContext SortuControllerContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost:7169");
            return new ControllerContext { HttpContext = httpContext };
        }
    }
}
