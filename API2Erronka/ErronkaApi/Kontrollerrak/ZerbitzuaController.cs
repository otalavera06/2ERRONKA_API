using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ErronkaApi.Kontrollerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZerbitzuaController : ControllerBase
    {
        public class EskaeraSortuDto
        {
            public int ProduktuaId { get; set; }
            public string Izena { get; set; }
            public decimal Prezioa { get; set; }
            public DateTime Data { get; set; }
            public int Egoera { get; set; }
        }

        public class ZerbitzuaSortuDto
        {
            public decimal PrezioTotala { get; set; }
            public DateTime Data { get; set; }
            public int? ErreserbaId { get; set; }
            public int? MahaiakId { get; set; }
            public IList<EskaeraSortuDto> Eskaerak { get; set; } = new List<EskaeraSortuDto>();
        }

        [HttpPost]
        public IActionResult Create([FromBody] ZerbitzuaSortuDto dto)
        {
            if (!dto.MahaiakId.HasValue) return BadRequest("MahaiakId is required");

            using var session = NHibernateHelper.OpenSession();
            using var tx = session.BeginTransaction();

            var eskaera = new Eskaera
            {
                erabiltzaileId = 1,
                mahaia_id = dto.MahaiakId.Value,
                komensalak = 0,
                egoera = "irekita",
                sukaldeaEgoera = "zain",
                sortzeData = dto.Data == default ? DateTime.Now : dto.Data,
                itxieraData = null
            };

            session.Save(eskaera);

            var mahaia = session.Query<Mahaia>().FirstOrDefault(m => m.id == dto.MahaiakId.Value);
            if (mahaia != null)
            {
                var link = new EskaeraMahaiak
                {
                    Eskaera = eskaera,
                    Mahaia = mahaia
                };
                session.Save(link);
            }

            foreach (var e in dto.Eskaerak)
            {
                var produktua = session.Query<Produktua>().FirstOrDefault(p => p.id == e.ProduktuaId);
                if (produktua == null) return BadRequest($"Produktua ez da existitzen: {e.ProduktuaId}");

                var ep = new EskaeraProduktuak
                {
                    Eskaera = eskaera,
                    Produktua = produktua,
                    Kantitatea = 1,
                    PrezioUnitarioa = e.Prezioa,
                    Guztira = e.Prezioa
                };
                session.Save(ep);
            }

            tx.Commit();

            return CreatedAtAction(nameof(GetByMahai), new { mahaiaId = dto.MahaiakId.Value }, new { Id = eskaera.id });
        }

        [HttpGet("mahaia/{mahaiaId:int}")]
        public IActionResult GetByMahai(int mahaiaId)
        {
            using var session = NHibernateHelper.OpenSession();

            var eskaerak = session.Query<Eskaera>()
                .Where(e => e.mahaia_id == mahaiaId)
                .OrderByDescending(e => e.sortzeData)
                .Take(50)
                .ToList();

            var result = eskaerak.Select(e =>
            {
                var produktuak = session.Query<EskaeraProduktuak>()
                    .Where(ep => ep.Eskaera.id == e.id)
                    .ToList();

                var egoeraInt = string.Equals(e.egoera, "itxita", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
                var prezioTotala = produktuak.Sum(ep => ep.PrezioUnitarioa * ep.Kantitatea);

                return new
                {
                    Id = e.id,
                    PrezioTotala = prezioTotala,
                    Data = e.sortzeData,
                    ErreserbaId = (int?)null,
                    MahaiakId = (int?)e.mahaia_id,
                    Eskaerak = produktuak.Select(ep => new
                    {
                        Id = ep.Id,
                        ProduktuaId = ep.Produktua.id,
                        Izena = ep.Produktua.izena,
                        Data = e.sortzeData,
                        Prezioa = ep.PrezioUnitarioa,
                        Egoera = egoeraInt
                    }).ToList()
                };
            }).ToList();

            return Ok(result);
        }

        [HttpPost("{id:int}/ordaindu")]
        public IActionResult Ordaindu(int id)
        {
            using var session = NHibernateHelper.OpenSession();
            using var tx = session.BeginTransaction();

            var eskaera = session.Query<Eskaera>().FirstOrDefault(e => e.id == id);
            if (eskaera == null) return NotFound();

            eskaera.egoera = "itxita";
            eskaera.itxieraData = DateTime.Now;
            session.Update(eskaera);

            tx.Commit();
            return Ok();
        }
    }
}
