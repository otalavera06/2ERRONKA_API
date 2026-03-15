using ErronkaApi.Kontrollerrak;
using ErronkaApi.Modeloak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ErronkaApi.Interfaces;
using ErronkaApi.DTOak;

namespace ErronkaApi.Testak
{
    public class KategoriaKontrolTest
    {

        [Fact]
        public void GetAll_kategoriak_badira_eta_itzultzen_ditu()
        {
            var mockRepo = new Mock<IKategoriaRepository>();

            var k1 = new Kategoria(1, "Edariak");
            var k2 = new Kategoria(2, "Janaria");
            var k3 = new Kategoria(3, "Arropa");

            var dto1 = new KategoriaDTO { id = k1.id, izena = k1.izena };
            var dto2 = new KategoriaDTO { id = k2.id, izena = k2.izena };
            var dto3 = new KategoriaDTO { id = k3.id, izena = k3.izena };

            var kategoriak = new List<KategoriaDTO> { dto1, dto2};

            mockRepo.Setup(r => r.GetAllDTO()).Returns(kategoriak);

            var controller = new KategoriaKontrollerra(mockRepo.Object);

            var result = controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var itzulitakoZerrenda = Assert.IsType<List<KategoriaDTO>>(ok.Value);

            Assert.Contains(dto1, itzulitakoZerrenda);
            Assert.Contains(dto2, itzulitakoZerrenda);

            Assert.DoesNotContain(dto3, itzulitakoZerrenda);
        }



        [Fact]
        public void GetAll_kategoriarik_ez_lista_hutsa_itzultzen_du()
        {
            var mockRepo = new Mock<IKategoriaRepository>();

            mockRepo.Setup(r => r.GetAllDTO()).Returns(new List<KategoriaDTO>());

            var controller = new KategoriaKontrollerra(mockRepo.Object);

            var result = controller.GetAll();

            
            var ok = Assert.IsType<OkObjectResult>(result);
            var itzulitakoZerrenda = Assert.IsType<List<KategoriaDTO>>(ok.Value);

            Assert.Empty(itzulitakoZerrenda);
        }

        [Fact]
        public void GetAll_repoak_salbuespena_botatzen_du()
        {
            var mockRepo = new Mock<IKategoriaRepository>();

            mockRepo.Setup(r => r.GetAllDTO())
                    .Throws(new System.Exception("DB error"));

            var controller = new KategoriaKontrollerra(mockRepo.Object);

            Assert.Throws<System.Exception>(() => controller.GetAll());
        }
    }
}