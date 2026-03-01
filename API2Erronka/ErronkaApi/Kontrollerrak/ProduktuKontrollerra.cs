using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Produktuak kudeatzeko kontroladorea.
    /// Produktuak kategoriaren arabera lortzeko aukera ematen du.
    /// </summary>
    [ApiController]
    [Route("api/Produktuak")]
    public class ProduktuakKontrollera : ControllerBase
    {
        private readonly ProduktuaRepository _repo;

        public ProduktuakKontrollera(ProduktuaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Kategoria jakin bateko produktuak lortzen ditu.
        /// </summary>
        /// <param name="kategoriaId">Kategoriaren IDa.</param>
        /// <returns>Kategoria horretako produktuen zerrenda.</returns>
        [HttpGet("kategoria/{kategoriaId}")]
        public IActionResult GetByKategoria(int kategoriaId)
        {
            var produktuak = _repo.GetAll()
                                   .Where(p => p.kategoria.id == kategoriaId)
                                   .Select(p => new ProduktuaDTO
                                   {
                                       id = p.id,
                                       izena = p.izena,
                                       prezioa = (decimal)p.prezioa,
                                       kategoria_id = p.kategoria.id,
                                       stock_aktuala = p.stock_aktuala
                                   })
                                   .ToList();

            return Ok(produktuak);
        }
    }
}
