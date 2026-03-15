using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ErronkaApi.Interfaces;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Mahaiak kudeatzeko kontroladorea.
    /// Mahai libreak lortzeko funtzioak eskaintzen ditu.
    /// </summary>
    [ApiController]
    [Route("api/mahaiak")]
    public class MahaiakController : ControllerBase
    {
        private readonly IMahaiaRepository _mahaiaService;

        public MahaiakController(IMahaiaRepository mahaiaService)
        {
            _mahaiaService = mahaiaService;
        }

        /// <summary>
        /// Mahai libreak lortzen ditu.
        /// </summary>
        /// <returns>Mahai libreen zerrenda ErantzunaDTO formatuan.</returns>
        [HttpGet("libreak")]
        public ActionResult<ErantzunaDTO<List<MahaiaDTO>>> LortuMahaiLibre()
        {
            var mahaiLibreak = _mahaiaService.LortuMahaiLibre();

            if(mahaiLibreak == null)
            {
                return StatusCode(500, new ErantzunaDTO<List<MahaiaDTO>>
                {
                    Code = 500,
                    Message = "Errorea gertatu da",
                    Datuak = null
                });
            }

            if (!mahaiLibreak.Any())
            {
                return Ok(new ErantzunaDTO<List<MahaiaDTO>>
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
