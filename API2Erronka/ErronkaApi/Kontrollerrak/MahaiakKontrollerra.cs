using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErronkaApi.Interfaces;
using ErronkaApi.NHibernate;

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

        public record LoginRequest(string erabiltzailea, string pasahitza);

        /// <summary>
        /// Mugikorreko mahaien login-a egiten du. m1, m2... edo "Mahaia 1" bezalako erabiltzaileak onartzen ditu.
        /// </summary>
        /// <param name="req">Erabiltzailea eta pasahitza.</param>
        /// <returns>Mahaiaren datuak edo 401.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.erabiltzailea) || string.IsNullOrWhiteSpace(req.pasahitza))
            {
                return Unauthorized();
            }

            var mahaiaId = LortuMahaiaId(req.erabiltzailea);
            if (!mahaiaId.HasValue || !PasahitzaBaliozkoa(req.pasahitza))
            {
                return Unauthorized();
            }

            using var session = NHibernateHelper.OpenSession();
            var row = session.CreateSQLQuery(
                    @"SELECT id, izena
                      FROM mahaiak
                      WHERE id = :id
                      LIMIT 1")
                .SetParameter("id", mahaiaId.Value)
                .UniqueResult<object[]>();

            if (row == null)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                Id = Convert.ToInt32(row[0]),
                Izena = row[1] == DBNull.Value ? $"Mahaia {mahaiaId.Value}" : row[1]?.ToString(),
                Erabiltzailea = req.erabiltzailea,
                chatBaimena = true
            });
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

        private static int? LortuMahaiaId(string erabiltzailea)
        {
            var garbituta = erabiltzailea.Trim().ToLowerInvariant()
                .Replace("mahaia", string.Empty)
                .Replace("mesa", string.Empty)
                .Replace("m", string.Empty)
                .Replace(" ", string.Empty);

            return int.TryParse(garbituta, out var id) && id > 0 ? id : null;
        }

        private static bool PasahitzaBaliozkoa(string pasahitza)
        {
            var p = pasahitza.Trim();
            return p == "123" || p == "1234";
        }

    }
}
