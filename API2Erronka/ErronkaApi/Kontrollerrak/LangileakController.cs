using ErronkaApi.NHibernate;
using ErronkaApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Langileen kudeaketa eta saioa hasteko endpoint-a.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LangileakController : ControllerBase
    {
        private readonly IErabiltzaileaRepository _repo;

        public LangileakController(IErabiltzaileaRepository repo)
        {
            _repo = repo;
        }

        public record LoginRequest(string erabiltzailea, string pasahitza);

        /// <summary>
        /// Saioa hasten du erabiltzaile eta pasahitzarekin.
        /// </summary>
        /// <param name="req">Saioa hasteko datuak.</param>
        /// <returns>Erabiltzailearen informazioa edo 401.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    var row = (object[])session.CreateSQLQuery(
                            @"SELECT id, izena, abizena, erabiltzailea, email, telefonoa, baimena, mahaiak_id, chat_baimena
                              FROM langileak
                              WHERE erabiltzailea = :u AND pasahitza = :p
                              LIMIT 1")
                        .SetParameter("u", req.erabiltzailea)
                        .SetParameter("p", req.pasahitza)
                        .UniqueResult();

                    if (row != null)
                    {
                        var baimena = row[6] != null && row[6] != DBNull.Value && Convert.ToInt32(row[6]) != 0;
                        var mahaiakId = row[7] == null || row[7] == DBNull.Value ? (int?)null : Convert.ToInt32(row[7]);
                        var chatBaimena = row[8] != null && row[8] != DBNull.Value && Convert.ToInt32(row[8]) != 0;

                        return Ok(new
                        {
                            Id = Convert.ToInt32(row[0]),
                            Izena = row[1] == DBNull.Value ? null : row[1]?.ToString(),
                            Abizena = row[2] == DBNull.Value ? null : row[2]?.ToString(),
                            Erabiltzailea = row[3] == DBNull.Value ? null : row[3]?.ToString(),
                            Email = row[4] == DBNull.Value ? null : row[4]?.ToString(),
                            Telefonoa = row[5] == DBNull.Value ? null : row[5]?.ToString(),
                            Baimena = baimena,
                            MahaiakId = mahaiakId,
                            chatBaimena = chatBaimena
                        });
                    }
                }
            }
            catch
            {
            }

            var erabiltzaileaFallback = _repo.Login(req.erabiltzailea, req.pasahitza);
            if (erabiltzaileaFallback != null)
            {
                var baimenaFallback = erabiltzaileaFallback.rola?.id == 1;
                return Ok(new
                {
                    Id = erabiltzaileaFallback.id,
                    Izena = (string?)null,
                    Abizena = (string?)null,
                    Erabiltzailea = erabiltzaileaFallback.erabiltzailea,
                    Email = erabiltzaileaFallback.emaila,
                    Telefonoa = (string?)null,
                    Baimena = baimenaFallback,
                    MahaiakId = (int?)null,
                    chatBaimena = erabiltzaileaFallback.txat
                });
            }

            return Unauthorized();
        }

        [HttpGet("{id}/txat-baimena")]
        public IActionResult CheckTxatBaimena(int id)
        {
            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    var chatBaimenaRaw = session.CreateSQLQuery(
                            @"SELECT chat_baimena FROM langileak WHERE id = :id LIMIT 1")
                        .SetParameter("id", id)
                        .UniqueResult();

                    if (chatBaimenaRaw != null)
                    {
                        var chatBaimena = chatBaimenaRaw != DBNull.Value && Convert.ToInt32(chatBaimenaRaw) != 0;
                        return Ok(new { chatBaimena = chatBaimena });
                    }
                }
            }
            catch
            {
            }

            var erabiltzaileaFallback = _repo.LortuErabiltzailea(id);
            if (erabiltzaileaFallback != null)
            {
                return Ok(new { chatBaimena = erabiltzaileaFallback.txat });
            }

            return NotFound();
        }
    }
}
