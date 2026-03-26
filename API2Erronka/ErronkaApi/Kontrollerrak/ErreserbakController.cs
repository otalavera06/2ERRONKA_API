using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Linq;

namespace ErronkaApi.Kontrollerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErreserbakController : ControllerBase
    {
        public class ErreserbakSortuDto
        {
            public DateTime Data { get; set; }
            public bool Mota { get; set; }
            public int? ErabiltzaileakId { get; set; }
            public int MahaiakId { get; set; }
        }

        public class ErreserbakUpdateDto
        {
            public DateTime? Data { get; set; }
            public bool? Mota { get; set; }
            public int? ErabiltzaileakId { get; set; }
            public int? MahaiakId { get; set; }
        }

        [HttpGet]
        public IActionResult GetByDate([FromQuery] string data, [FromQuery] bool mota)
        {
            if (!DateTime.TryParseExact(data, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var eguna))
                return BadRequest("data format: yyyy-MM-dd");

            using var session = NHibernateHelper.OpenSession();
            var list = session.Query<Erreserba>()
                .Where(r => r.Data.Date == eguna.Date && r.Mota == mota)
                .Select(r => new
                {
                    Id = r.Id,
                    Data = r.Data,
                    Mota = r.Mota,
                    ErabiltzaileakId = r.ErabiltzaileakId,
                    MahaiakId = r.MahaiakId
                })
                .ToList();

            return Ok(list);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ErreserbakSortuDto dto)
        {
            using var session = NHibernateHelper.OpenSession();
            using var tx = session.BeginTransaction();

            var entity = new Erreserba
            {
                Data = dto.Data,
                Mota = dto.Mota,
                ErabiltzaileakId = dto.ErabiltzaileakId,
                MahaiakId = dto.MahaiakId
            };

            session.Save(entity);
            tx.Commit();

            return Ok(new
            {
                Id = entity.Id,
                Data = entity.Data,
                Mota = entity.Mota,
                ErabiltzaileakId = entity.ErabiltzaileakId,
                MahaiakId = entity.MahaiakId
            });
        }

        [HttpPut("mahaia/{mahaiaId:int}")]
        public IActionResult UpdateByMahai(int mahaiaId, [FromQuery] string data, [FromQuery] bool mota, [FromBody] ErreserbakUpdateDto dto)
        {
            if (!DateTime.TryParseExact(data, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var eguna))
                return BadRequest("data format: yyyy-MM-dd");

            using var session = NHibernateHelper.OpenSession();
            using var tx = session.BeginTransaction();

            var entity = session.Query<Erreserba>()
                .FirstOrDefault(r => r.MahaiakId == mahaiaId && r.Data.Date == eguna.Date && r.Mota == mota);
            if (entity == null) return NotFound();

            if (dto.Data.HasValue) entity.Data = dto.Data.Value;
            if (dto.Mota.HasValue) entity.Mota = dto.Mota.Value;
            if (dto.ErabiltzaileakId.HasValue) entity.ErabiltzaileakId = dto.ErabiltzaileakId;
            if (dto.MahaiakId.HasValue) entity.MahaiakId = dto.MahaiakId.Value;

            session.Update(entity);
            tx.Commit();

            return NoContent();
        }

        [HttpDelete("mahaia/{mahaiaId:int}")]
        public IActionResult DeleteByMahai(int mahaiaId, [FromQuery] string data, [FromQuery] bool mota)
        {
            if (!DateTime.TryParseExact(data, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var eguna))
                return BadRequest("data format: yyyy-MM-dd");

            using var session = NHibernateHelper.OpenSession();
            using var tx = session.BeginTransaction();

            var entity = session.Query<Erreserba>()
                .FirstOrDefault(r => r.MahaiakId == mahaiaId && r.Data.Date == eguna.Date && r.Mota == mota);
            if (entity == null) return NotFound();

            session.Delete(entity);
            tx.Commit();

            return NoContent();
        }
    }
}
