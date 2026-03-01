using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using Microsoft.AspNetCore.Mvc;
using ErronkaApi.Repositorioak;
using ErronkaApi.NHibernate;
using System.Linq;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Eskaerak kudeatzeko kontroladorea.
    /// Eskaerak sortu, kontsultatu, eguneratu eta ezabatzeko aukera ematen du.
    /// </summary>
    [ApiController]
    [Route("api/eskaerak")]
    public class EskaeraKontrollerra : ControllerBase
    {
        private readonly EskaeraRepository _repo;

        public EskaeraKontrollerra()
        {
            _repo = new EskaeraRepository(NHibernateHelper.SessionFactory);
        }

        /// <summary>
        /// Eskaera berri bat sortzen du.
        /// </summary>
        /// <param name="dto">Eskaera berriaren datuak.</param>
        /// <returns>Sortutako eskaeraren informazioa.</returns>
        [HttpPost]
        public IActionResult SortuEskaera([FromBody] EskaeraSortuDTO dto)
        {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

            var erantzuna = _repo.SortuEskaera(dto);

            if (erantzuna.Code == 200)
            {
                return Ok(erantzuna);
                
            }
            else {
                return BadRequest(erantzuna);
            }
        }

        /// <summary>
        /// Eskaera guztiak lortzen ditu.
        /// </summary>
        /// <returns>Eskaeren zerrenda.</returns>
        [HttpGet]
        public IActionResult LortuEskaerak()
        {
            var erantzuna = _repo.LortuEskaerak();

            return StatusCode(erantzuna.Code, erantzuna);
        }

        /// <summary>
        /// Eskaera baten produktuak lortzen ditu.
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <returns>Eskaeraren produktuen zerrenda.</returns>
        [HttpGet("{eskaeraId}/produktuak")]
        public IActionResult LortuEskaeraProduktuak(int eskaeraId)
        {
            var erantzuna = _repo.LortuEskaeraProduktuak(eskaeraId);

            return StatusCode(erantzuna.Code, erantzuna);
        }

        /// <summary>
        /// Eskaera bat ezabatzen du.
        /// </summary>
        /// <param name="eskaeraId">Ezabatu nahi den eskaeraren IDa.</param>
        /// <returns>Erantzunaren emaitza.</returns>
        [HttpDelete("{eskaeraId}")]
        public IActionResult EzabatuEskaera(int eskaeraId)
        {
            var erantzuna = _repo.EzabatuEskaera(eskaeraId);

            if (erantzuna.Code == 200)
            {
                return Ok(erantzuna);
            }
            else
            {
                return BadRequest(erantzuna);
            }
        }

        /// <summary>
        /// Mahai baten kapazitatea lortzen du.
        /// </summary>
        /// <param name="mahaiaId">Mahaiaren IDa.</param>
        /// <returns>Mahaiaren kapazitatea.</returns>
        [HttpGet("mahaiak/{mahaiaId}/kapazitatea")]
        public IActionResult LortuMahaiKapasitatea(int mahaiaId)
        {
            var erantzuna = _repo.LortuMahaiKapazitatea(mahaiaId);

            if (erantzuna.Code == 200)
                return Ok(erantzuna);
            else if (erantzuna.Code == 404)
                return NotFound(erantzuna);
            else
                return StatusCode(500, erantzuna);
        }

        /// <summary>
        /// Eskaera bat eguneratzen du (komensalak eta produktuak).
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <param name="dto">Eguneratu nahi diren datuak.</param>
        /// <returns>Eguneratzearen emaitza.</returns>
        [HttpPut("{eskaeraId}")]
        public IActionResult EguneratuEskaera(
        int eskaeraId,
        [FromBody] EskaeraEguneratuDTO dto)
        {
            if (dto == null || dto.Produktuak == null || !dto.Produktuak.Any())
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = "Ez duzu produkturik bidali",
                    Datuak = new List<string>()
                });
            }

            var erantzuna = _repo.EguneratuEskaera(eskaeraId, dto.Komensalak, dto.Produktuak);

            if (erantzuna.Code == 200)
                return Ok(erantzuna);
            else if (erantzuna.Code == 404)
                return NotFound(erantzuna);
            else if (erantzuna.Code == 400)
                return BadRequest(erantzuna);
            else
                return StatusCode(500, erantzuna);
        }

        /// <summary>
        /// Eskaera baten sukaldeko egoera eguneratzen du.
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <param name="dto">Sukaldeko egoera berria.</param>
        /// <returns>Eguneratzearen emaitza.</returns>
        [HttpPut("{eskaeraId}/sukaldea-egoera")]
        public IActionResult EguneratuSukaldeaEgoera(int eskaeraId, [FromBody] EskaeraSukaldeaEgoeraDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = "Datuak behar dira",
                    Datuak = new List<string>()
                });
            }

            var erantzuna = _repo.EguneratuSukaldeaEgoera(eskaeraId, dto.SukaldeaEgoera);

            if (erantzuna.Code == 200)
                return Ok(erantzuna);
            else if (erantzuna.Code == 404)
                return NotFound(erantzuna);
            else if (erantzuna.Code == 400)
                return BadRequest(erantzuna);
            else
                return StatusCode(500, erantzuna);
        }

        /// <summary>
        /// Eskaera bat ordaintzera bidaltzen du.
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <returns>Emaitza.</returns>
        [HttpPost("{eskaeraId}/ordainduEskaera")]
        public IActionResult OrdainduEskaera(int eskaeraId)
        {
            var erantzuna = _repo.OrdaintzeraBidali(eskaeraId);

            if (erantzuna.Code == 200)
                return Ok(erantzuna);
            else if (erantzuna.Code == 404)
                return NotFound(erantzuna);
            else if (erantzuna.Code == 400)
                return BadRequest(erantzuna);
            else
                return StatusCode(500, erantzuna);
        }

        /// <summary>
        /// Eskaera baten faktura sortzen du.
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <returns>Emaitza.</returns>
        [HttpPost("{eskaeraId}/sortuFaktura")]
        public IActionResult SortuFaktura(int eskaeraId)
        {
            var erantzuna = _repo.SortuFaktura(eskaeraId);

            if (erantzuna.Code == 200)
                return Ok(erantzuna);
            else if (erantzuna.Code == 404)
                return NotFound(erantzuna);
            else
                return StatusCode(500, erantzuna);
        }

        /// <summary>
        /// Ordaintzeko dauden eskaerak lortzen ditu.
        /// </summary>
        /// <returns>Ordaintzeko dauden eskaeren zerrenda.</returns>
        [HttpGet("ordainketa-pendiente")]
        public IActionResult LortuEskaerakOrdaintzeko()
        {
            var erantzuna = _repo.LortuEskaerakOrdaintzeko();

            if (erantzuna.Code == 200)
                return Ok(erantzuna);
            else
                return StatusCode(erantzuna.Code, erantzuna);
        }
    }
}
