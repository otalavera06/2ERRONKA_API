using ErronkaApi.Interfaces;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Modeloak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ErronkaApi.Testak
{
    public class ErreserbakKontrolTest
    {
        [Fact]
        public void GetByDate_data_ok_den_when_erreserbak_daude_itzultzen_du()
        {
            var mockRepo = new Mock<IErreserbaRepository>();
            mockRepo.Setup(r => r.GetByDate(new DateTime(2026, 4, 16), true))
                .Returns(new List<Erreserba>
                {
                    new Erreserba { Id = 1, Data = new DateTime(2026, 4, 16), Mota = true, MahaiakId = 3, ErabiltzaileakId = 7 }
                });

            var controller = new ErreserbakController(mockRepo.Object);

            var result = controller.GetByDate("2026-04-16", true);

            var ok = Assert.IsType<OkObjectResult>(result);
            var values = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
            var item = Assert.Single(values);
            Assert.Equal(1, (int)item.GetType().GetProperty("Id")!.GetValue(item)!);
            Assert.Equal(3, (int)item.GetType().GetProperty("MahaiakId")!.GetValue(item)!);
        }

        [Fact]
        public void GetByDate_data_txarra_den_when_badrequest_itzultzen_du()
        {
            var mockRepo = new Mock<IErreserbaRepository>();
            var controller = new ErreserbakController(mockRepo.Object);

            var result = controller.GetByDate("16-04-2026", true);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("data format: yyyy-MM-dd", badRequest.Value);
        }

        [Fact]
        public void Create_repoak_sortzen_duenean_ok_itzultzen_du()
        {
            var dto = new ErreserbakController.ErreserbakSortuDto
            {
                Data = new DateTime(2026, 4, 16),
                Mota = false,
                ErabiltzaileakId = 5,
                MahaiakId = 2
            };

            var mockRepo = new Mock<IErreserbaRepository>();
            mockRepo.Setup(r => r.Create(dto))
                .Returns(new Erreserba
                {
                    Id = 11,
                    Data = dto.Data,
                    Mota = dto.Mota,
                    ErabiltzaileakId = dto.ErabiltzaileakId,
                    MahaiakId = dto.MahaiakId
                });

            var controller = new ErreserbakController(mockRepo.Object);

            var result = controller.Create(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public void UpdateByMahai_repoak_ez_badauka_notfound_itzultzen_du()
        {
            var dto = new ErreserbakController.ErreserbakUpdateDto { MahaiakId = 4 };
            var mockRepo = new Mock<IErreserbaRepository>();
            mockRepo.Setup(r => r.UpdateByMahai(3, new DateTime(2026, 4, 16), true, dto)).Returns(false);

            var controller = new ErreserbakController(mockRepo.Object);

            var result = controller.UpdateByMahai(3, "2026-04-16", true, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteByMahai_repoak_ezabatzen_duenean_nocontent_itzultzen_du()
        {
            var mockRepo = new Mock<IErreserbaRepository>();
            mockRepo.Setup(r => r.DeleteByMahai(3, new DateTime(2026, 4, 16), false)).Returns(true);

            var controller = new ErreserbakController(mockRepo.Object);

            var result = controller.DeleteByMahai(3, "2026-04-16", false);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
