using Microsoft.AspNetCore.Mvc;
using ErronkaApi.Repositorioak;
using ErronkaApi.DTOak;
using System.Linq;
using ErronkaApi.Interfaces;
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
        private readonly IKategoriaRepository _repo;
        public KategoriaKontrollerra(IKategoriaRepository repo)
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
            var kategoriak = _repo.GetAllDTO();
            return Ok(kategoriak);
        }
    }
}
