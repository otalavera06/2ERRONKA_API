using Api.Modeloak;
using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using NHibernate;
using System.Linq;
using System.Linq.Expressions;

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
        private readonly MahaiaRepository _repoMahaia;
        private readonly ProduktuaRepository _repoProduktua;
        private readonly EskaeraProduktuakRepository _repoEskaeraProduktuak;
        private readonly EskaeraMahaiakRepository _repoEskaeraMahaiak;

        public EskaeraKontrollerra()
        {
            _repo = new EskaeraRepository(NHibernateHelper.SessionFactory);
            _repoMahaia = new MahaiaRepository(NHibernateHelper.SessionFactory);
            _repoProduktua = new ProduktuaRepository(NHibernateHelper.SessionFactory);
            _repoEskaeraProduktuak = new EskaeraProduktuakRepository(NHibernateHelper.SessionFactory);
            _repoEskaeraMahaiak = new EskaeraMahaiakRepository(NHibernateHelper.SessionFactory);
        }

        /// <summary>
        /// Eskaera berri bat sortzen du.
        /// </summary>
        /// <param name="dto">Eskaera berriaren datuak.</param>
        /// <returns>Sortutako eskaeraren informazioa.</returns>
        //[HttpPost]
        //public IActionResult SortuEskaera([FromBody] EskaeraSortuDTO dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var erantzuna = _repo.SortuEskaera(dto);

        //    if (erantzuna.Code == 200)
        //    {
        //        return Ok(erantzuna);

        //    }
        //    else {
        //        return BadRequest(erantzuna);
        //    }
        //}

        [HttpPost]
        public IActionResult SortuEskaera([FromBody] EskaeraSortuDTO dto)
        {

            Mahaia mahaia = _repoMahaia.Get(dto.MahaiaId);

            if (mahaia == null)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = "Mahaia ez da aurkitu",
                    Datuak = new List<string>()
                });
            }

            mahaia.egoera = "okupatuta";
            _repoMahaia.Update(mahaia);

            var produktuakStockGabe = new List<string>();

            foreach (var p in dto.Produktuak)
            {
                Produktua produktua = _repoProduktua.Get(p.ProduktuaId);
                if (produktua.stock_aktuala < p.Kantitatea)
                {
                    produktuakStockGabe.Add(produktua.izena);
                }
            }
            if (produktuakStockGabe.Any())
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = "Stock gabe dauden produktuak daude",
                    Datuak = produktuakStockGabe
                });
            }

            var eskaera = new Eskaera
            {
                erabiltzaileId = dto.ErabiltzaileId,
                komensalak = dto.Komensalak,
                egoera = "irekita",
                sukaldeaEgoera = "zain",
                sortzeData = DateTime.Now,
                mahaia_id = dto.MahaiaId
            };

            var eskaeraMahaiak = new EskaeraMahaiak
            {
                Eskaera = eskaera,
                Mahaia = mahaia
            };

            eskaera.EskaeraMahaiak.Add(eskaeraMahaiak);
            mahaia.EskaeraMahaiak.Add(eskaeraMahaiak);

            foreach (var p in dto.Produktuak)
            {
                var produktua = _repoProduktua.Get(p.ProduktuaId);
                produktua.stock_aktuala -= p.Kantitatea;
                _repoProduktua.Update(produktua);

                var ep = new EskaeraProduktuak
                {
                    Eskaera = eskaera,
                    Produktua = produktua,
                    Kantitatea = p.Kantitatea,
                    PrezioUnitarioa = produktua.prezioa,
                    Guztira = produktua.prezioa * p.Kantitatea
                };
                eskaera.EskaeraProduktuak.Add(ep);
            }

            _repo.Save(eskaera);

            IList<object> eskaerak = new List<object> { eskaera };

            return Ok(new ErantzunaDTO<Eskaera>
            {
                Code = 200,
                Message = "Eskaera ongi sortu da",
                Datuak = new List<Eskaera> { eskaera },
            });
        }

        /// <summary>
        /// Eskaera guztiak lortzen ditu.
        /// </summary>
        /// <returns>Eskaeren zerrenda.</returns>
        //[HttpGet]
        //public IActionResult LortuEskaerak()
        //{
        //    var erantzuna = _repo.LortuEskaerak();

        //    return StatusCode(erantzuna.Code, erantzuna);
        //}

        [HttpGet]
        public IActionResult LortuEskaerak()
        {
            try
            {
                var eskaerak = _repo.LortuEskaerak2();

                var dtoak = eskaerak.Select(e => new EskaeraDTO
                {
                    Id = e.id,
                    Izena = $"Eskaera #{e.id} ({e.sortzeData:dd/MM/yyyy HH:mm})",
                    MahaiaId = e.mahaia_id,
                    Komensalak = e.komensalak,
                    Data = e.sortzeData.ToString("yyyy-MM-dd HH:mm"),
                    SukaldeaEgoera = string.IsNullOrWhiteSpace(e.sukaldeaEgoera) ? "zain" : e.sukaldeaEgoera
                }).ToList();

                return Ok(new ErantzunaDTO<EskaeraDTO>
                {
                    Code = 200,
                    Message = "Eskaerak lortu dira",
                    Datuak = dtoak
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErantzunaDTO<EskaeraDTO>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<EskaeraDTO>()
                });
            }
        }

    /// <summary>
    /// Eskaera baten produktuak lortzen ditu.
    /// </summary>
    /// <param name="eskaeraId">Eskaeraren IDa.</param>
    /// <returns>Eskaeraren produktuen zerrenda.</returns>
    //[HttpGet("{eskaeraId}/produktuak")]
    //public IActionResult LortuEskaeraProduktuak(int eskaeraId)
    //{
    //    var erantzuna = _repo.LortuEskaeraProduktuak(eskaeraId);

    //    return StatusCode(erantzuna.Code, erantzuna);
    //}

    [HttpGet("{eskaeraId}/produktuak")]
    public IActionResult LortuEskaeraProduktuak(int eskaeraId)
    {
        try
            {
                var produktuLista = _repo.LortuEskaeraProduktuak2(eskaeraId);

                var result = new List<EskaeraLortuDTO>();

                foreach (var ep in produktuLista)
                {
                    for (int i = 0; i < ep.Kantitatea; i++)  // Kantitate bakoitzeko ilara
                    {
                        result.Add(new EskaeraLortuDTO
                        {
                            ProduktuaId = ep.Produktua.id,
                            ProduktuaIzena = ep.Produktua.izena,
                            PrezioUnitarioa = ep.Produktua.prezioa,
                            Kantitatea = 1  // TPV logikari egokitua
                        });
                    }
                }

                return Ok(new ErantzunaDTO<EskaeraLortuDTO>
                {
                    Code = 200,
                    Message = "Produktuak lortu dira",
                    Datuak = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErantzunaDTO<EskaeraLortuDTO>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<EskaeraLortuDTO>()
                });
            }
        }

    /// <summary>
    /// Eskaera bat ezabatzen du.
    /// </summary>
    /// <param name="eskaeraId">Ezabatu nahi den eskaeraren IDa.</param>
    /// <returns>Erantzunaren emaitza.</returns>
    //[HttpDelete("{eskaeraId}")]
    //public IActionResult EzabatuEskaera(int eskaeraId)
    //{
    //    var erantzuna = _repo.EzabatuEskaera(eskaeraId);

    //    if (erantzuna.Code == 200)
    //    {
    //        return Ok(erantzuna);
    //    }
    //    else
    //    {
    //        return BadRequest(erantzuna);
    //    }
    //}


    [HttpDelete("{eskaeraId}")]
    public IActionResult EzabatuEskaera(int eskaeraId)
    {
        try
        {
            Eskaera eskaera = _repo.Get(eskaeraId);

            if (eskaera == null)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = "Eskaera ez da aurkitu",
                    Datuak = new List<string>()
                });
            }

            // Lortu eskaeraren produktuak zuzenean databasetik eager loading bidez
            var eskaeraProduktuak = _repoEskaeraProduktuak.GetByEskaeraId(eskaeraId);

            if (eskaera.EskaeraMahaiak.Any())
            {
                foreach (var em in eskaera.EskaeraMahaiak)
                {
                    em.Mahaia.egoera = "libre";
                    _repoMahaia.Update(em.Mahaia);
                    _repoEskaeraMahaiak.Delete(em);
                }
            }

            if (eskaeraProduktuak.Any())
            {
                foreach (var ep in eskaeraProduktuak)
                {
                    var produktua = _repoProduktua.Get(ep.Produktua.id);
                    if (produktua != null)
                    {
                        produktua.stock_aktuala += ep.Kantitatea;
                        _repoProduktua.Update(produktua);
                    }
                    _repoEskaeraProduktuak.Delete(ep);
                }
            }

            _repo.Delete(eskaera);

                //tx.Commit();

            IList<object> eskaerak = new List<object> { eskaera };

            return Ok(new ErantzunaDTO<Eskaera>
            {
                Code = 200,
                Message = "Eskaera ongi ezabatu da",
                Datuak = new List<Eskaera> { eskaera },
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErantzunaDTO<EskaeraLortuDTO>
            {
                Code = 500,
                Message = "Errore bat egon da: " + ex.Message,
                Datuak = new List<EskaeraLortuDTO>()
            });
        }
    }

    /// <summary>
    /// Mahai baten kapazitatea lortzen du.
    /// </summary>
    /// <param name="mahaiaId">Mahaiaren IDa.</param>
    /// <returns>Mahaiaren kapazitatea.</returns>
    //[HttpGet("mahaiak/{mahaiaId}/kapazitatea")]
    //public IActionResult LortuMahaiKapasitatea(int mahaiaId)
    //{
    //    var erantzuna = _repo.LortuMahaiKapazitatea(mahaiaId);

    //    if (erantzuna.Code == 200)
    //        return Ok(erantzuna);
    //    else if (erantzuna.Code == 404)
    //        return NotFound(erantzuna);
    //    else
    //        return StatusCode(500, erantzuna);
    //}

    [HttpGet("mahaiak/{mahaiaId}/kapazitatea")]
    public IActionResult LortuMahaiKapasitatea(int mahaiaId)
    {
            try
            {
                var mahaia = _repoMahaia.Get(mahaiaId);

                if (mahaia == null)
                {
                    return BadRequest(new ErantzunaDTO<string>
                    {
                        Code = 400,
                        Message = "Mahaia ez da aurkitu",
                        Datuak = new List<string>()
                    });
                }
                return Ok(new ErantzunaDTO<int>
                {
                    Code = 200,
                    Message = "Mahaia lortu da arrakastaz",
                    Datuak = new List<int> { mahaia.kapazitatea } //TO-DO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErantzunaDTO<EskaeraLortuDTO>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<EskaeraLortuDTO>()
                });
            }
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

    [HttpPut("{eskaeraId}")]
    public IActionResult EguneratuEskaera2(
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
