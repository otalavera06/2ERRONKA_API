using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ErronkaApi.Interfaces;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.DTOak;
using System;
using System.Collections.Generic;

namespace ErronkaApi.Testak
{
    public class ZerbitzuaKontrolTest
    {
        [Fact]
        public void Create_mahaiaId_eman_da_201()
        {
            var mockRepo = new Mock<IZerbitzuaRepository>();
            mockRepo.Setup(r => r.Create(It.IsAny<ZerbitzuaController.ZerbitzuaSortuDto>())).Returns(123);
            var controller = new ZerbitzuaController(mockRepo.Object);
            var dto = new ZerbitzuaController.ZerbitzuaSortuDto
            {
                PrezioTotala = 10,
                Data = DateTime.Now,
                MahaiakId = 5,
                Eskaerak = new List<ZerbitzuaController.EskaeraSortuDto>()
            };
            var result = controller.Create(dto);
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ZerbitzuaController.GetByMahai), created.ActionName);
            Assert.NotNull(created.Value);
        }

        [Fact]
        public void Create_mahaiaId_gabe_400()
        {
            var mockRepo = new Mock<IZerbitzuaRepository>();
            var controller = new ZerbitzuaController(mockRepo.Object);
            var dto = new ZerbitzuaController.ZerbitzuaSortuDto
            {
                PrezioTotala = 10,
                Data = DateTime.Now,
                MahaiakId = null,
                Eskaerak = new List<ZerbitzuaController.EskaeraSortuDto>()
            };
            var result = controller.Create(dto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Create_produktua_ez_da_existitzen_400()
        {
            var mockRepo = new Mock<IZerbitzuaRepository>();
            mockRepo.Setup(r => r.Create(It.IsAny<ZerbitzuaController.ZerbitzuaSortuDto>()))
                .Throws(new InvalidOperationException("Produktua ez da existitzen: 999"));

            var controller = new ZerbitzuaController(mockRepo.Object);
            var dto = new ZerbitzuaController.ZerbitzuaSortuDto
            {
                PrezioTotala = 10,
                Data = DateTime.Now,
                MahaiakId = 5,
                Eskaerak = new List<ZerbitzuaController.EskaeraSortuDto>
                {
                    new ZerbitzuaController.EskaeraSortuDto { ProduktuaId = 999, Izena = "X", Prezioa = 1, Data = DateTime.Now, Egoera = 0 }
                }
            };

            var result = controller.Create(dto);
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Produktua ez da existitzen: 999", bad.Value);
        }

        [Fact]
        public void LortuMahaiarenZerbitzuak_ok_lista()
        {
            var mockRepo = new Mock<IZerbitzuaRepository>();
            var lista = new List<ZerbitzuaMahaiDTO>
            {
                new ZerbitzuaMahaiDTO
                {
                    Id = 1,
                    PrezioTotala = 5,
                    Data = DateTime.Now,
                    ErreserbaId = null,
                    MahaiakId = 2,
                    Eskaerak = new List<ZerbitzuaEskaeraDTO>()
                }
            };
            mockRepo.Setup(r => r.GetByMahai(2)).Returns(lista);
            var controller = new ZerbitzuaController(mockRepo.Object);
            var result = controller.GetByMahai(2);
            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<List<ZerbitzuaMahaiDTO>>(ok.Value);
            Assert.Single(returned);
        }

        [Fact]
        public void Ordaindu_ondo_200()
        {
            var mockRepo = new Mock<IZerbitzuaRepository>();
            mockRepo.Setup(r => r.Ordaindu(1)).Returns(true);
            var controller = new ZerbitzuaController(mockRepo.Object);
            var result = controller.Ordaindu(1);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void Ordaindu_ezTopatua_404()
        {
            var mockRepo = new Mock<IZerbitzuaRepository>();
            mockRepo.Setup(r => r.Ordaindu(99)).Returns(false);
            var controller = new ZerbitzuaController(mockRepo.Object);
            var result = controller.Ordaindu(99);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
