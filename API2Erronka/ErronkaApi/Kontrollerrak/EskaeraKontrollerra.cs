using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Eskaera berri bat sortzen du.
        /// </summary>
        /// <param name="dto">Eskaera berriaren datuak.</param>
        /// <returns>Sortutako eskaeraren informazioa.</returns>
        [HttpPost]
        public IActionResult SortuEskaera([FromBody] EskaeraSortuDTO dto)
        {
            if (dto == null) return BadRequest(new ErantzunaDTO<string> { Code = 400, Message = "Datuak behar dira" });

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
                if (produktua == null) continue;
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
                if (produktua == null) continue;
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

        /// <summary>
        /// Eskaera bat ezabatzen du.
        /// </summary>
        /// <param name="eskaeraId">Ezabatu nahi den eskaeraren IDa.</param>
        /// <returns>Erantzunaren emaitza.</returns>
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

        /// <summary>
        /// Mahai baten kapazitatea lortzen du.
        /// </summary>
        /// <param name="mahaiaId">Mahaiaren IDa.</param>
        /// <returns>Mahaiaren kapazitatea.</returns>
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
                    Datuak = new List<int> { mahaia.kapazitatea }
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

        /// <summary>
        /// Eskaera bat eguneratzen du (komensalak eta produktuak).
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <param name="dto">Eguneratu nahi diren datuak.</param>
        /// <returns>Eguneratzearen emaitza.</returns>
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

        /// <summary>
        /// Eskaera baten sukaldeko egoera eguneratzen du.
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <param name="dto">Sukaldeko egoera berria.</param>
        /// <returns>Eguneratzearen emaitza.</returns>
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

                eskaera.sukaldeaEgoera = dto.SukaldeaEgoera.ToLower();
                _repo.Update(eskaera);

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

        /// <summary>
        /// Eskaera bat ordaintzera bidaltzen du.
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <returns>Emaitza.</returns>
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

        /// <summary>
        /// Ordaintzeko dauden eskaerak lortzen ditu.
        /// </summary>
        /// <returns>Ordaintzeko dauden eskaeren zerrenda.</returns>
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
                    Data = e.sortzeData.ToString("yyyy-MM-dd HH:mm"),
                    SukaldeaEgoera = e.sukaldeaEgoera
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
