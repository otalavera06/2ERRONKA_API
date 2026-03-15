using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Modeloak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ErronkaApi.Testak
{
    public class EskaeraKontrolTest
    {
        private readonly Mock<IEskaeraRepository> _mockRepo;
        private readonly Mock<IMahaiaRepository> _mockRepoMahaia;
        private readonly Mock<IProduktuaRepository> _mockRepoProduktua;
        private readonly Mock<IEskaeraProduktuakRepository> _mockRepoEskaeraProduktuak;
        private readonly Mock<IEskaeraMahaiakRepository> _mockRepoEskaeraMahaiak;
        private readonly EskaeraKontrollerra _controller;

        public EskaeraKontrolTest()
        {
            _mockRepo = new Mock<IEskaeraRepository>();
            _mockRepoMahaia = new Mock<IMahaiaRepository>();
            _mockRepoProduktua = new Mock<IProduktuaRepository>();
            _mockRepoEskaeraProduktuak = new Mock<IEskaeraProduktuakRepository>();
            _mockRepoEskaeraMahaiak = new Mock<IEskaeraMahaiakRepository>();

            _controller = new EskaeraKontrollerra(
                _mockRepo.Object,
                _mockRepoMahaia.Object,
                _mockRepoProduktua.Object,
                _mockRepoEskaeraProduktuak.Object,
                _mockRepoEskaeraMahaiak.Object
            );
        }

    
        [Fact]
        public void SortuEskaera_mahai_librearekin_eskaera_sortzen_du()
        {
            
            var dto = new EskaeraSortuDTO
            {
                MahaiaId = 1,
                ErabiltzaileId = 1,
                Komensalak = 2,
                Produktuak = new List<EskaeraProduktuaSortuDTO>
                {
                    new EskaeraProduktuaSortuDTO { ProduktuaId = 1, Kantitatea = 2 }
                }
            };
            var mahaia = new Mahaia { id = 1, egoera = "libre", EskaeraMahaiak = new List<EskaeraMahaiak>() };
            var produktua = new Produktua { id = 1, izena = "Test", stock_aktuala = 10, prezioa = 5 };

            _mockRepoMahaia.Setup(r => r.Get(1)).Returns(mahaia);
            _mockRepoProduktua.Setup(r => r.Get(1)).Returns(produktua);

            
            var result = _controller.SortuEskaera(dto);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<Eskaera>>(okResult.Value);
            Assert.Equal(200, response.Code);
            _mockRepo.Verify(r => r.Save(It.IsAny<Eskaera>()), Times.Once);
            _mockRepoMahaia.Verify(r => r.Update(It.Is<Mahaia>(m => m.egoera == "okupatuta")), Times.Once);
            _mockRepoProduktua.Verify(r => r.Update(It.Is<Produktua>(p => p.stock_aktuala == 8)), Times.Once);
        }

        [Fact]
        public void SortuEskaera_dto_null_bada_400_itzultzen_du()
        {
            
            var result = _controller.SortuEskaera(null);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<string>>(badRequestResult.Value);
            Assert.Equal(400, response.Code);
            Assert.Equal("Datuak behar dira", response.Message);
        }

        [Fact]
        public void SortuEskaera_mahaia_ez_bada_existitzen_400_itzultzen_du()
        {
            
            var dto = new EskaeraSortuDTO { MahaiaId = 99 };
            _mockRepoMahaia.Setup(r => r.Get(99)).Returns((Mahaia)null);

            
            var result = _controller.SortuEskaera(dto);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<string>>(badRequestResult.Value);
            Assert.Equal(400, response.Code);
            Assert.Equal("Mahaia ez da aurkitu", response.Message);
        }

        [Fact]
        public void SortuEskaera_stockik_ez_badago_400_itzultzen_du()
        {
            
            var dto = new EskaeraSortuDTO
            {
                MahaiaId = 1,
                Produktuak = new List<EskaeraProduktuaSortuDTO>
                {
                    new EskaeraProduktuaSortuDTO { ProduktuaId = 1, Kantitatea = 50 }
                }
            };
            var mahaia = new Mahaia { id = 1, egoera = "libre" };
            var produktua = new Produktua { id = 1, izena = "Ardoa", stock_aktuala = 10 };

            _mockRepoMahaia.Setup(r => r.Get(1)).Returns(mahaia);
            _mockRepoProduktua.Setup(r => r.Get(1)).Returns(produktua);

            
            var result = _controller.SortuEskaera(dto);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<string>>(badRequestResult.Value);
            Assert.Equal(400, response.Code);
            Assert.Contains("Ardoa", response.Datuak);
        }

        [Fact]
        public void SortuEskaera_produktu_batzuk_ez_badira_aurkitzen_besteak_gehitzen_ditu()
        {
            
            var dto = new EskaeraSortuDTO
            {
                MahaiaId = 1,
                Produktuak = new List<EskaeraProduktuaSortuDTO>
                {
                    new EskaeraProduktuaSortuDTO { ProduktuaId = 1, Kantitatea = 1 },
                    new EskaeraProduktuaSortuDTO { ProduktuaId = 99, Kantitatea = 1 } // No existe
                }
            };
            var mahaia = new Mahaia { id = 1, egoera = "libre", EskaeraMahaiak = new List<EskaeraMahaiak>() };
            var produktua = new Produktua { id = 1, izena = "Existitzen da", stock_aktuala = 10, prezioa = 5 };

            _mockRepoMahaia.Setup(r => r.Get(1)).Returns(mahaia);
            _mockRepoProduktua.Setup(r => r.Get(1)).Returns(produktua);
            _mockRepoProduktua.Setup(r => r.Get(99)).Returns((Produktua)null);

            
            var result = _controller.SortuEskaera(dto);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<Eskaera>>(okResult.Value);
            var eskaera = response.Datuak.First();
            Assert.Single(eskaera.EskaeraProduktuak);
        }
        
        [Fact]
        public void LortuEskaerak_eskaerak_daudenean_itzultzen_ditu()
        {
            
            var eskaerak = new List<Eskaera>
            {
                new Eskaera { id = 1, sortzeData = DateTime.Now, egoera = "irekita" }
            };
            _mockRepo.Setup(r => r.LortuEskaerak2()).Returns(eskaerak);

            
            var result = _controller.LortuEskaerak();

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<EskaeraDTO>>(okResult.Value);
            Assert.Equal(200, response.Code);
            Assert.Single(response.Datuak);
        }

        [Fact]
        public void LortuEskaerak_errore_bada_500_itzultzen_du()
        {
            
            _mockRepo.Setup(r => r.LortuEskaerak2()).Throws(new Exception("Database error"));

            
            var result = _controller.LortuEskaerak();

            
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
        }
        

        [Fact]
        public void LortuEskaeraProduktuak_produktuak_itzultzen_ditu()
        {
            
            var eskaeraId = 1;
            var produktuak = new List<EskaeraProduktuak>
            {
                new EskaeraProduktuak { Produktua = new Produktua { id = 1, izena = "P1", prezioa = 10 }, Kantitatea = 2 }
            };
            _mockRepo.Setup(r => r.LortuEskaeraProduktuak2(eskaeraId)).Returns(produktuak);

            
            var result = _controller.LortuEskaeraProduktuak(eskaeraId);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<EskaeraLortuDTO>>(okResult.Value);
            Assert.Equal(200, response.Code);
            Assert.Equal(2, response.Datuak.Count); 
        }

        [Fact]
        public void LortuEskaeraProduktuak_errore_bada_500_itzultzen_du()
        {
            _mockRepo.Setup(r => r.LortuEskaeraProduktuak2(It.IsAny<int>())).Throws(new Exception("Error"));
            var result = _controller.LortuEskaeraProduktuak(1);
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
        }

        [Fact]
        public void EzabatuEskaera_eskaera_ezabatzen_du()
        {
            
            var eskaeraId = 1;
            var eskaera = new Eskaera { id = eskaeraId, EskaeraMahaiak = new List<EskaeraMahaiak>() };
            _mockRepo.Setup(r => r.Get(eskaeraId)).Returns(eskaera);
            _mockRepoEskaeraProduktuak.Setup(r => r.GetByEskaeraId(eskaeraId)).Returns(new List<EskaeraProduktuak>());

            
            var result = _controller.EzabatuEskaera(eskaeraId);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<Eskaera>>(okResult.Value);
            Assert.Equal(200, response.Code);
            _mockRepo.Verify(r => r.Delete(eskaera), Times.Once);
        }

        [Fact]
        public void EzabatuEskaera_ez_badago_400_itzultzen_du()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<int>())).Returns((Eskaera)null);
            var result = _controller.EzabatuEskaera(1);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, ((ErantzunaDTO<string>)badRequestResult.Value).Code);
        }

        [Fact]
        public void EzabatuEskaera_errore_bada_500_itzultzen_du()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<int>())).Throws(new Exception("Error"));
            var result = _controller.EzabatuEskaera(1);
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
        }

        [Fact]
        public void LortuMahaiKapasitatea_kapazitatea_itzultzen_du()
        {
            
            var mahaiaId = 1;
            var mahaia = new Mahaia { id = mahaiaId, kapazitatea = 4 };
            _mockRepoMahaia.Setup(r => r.Get(mahaiaId)).Returns(mahaia);

            
            var result = _controller.LortuMahaiKapasitatea(mahaiaId);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<int>>(okResult.Value);
            Assert.Equal(200, response.Code);
            Assert.Equal(4, response.Datuak.First());
        }

        [Fact]
        public void LortuMahaiKapasitatea_ez_badago_400_itzultzen_du()
        {
            _mockRepoMahaia.Setup(r => r.Get(It.IsAny<int>())).Returns((Mahaia)null);
            var result = _controller.LortuMahaiKapasitatea(1);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, ((ErantzunaDTO<string>)badRequestResult.Value).Code);
        }

        [Fact]
        public void LortuMahaiKapasitatea_errore_bada_500_itzultzen_du()
        {
            _mockRepoMahaia.Setup(r => r.Get(It.IsAny<int>())).Throws(new Exception("Error"));
            var result = _controller.LortuMahaiKapasitatea(1);
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
        }

        [Fact]
        public void EguneratuEskaera_eskaera_eguneratzen_du()
        {
            
            var eskaeraId = 1;
            var eskaera = new Eskaera { id = eskaeraId, EskaeraProduktuak = new List<EskaeraProduktuak>() };
            var dto = new EskaeraEguneratuDTO
            {
                Komensalak = 4,
                Produktuak = new List<EskaeraProduktuaEditatuDTO>
                {
                    new EskaeraProduktuaEditatuDTO { ProduktuaId = 1, Kantitatea = 3 }
                }
            };
            var produktua = new Produktua { id = 1, izena = "P1", stock_aktuala = 10, prezioa = 5 };

            _mockRepo.Setup(r => r.Get(eskaeraId)).Returns(eskaera);
            _mockRepo.Setup(r => r.LortuEskaeraProduktuak2(eskaeraId)).Returns(new List<EskaeraProduktuak>());
            _mockRepoProduktua.Setup(r => r.Get(1)).Returns(produktua);

            
            var result = _controller.EguneratuEskaera(eskaeraId, dto);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<string>>(okResult.Value);
            Assert.Equal(200, response.Code);
            Assert.Equal(4, eskaera.komensalak);
            _mockRepo.Verify(r => r.Update(eskaera), Times.Once);
        }

        [Fact]
        public void EguneratuEskaera_dto_null_edo_hutsik_bada_400_itzultzen_du()
        {
            var result = _controller.EguneratuEskaera(1, null);
            Assert.IsType<BadRequestObjectResult>(result);

            var dtoHutsik = new EskaeraEguneratuDTO { Produktuak = new List<EskaeraProduktuaEditatuDTO>() };
            var result2 = _controller.EguneratuEskaera(1, dtoHutsik);
            Assert.IsType<BadRequestObjectResult>(result2);
        }

        [Fact]
        public void EguneratuEskaera_ez_badago_404_itzultzen_du()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<int>())).Returns((Eskaera)null);
            var dto = new EskaeraEguneratuDTO { Produktuak = new List<EskaeraProduktuaEditatuDTO> { new EskaeraProduktuaEditatuDTO() } };
            var result = _controller.EguneratuEskaera(1, dto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, ((ErantzunaDTO<string>)notFoundResult.Value).Code);
        }

        [Fact]
        public void EguneratuEskaera_stockik_ez_badago_400_itzultzen_du()
        {
            var eskaeraId = 1;
            var eskaera = new Eskaera { id = eskaeraId, EskaeraProduktuak = new List<EskaeraProduktuak>() };
            var dto = new EskaeraEguneratuDTO { Produktuak = new List<EskaeraProduktuaEditatuDTO> { new EskaeraProduktuaEditatuDTO { ProduktuaId = 1, Kantitatea = 100 } } };
            var produktua = new Produktua { id = 1, izena = "P1", stock_aktuala = 10 };

            _mockRepo.Setup(r => r.Get(eskaeraId)).Returns(eskaera);
            _mockRepo.Setup(r => r.LortuEskaeraProduktuak2(eskaeraId)).Returns(new List<EskaeraProduktuak>());
            _mockRepoProduktua.Setup(r => r.Get(1)).Returns(produktua);

            var result = _controller.EguneratuEskaera(eskaeraId, dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Stock nahikorik ez", ((ErantzunaDTO<string>)badRequestResult.Value).Message);
        }

        [Fact]
        public void EguneratuEskaera_errore_bada_500_itzultzen_du()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<int>())).Throws(new Exception("Error"));
            var dto = new EskaeraEguneratuDTO { Produktuak = new List<EskaeraProduktuaEditatuDTO> { new EskaeraProduktuaEditatuDTO() } };
            var result = _controller.EguneratuEskaera(1, dto);
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
        }

        [Fact]
        public void EguneratuSukaldeaEgoera_egoera_aldatzen_du()
        {
            
            var eskaeraId = 1;
            var eskaera = new Eskaera { id = eskaeraId, sukaldeaEgoera = "zain" };
            var dto = new EskaeraSukaldeaEgoeraDTO { SukaldeaEgoera = "hasi" };

            _mockRepo.Setup(r => r.Get(eskaeraId)).Returns(eskaera);

            
            var result = _controller.EguneratuSukaldeaEgoera(eskaeraId, dto);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<string>>(okResult.Value);
            Assert.Equal(200, response.Code);
            Assert.Equal("hasi", eskaera.sukaldeaEgoera);
            _mockRepo.Verify(r => r.Update(eskaera), Times.Once);
        }

        [Fact]
        public void EguneratuSukaldeaEgoera_baliogabea_bada_400_itzultzen_du()
        {
            var dto = new EskaeraSukaldeaEgoeraDTO { SukaldeaEgoera = "invalid" };
            var result = _controller.EguneratuSukaldeaEgoera(1, dto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void EguneratuSukaldeaEgoera_ez_badago_404_itzultzen_du()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<int>())).Returns((Eskaera)null);
            var dto = new EskaeraSukaldeaEgoeraDTO { SukaldeaEgoera = "hasi" };
            var result = _controller.EguneratuSukaldeaEgoera(1, dto);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void EguneratuSukaldeaEgoera_errore_bada_500_itzultzen_du()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<int>())).Throws(new Exception("Error"));
            var dto = new EskaeraSukaldeaEgoeraDTO { SukaldeaEgoera = "hasi" };
            var result = _controller.EguneratuSukaldeaEgoera(1, dto);
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
        }

        [Fact]
        public void OrdainduEskaera_eskaera_badago_egoera_aldatzen_du()
        {
            
            var eskaeraId = 1;
            var eskaera = new Eskaera { id = eskaeraId, egoera = "irekita" };
            _mockRepo.Setup(r => r.Get(eskaeraId)).Returns(eskaera);

            
            var result = _controller.OrdainduEskaera(eskaeraId);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<string>>(okResult.Value);
            Assert.Equal(200, response.Code);
            _mockRepo.Verify(r => r.Update(It.Is<Eskaera>(e => e.egoera == "ordainketa_pendiente")), Times.Once);
        }

        [Fact]
        public void OrdainduEskaera_eskaera_ez_bada_aurkitu_404_itzultzen_du()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<int>())).Returns((Eskaera)null);
            var result = _controller.OrdainduEskaera(99);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, ((ErantzunaDTO<string>)notFoundResult.Value).Code);
        }

        [Fact]
        public void OrdainduEskaera_errore_bada_500_itzultzen_du()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<int>())).Throws(new Exception("Error"));
            var result = _controller.OrdainduEskaera(1);
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
        }
        
        [Fact]
        public void LortuEskaerakOrdaintzeko_eskaerak_daudenean_itzultzen_ditu()
        {
            
            var eskaerak = new List<Eskaera>
            {
                new Eskaera { id = 3, sortzeData = DateTime.Now, egoera = "ordainketa_pendiente" }
            };
            _mockRepo.Setup(r => r.LortuEskaerakOrdaintzeko()).Returns(eskaerak);

            
            var result = _controller.LortuEskaerakOrdaintzeko();

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ErantzunaDTO<EskaeraDTO>>(okResult.Value);
            Assert.Equal(200, response.Code);
            Assert.Single(response.Datuak);
        }

        [Fact]
        public void LortuEskaerakOrdaintzeko_errore_bada_500_itzultzen_du()
        {
            _mockRepo.Setup(r => r.LortuEskaerakOrdaintzeko()).Throws(new Exception("Error"));
            var result = _controller.LortuEskaerakOrdaintzeko();
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
        }
       
    }
}
