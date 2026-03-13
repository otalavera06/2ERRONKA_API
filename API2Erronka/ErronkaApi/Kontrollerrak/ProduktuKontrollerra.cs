using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
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
        private readonly IProduktuaRepository _repo;

        public ProduktuakKontrollera(IProduktuaRepository repo)
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
            var produktuak = _repo.GetAllByKategoriaId(kategoriaId);

            return Ok(produktuak);
        }
    }
}
