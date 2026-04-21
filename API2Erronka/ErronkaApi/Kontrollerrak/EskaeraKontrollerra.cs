using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace ErronkaApi.Kontrollerrak
{
    [ApiController]
    [Route("api/eskaerak")]
    public class EskaeraKontrollerra : ControllerBase
    {
        private readonly IEskaeraRepository _repo;
        private readonly IMahaiaRepository _repoMahaia;
        private readonly IProduktuaRepository _repoProduktua;
        private readonly IEskaeraProduktuakRepository _repoEskaeraProduktuak;
        private readonly IEskaeraMahaiakRepository _repoEskaeraMahaiak;

        public EskaeraKontrollerra(
            IEskaeraRepository repo,
            IMahaiaRepository repoMahaia,
            IProduktuaRepository repoProduktua,
            IEskaeraProduktuakRepository repoEskaeraProduktuak,
            IEskaeraMahaiakRepository repoEskaeraMahaiak)
        {
            _repo = repo;
            _repoMahaia = repoMahaia;
            _repoProduktua = repoProduktua;
            _repoEskaeraProduktuak = repoEskaeraProduktuak;
            _repoEskaeraMahaiak = repoEskaeraMahaiak;
        }

        [HttpPost]
        public IActionResult SortuEskaera([FromBody] EskaeraSortuDTO dto)
        {
            if (dto == null || dto.Produktuak == null || !dto.Produktuak.Any())
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = "Datuak behar dira",
                    Datuak = new List<string>()
                });
            }

            try
            {
                var mahaia = _repoMahaia.Get(dto.MahaiaId);
                if (mahaia == null)
                {
                    return BadRequest(new ErantzunaDTO<string>
                    {
                        Code = 400,
                        Message = "Mahaia ez da aurkitu",
                        Datuak = new List<string>()
                    });
                }

                var erroreak = new List<string>();
                foreach (var p in dto.Produktuak)
                {
                    var produktua = _repoProduktua.Get(p.ProduktuaId);
                    if (produktua == null)
                    {
                        erroreak.Add($"Produktua ez da existitzen: {p.ProduktuaId}");
                    }
                    else if (produktua.stock_aktuala < p.Kantitatea)
                    {
                        erroreak.Add($"Stock nahikorik ez: {produktua.izena}");
                    }
                }

                if (erroreak.Any())
                {
                    return BadRequest(new ErantzunaDTO<string>
                    {
                        Code = 400,
                        Message = "Erroreak daude",
                        Datuak = erroreak
                    });
                }

                var eskaera = _repo.SortuEskaera(dto);

                return Ok(new ErantzunaDTO<Eskaera>
                {
                    Code = 200,
                    Message = "Eskaera ongi sortu da",
                    Datuak = new List<Eskaera> { eskaera }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<string>()
                });
            }
        }

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
                    SukaldeaEgoera = string.IsNullOrWhiteSpace(e.sukaldeaEgoera) ? "zain" : (string)e.sukaldeaEgoera
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
                    Message = "Errore bat egon da: " + ex.Message + " " + ex.StackTrace,
                    Datuak = new List<EskaeraDTO>()
                });
            }
        }

        [HttpGet("{eskaeraId}/produktuak")]
        public IActionResult LortuEskaeraProduktuak(int eskaeraId)
        {
            try
            {
                var produktuLista = _repo.LortuEskaeraProduktuak2(eskaeraId);

                var result = new List<EskaeraLortuDTO>();

                foreach (var ep in produktuLista)
                {
                    for (int i = 0; i < ep.Kantitatea; i++)
                    {
                        result.Add(new EskaeraLortuDTO
                        {
                            ProduktuaId = ep.Produktua.id,
                            ProduktuaIzena = ep.Produktua.izena,
                            PrezioUnitarioa = ep.Produktua.prezioa,
                            Kantitatea = 1
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

                var eskaeraProduktuak = _repoEskaeraProduktuak.GetByEskaeraId(eskaeraId);

                if (eskaera.EskaeraMahaiak.Any())
                {
                    foreach (var em in eskaera.EskaeraMahaiak)
                    {
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

                return Ok(new ErantzunaDTO<Eskaera>
                {
                    Code = 200,
                    Message = "Eskaera ongi ezabatu da",
                    Datuak = new List<Eskaera> { eskaera },
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<string>()
                });
            }
        }

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
                    Datuak = new List<int> { 4 }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<string>()
                });
            }
        }

        [HttpPut("{eskaeraId}")]
        public IActionResult EguneratuEskaera(int eskaeraId, [FromBody] EskaeraEguneratuDTO dto)
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

            try
            {
                var eskaera = _repo.Get(eskaeraId);
                if (eskaera == null)
                {
                    return NotFound(new ErantzunaDTO<string>
                    {
                        Code = 404,
                        Message = "Eskaera ez da aurkitu",
                        Datuak = new List<string>()
                    });
                }

                var eskaerakoProduktuak = _repo.LortuEskaeraProduktuak2(eskaeraId);

                if (dto.Komensalak > 0)
                {
                    eskaera.komensalak = dto.Komensalak;
                }

                foreach (var pDto in dto.Produktuak)
                {
                    var produktua = _repoProduktua.Get(pDto.ProduktuaId);
                    if (produktua == null)
                        return BadRequest(new ErantzunaDTO<string> { Code = 400, Message = $"Produktua ez da existitzen: {pDto.ProduktuaId}" });

                    var ep = eskaerakoProduktuak.FirstOrDefault(x => x.Produktua.id == pDto.ProduktuaId);

                    if (ep == null)
                    {
                        if (produktua.stock_aktuala < pDto.Kantitatea)
                            return BadRequest(new ErantzunaDTO<string> { Code = 400, Message = $"Stock nahikorik ez: {produktua.izena}" });

                        produktua.stock_aktuala -= pDto.Kantitatea;

                        var berria = new EskaeraProduktuak
                        {
                            Eskaera = eskaera,
                            Produktua = produktua,
                            Kantitatea = pDto.Kantitatea,
                            PrezioUnitarioa = produktua.prezioa,
                            Guztira = produktua.prezioa * pDto.Kantitatea
                        };

                        eskaera.EskaeraProduktuak.Add(berria);
                        _repoProduktua.Update(produktua);
                    }
                    else
                    {
                        int diferentzia = pDto.Kantitatea - ep.Kantitatea;

                        if (diferentzia != 0)
                        {
                            if (diferentzia > 0 && produktua.stock_aktuala < diferentzia)
                                return BadRequest(new ErantzunaDTO<string> { Code = 400, Message = $"Stock nahikorik ez: {produktua.izena}" });

                            produktua.stock_aktuala -= diferentzia;

                            ep.Kantitatea = pDto.Kantitatea;
                            ep.Guztira = ep.PrezioUnitarioa * ep.Kantitatea;

                            _repoProduktua.Update(produktua);
                            _repoEskaeraProduktuak.Update(ep);
                        }
                    }
                }

                foreach (var ep in eskaerakoProduktuak)
                {
                    bool badagoDTOan = dto.Produktuak.Any(p => p.ProduktuaId == ep.Produktua.id);

                    if (!badagoDTOan)
                    {
                        var produktua = _repoProduktua.Get(ep.Produktua.id);
                        if (produktua != null)
                        {
                            produktua.stock_aktuala += ep.Kantitatea;
                            _repoProduktua.Update(produktua);
                        }

                        eskaera.EskaeraProduktuak.Remove(ep);
                        _repoEskaeraProduktuak.Delete(ep);
                    }
                }

                _repo.Update(eskaera);

                return Ok(new ErantzunaDTO<string>
                {
                    Code = 200,
                    Message = "Eskaera eguneratu da arrakastaz",
                    Datuak = new List<string>()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<string>()
                });
            }
        }

        [HttpGet("sukaldea")]
        public IActionResult LortuSukaldekoEskaerak()
        {
            try
            {
                var produktuak = _repo.LortuSukaldekoEskaerak();

                var grouped = produktuak.GroupBy(ep => ep.Eskaera.id).Select(g => new EskaeraDTO
                {
                    Id = g.Key,
                    Izena = $"Eskaera #{g.Key} ({g.First().Eskaera.sortzeData:HH:mm})",
                    MahaiaId = g.First().Eskaera.mahaia_id,
                    Komensalak = g.First().Eskaera.komensalak,
                    Data = g.First().Eskaera.sortzeData.ToString("yyyy-MM-dd HH:mm"),
                    SukaldeaEgoera = "zain",
                    Produktuak = g.Select(ep => new EskaeraLortuDTO
                    {
                        ProduktuaId = ep.Produktua.id,
                    ProduktuaIzena = string.IsNullOrWhiteSpace(ep.Izena) ? ep.Produktua.izena : ep.Izena,
                    PrezioUnitarioa = ep.PrezioUnitarioa,
                        Kantitatea = ep.Kantitatea
                    }).ToList()
                }).ToList();

                return Ok(new ErantzunaDTO<EskaeraDTO>
                {
                    Code = 200,
                    Message = "Sukaldeko eskaerak lortu dira",
                    Datuak = grouped
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

        [HttpPut("{eskaeraId}/sukaldea-egoera")]
        public IActionResult EguneratuSukaldeaEgoera(int eskaeraId, [FromBody] EskaeraSukaldeaEgoeraDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.SukaldeaEgoera))
            {
                return BadRequest(new ErantzunaDTO<string> { Code = 400, Message = "Datuak behar dira" });
            }

            string[] onartuak = { "zain", "hasi", "prest" };
            if (!onartuak.Contains(dto.SukaldeaEgoera.ToLower()))
            {
                return BadRequest(new ErantzunaDTO<string> { Code = 400, Message = "Sukaldea egoera ez da baliozkoa (zain, hasi, prest)" });
            }

            try
            {
                var eskaera = _repo.Get(eskaeraId);
                if (eskaera == null)
                {
                    return NotFound(new ErantzunaDTO<string> { Code = 404, Message = "Eskaera ez da aurkitu" });
                }

                _repo.EguneratuSukaldeaEgoera(eskaeraId, dto.SukaldeaEgoera);

                return Ok(new ErantzunaDTO<string>
                {
                    Code = 200,
                    Message = "Sukaldea egoera eguneratu da",
                    Datuak = new List<string>()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErantzunaDTO<string> { Code = 500, Message = "Errore bat egon da: " + ex.Message });
            }
        }

        [HttpPost("{eskaeraId}/ordaindu")]
        public IActionResult OrdainduEskaera(int eskaeraId)
        {
            try
            {
                var eskaera = _repo.Get(eskaeraId);
                if (eskaera == null)
                {
                    return NotFound(new ErantzunaDTO<string> { Code = 404, Message = "Eskaera ez da aurkitu" });
                }

                eskaera.egoera = "ordainketa_pendiente";
                _repo.Update(eskaera);

                return Ok(new ErantzunaDTO<string>
                {
                    Code = 200,
                    Message = "Eskaera ordainketara bidali da",
                    Datuak = new List<string>()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErantzunaDTO<string> { Code = 500, Message = "Errore bat egon da: " + ex.Message });
            }
        }

        [HttpGet("ordainketa-pendiente")]
        public IActionResult LortuEskaerakOrdaintzeko()
        {
            try
            {
                var eskaerak = _repo.LortuEskaerakOrdaintzeko();

                var dtoak = eskaerak.Select(e => new EskaeraDTO
                {
                    Id = e.id,
                    Izena = $"Eskaera #{e.id} ({e.sortzeData:dd/MM/yyyy HH:mm})",
                    MahaiaId = e.mahaia_id,
                    Komensalak = e.komensalak,
                    Data = e.sortzeData.ToString("yyyy-MM-dd HH:mm"),
                    SukaldeaEgoera = string.IsNullOrWhiteSpace(e.sukaldeaEgoera) ? "zain" : (string)e.sukaldeaEgoera
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
    }
}
