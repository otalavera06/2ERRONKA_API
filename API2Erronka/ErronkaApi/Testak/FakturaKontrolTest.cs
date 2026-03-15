using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Modeloak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ErronkaApi.Testak
{
    public class FakturaKontrolTest
    {
        [Fact]
        public void SortuFaktura_existitzen_den_eskaera_sartuta_faktura_sortzen_du()
        {
            
            var mockRepoEskaera = new Mock<IEskaeraRepository>();
            var mockRepoMahaia = new Mock<IMahaiaRepository>();
            var mockRepoProduktua = new Mock<IProduktuaRepository>();
            var mockRepoEskaeraProduktuak = new Mock<IEskaeraProduktuakRepository>();

            var eskaeraId = 1;
            var eskaera = new Eskaera { id = eskaeraId, mahaia_id = 1, sortzeData = DateTime.Now };
            
            mockRepoEskaera.Setup(r => r.Get(eskaeraId)).Returns(eskaera);
            mockRepoEskaera.Setup(r => r.LortuEskaeraProduktuak2(eskaeraId)).Returns(new List<EskaeraProduktuak>());

            var controller = new FakturaKontrollerra(
                mockRepoEskaera.Object,
                mockRepoMahaia.Object,
                mockRepoProduktua.Object,
                mockRepoEskaeraProduktuak.Object
            );

            
            var result = controller.SortuFaktura(eskaeraId);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<string>>(okResult.Value);
            Assert.Equal(200, response.Code);
            Assert.Contains("Faktura ongi sortuta", response.Message);
            
            mockRepoEskaera.Verify(r => r.Update(It.IsAny<Eskaera>()), Times.Once);
        }

        [Fact]
        public void SortuFaktura_eskaera_ez_da_existitzen_404_itzultzen_du()
        {
            
            var mockRepoEskaera = new Mock<IEskaeraRepository>();
            var mockRepoMahaia = new Mock<IMahaiaRepository>();
            var mockRepoProduktua = new Mock<IProduktuaRepository>();
            var mockRepoEskaeraProduktuak = new Mock<IEskaeraProduktuakRepository>();

            mockRepoEskaera.Setup(r => r.Get(It.IsAny<int>())).Returns((Eskaera)null);

            var controller = new FakturaKontrollerra(
                mockRepoEskaera.Object,
                mockRepoMahaia.Object,
                mockRepoProduktua.Object,
                mockRepoEskaeraProduktuak.Object
            );

            
            var result = controller.SortuFaktura(99);

            
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<string>>(notFoundResult.Value);
            Assert.Equal(404, response.Code);
        }
    }
}
