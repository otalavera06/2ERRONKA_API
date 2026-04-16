using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
using ErronkaApi.NHibernate;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ErronkaApi.Kontrollerrak
{
    [ApiController]
    [Route("api/platerak")]
    public class PlaterakController : ControllerBase
    {
        private readonly IPlateraRepository _repo;

        public PlaterakController(IPlateraRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var platerak = _repo.GetAll(baseUrl);

                return Ok(new ErantzunaDTO<PlateraDTO>
                {
                    Code = 200,
                    Message = "Platerak lortu dira",
                    Datuak = platerak
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErantzunaDTO<PlateraDTO>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<PlateraDTO>()
                });
            }
        }
    }
}
