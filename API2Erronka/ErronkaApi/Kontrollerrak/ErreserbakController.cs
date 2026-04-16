using ErronkaApi.Interfaces;
using ErronkaApi.Modeloak;
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
        private readonly IErreserbaRepository _repo;

        public ErreserbakController(IErreserbaRepository repo)
        {
            _repo = repo;
        }

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

            var list = _repo.GetByDate(eguna, mota)
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
            var entity = _repo.Create(dto);
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

            if (!_repo.UpdateByMahai(mahaiaId, eguna, mota, dto)) return NotFound();

            return NoContent();
        }

        [HttpDelete("mahaia/{mahaiaId:int}")]
        public IActionResult DeleteByMahai(int mahaiaId, [FromQuery] string data, [FromQuery] bool mota)
        {
            if (!DateTime.TryParseExact(data, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var eguna))
                return BadRequest("data format: yyyy-MM-dd");

            if (!_repo.DeleteByMahai(mahaiaId, eguna, mota)) return NotFound();

            return NoContent();
        }
    }
}
