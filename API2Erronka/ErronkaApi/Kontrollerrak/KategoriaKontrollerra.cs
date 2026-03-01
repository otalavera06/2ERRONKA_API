using Microsoft.AspNetCore.Mvc;
using ErronkaApi.Repositorioak;
using ErronkaApi.DTOak;
using System.Linq;
namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Kategoriak kudeatzeko kontroladorea.
    /// Produktuen kategoriak zerrendatzeko balio du.
    /// </summary>
    [ApiController]
    [Route("api/Kategoria")]
    public class KategoriaKontrollerra : ControllerBase
    {
        private readonly KategoriaRepository _repo;
        public KategoriaKontrollerra(KategoriaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Kategoria guztiak lortzen ditu.
        /// </summary>
        /// <returns>Kategorien zerrenda.</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var kategoriak = _repo.GetAll()
                                    .Select(k => new KategoriaDTO
                                    {
                                        id = k.id,
                                        izena = k.izena
                                    })
                                    .ToList();
            return Ok(kategoriak);
        }
    }
}
