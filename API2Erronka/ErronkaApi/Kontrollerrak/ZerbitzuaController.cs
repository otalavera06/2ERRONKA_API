 using ErronkaApi.Modeloak;
 using ErronkaApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ErronkaApi.Kontrollerrak
{
     /// <summary>
     /// Zerbitzuen kudeaketa: eskaerak sortu, mahaika kontsultatu eta ordaindu.
     /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ZerbitzuaController : ControllerBase
    {
         private readonly IZerbitzuaRepository _repo;
 
         public ZerbitzuaController(IZerbitzuaRepository repo)
         {
             _repo = repo;
         }
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

        /// <summary>
        /// Zerbitzu berria sortzen du mahai baten gainean.
        /// </summary>
        /// <param name="dto">Zerbitzua sortzeko datuak.</param>
        /// <returns>201 Created edo 400.</returns>
        [HttpPost]
        public IActionResult Create([FromBody] ZerbitzuaSortuDto dto)
        {
            if (!dto.MahaiakId.HasValue) return BadRequest("MahaiakId is required");
            try
            {
                var id = _repo.Create(dto);
                return CreatedAtAction(nameof(GetByMahai), new { mahaiaId = dto.MahaiakId.Value }, new { Id = id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Mahaiko azken zerbitzuak lortzen ditu.
        /// </summary>
        /// <param name="mahaiaId">Mahaiaren IDa.</param>
        /// <returns>200 OK eta zerbitzuen zerrenda.</returns>
        [HttpGet("mahaia/{mahaiaId:int}")]
        public IActionResult GetByMahai(int mahaiaId)
        {
             var result = _repo.GetByMahai(mahaiaId);
             return Ok(result);
        }

        /// <summary>
        /// Eskaera ordainduta markatzen du.
        /// </summary>
        /// <param name="id">Eskaeraren IDa.</param>
        /// <returns>200 OK edo 404.</returns>
        [HttpPost("{id:int}/ordaindu")]
        public IActionResult Ordaindu(int id)
        {
             var ok = _repo.Ordaindu(id);
             if (!ok) return NotFound();
             return Ok();
        }
    }
}
