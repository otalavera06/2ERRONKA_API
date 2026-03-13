using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Mahaiak kudeatzeko kontroladorea.
    /// Mahai libreak lortzeko funtzioak eskaintzen ditu.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MahaiakController : ControllerBase
    {
        private readonly MahaiaRepository _mahaiaService;

        public MahaiakController(MahaiaRepository mahaiaService)
        {
            _mahaiaService = mahaiaService;
        }

        /// <summary>
        /// Mahai libre bat lortzen du.
        /// </summary>
        /// <returns>Mahai libre baten informazioa.</returns>
        [HttpGet("libre2")]
        public async Task<ActionResult<ErantzunaDTO<MahaiaDTO>>> LortuMahaiLibre()
        {
            var mahaiLibreak = _mahaiaService.LortuMahaiLibreAsync();

            if(mahaiLibreak == null)
            {
                return StatusCode(500, new ErantzunaDTO<MahaiaDTO>
                {
                    Code = 500,
                    Message = "Errorea gertatu da",
                    Datuak = null
                });
            }

            if (!mahaiLibreak.Any())
            {
                return Ok(new ErantzunaDTO<MahaiaDTO>
                {
                    Code = 201,
                    Message = "Ez dago mahai librerik",
                    Datuak = null
                }); 
            }

            return Ok(new ErantzunaDTO<MahaiaDTO>
            {
                Code = 200,
                Message = "Mahai libreak lortu dira",
                Datuak = mahaiLibreak
            });
        }

    }
}
