using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Modeloak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHibernate;
using Xunit;


namespace ErronkaApi.Testak
{
    public class ProduktuKontrolTest
    {
        [Fact]
        public void GetByKategoria_existitzen_den_kategoria_sartuta_kategoria_horretako_produktuak_itzultzen_ditu()
        {
            var mockRepo = new Mock<IProduktuaRepository>();

            var kategoria = new Kategoria(1, "KAT1");
            var produktua1kat1 = new Produktua(1, "Produktua KAT1EKOA", kategoria, 2, 5);
            var produktua2kat1 = new Produktua(1, "Produktua2 KAT1EKOA", kategoria, 2, 5);
            var kategoria2 = new Kategoria(1, "KAT2");
            var produktua1kat2 = new Produktua(1, "Produktua KAT2EKOA", kategoria, 2, 5);

            List<ProduktuaDTO> produktuZerrenda = new List<ProduktuaDTO>();
            ProduktuaDTO p1k1dto = new ProduktuaDTO(produktua1kat1);
            ProduktuaDTO p2k1dto = new ProduktuaDTO(produktua2kat1);
            ProduktuaDTO p1k2dto = new ProduktuaDTO(produktua1kat2);

            produktuZerrenda.Add(p1k1dto);
            produktuZerrenda.Add(p2k1dto);

            mockRepo.Setup(r => r.GetAllByKategoriaId(kategoria.id)).Returns(produktuZerrenda);

            var controller = new ProduktuakKontrollera(mockRepo.Object);

            var result = controller.GetByKategoria(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            var returnedProduktuakList = Assert.IsType<List<ProduktuaDTO>>(okResult.Value);

            Assert.Contains(p1k1dto, returnedProduktuakList);
            Assert.Contains(p2k1dto, returnedProduktuakList);
            Assert.DoesNotContain(p1k2dto, returnedProduktuakList);

        }
    }
}