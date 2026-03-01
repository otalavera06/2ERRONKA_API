using Api.Modeloak;
using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;
using NHibernate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ErronkaApi.Repositorioak
{
    public class EskaeraRepository
    {
        private readonly ISessionFactory _sessionFactory;
        private static readonly HashSet<string> SukaldeaEgoerakOnartuak = new(StringComparer.OrdinalIgnoreCase)
        {
            "zain",
            "hasi",
            "prest"
        };

        public EskaeraRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ErantzunaDTO<string> SortuEskaera(EskaeraSortuDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            try
            {
                
                var mahaia = session.Get<Mahaia>(dto.MahaiaId);

                if (mahaia == null)
                    return new ErantzunaDTO<string>
                    {
                        Code = 400,
                        Message = "Mahaia ez da aurkitu",
                        Datuak = new List<string>()
                    };

                mahaia.egoera = "okupatuta";
                session.Update(mahaia);

                var produktuakStockGabe = new List<string>();

                foreach (var p in dto.Produktuak)
                {
                    var produktua = session.Get<Produktua>(p.ProduktuaId, LockMode.Upgrade);
                    if (produktua.stock_aktuala < p.Kantitatea)
                    {
                        produktuakStockGabe.Add(produktua.izena);
                    }
                }
                if (produktuakStockGabe.Any())
                {
                    return new ErantzunaDTO<string>
                    {
                        Code = 400,
                        Message = "Stock gabe dauden produktuak daude",
                        Datuak = produktuakStockGabe
                    };
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
                    var produktua = session.Get<Produktua>(p.ProduktuaId, LockMode.Upgrade);
                    produktua.stock_aktuala -= p.Kantitatea;
                    session.Update(produktua);

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
                
                session.Save(eskaera);
                tx.Commit();

                IList<object> eskaerak = new List<object> { eskaera };

                return new ErantzunaDTO<string>
                {
                    Code = 200,
                    Message = "Eskaera ongi sortu da",
                    Datuak = new List<string>()
                };
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<string>()
                };
            }
        }

        public ErantzunaDTO<EskaeraDTO> LortuEskaerak()
        {
            using var session = _sessionFactory.OpenSession();
            try
            {
                var eskaerak = session.Query<Eskaera>()
                    .Where(e => e.egoera == "irekita")
                    .OrderByDescending(e => e.sortzeData)
                    .ToList();

                var dtoak = eskaerak.Select(e => new EskaeraDTO
                {
                    Id = e.id,
                    Izena = $"Eskaera #{e.id} ({e.sortzeData:dd/MM/yyyy HH:mm})",
                    MahaiaId = e.mahaia_id,
                    Komensalak = e.komensalak,
                    Data = e.sortzeData.ToString("yyyy-MM-dd HH:mm"),
                    SukaldeaEgoera = string.IsNullOrWhiteSpace(e.sukaldeaEgoera) ? "zain" : e.sukaldeaEgoera
                }).ToList();

                return new ErantzunaDTO<EskaeraDTO>
                {
                    Code = 200,
                    Message = "Eskaerak lortu dira",
                    Datuak = dtoak
                };
            }
            catch (Exception ex)
            {
                return new ErantzunaDTO<EskaeraDTO>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<EskaeraDTO>()
                };
            }
        }

        /// <summary>
        /// Mahai bakoitzeko zerbitzua, bere eskaerekin, itzultzen du. FROGA
        /// </summary>
        /// <remarks>
        /// Mahaiak bilatu hauen idearen arabera, goitik .
        /// </remarks>
        /// <returns>
        /// Zerbitzuen zerrenda DTO formatuan, zerbitzu bakoitzaren eskaera zerrendarekin batera.
        /// Froga
        /// </returns>
        public ErantzunaDTO<EskaeraLortuDTO> LortuEskaeraProduktuak(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            try
            {
                var produktuLista = session.Query<EskaeraProduktuak>()
                    .Where(ep => ep.Eskaera.id == eskaeraId)
                    .ToList();

                var result = new List<EskaeraLortuDTO>();

                // Unitateko ilara bakoitza sortu
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

                return new ErantzunaDTO<EskaeraLortuDTO>
                {
                    Code = 200,
                    Message = "Produktuak lortu dira",
                    Datuak = result
                };
            }
            catch (Exception ex)
            {
                return new ErantzunaDTO<EskaeraLortuDTO>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<EskaeraLortuDTO>()
                };
            }
        }

        public ErantzunaDTO<int> LortuMahaiKapazitatea(int mahaiaId)
        {
            using var session = _sessionFactory.OpenSession();
            try
            {
                var mahaia = session.Get<Mahaia>(mahaiaId);

                if (mahaia == null)
                {
                    return new ErantzunaDTO<int>
                    {
                        Code = 404,
                        Message = "Mahaia ez da aurkitu",
                        Datuak = new List<int>()
                    };
                }

                return new ErantzunaDTO<int>
                {
                    Code = 200,
                    Message = "Mahaia lortu da arrakastaz",
                    Datuak = new List<int> { mahaia.kapazitatea }
                };
            }
            catch (Exception ex)
            {
                return new ErantzunaDTO<int>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<int>()
                };
            }
        }

        public ErantzunaDTO<string> EzabatuEskaera(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            try
            {
                var eskaera = session.Get<Eskaera>(eskaeraId);
                if (eskaera == null)
                {
                    return new ErantzunaDTO<string>
                    {
                        Code = 404,
                        Message = "Eskaera ez da aurkitu",
                        Datuak = new List<string>()
                    };
                }

                // Lortu eskaeraren produktuak zuzenean databasetik eager loading bidez
                var eskaeraProduktuak = session.Query<EskaeraProduktuak>()
                    .Where(ep => ep.Eskaera.id == eskaeraId)
                    .ToList();

                if (eskaera.EskaeraMahaiak.Any())
                {
                    foreach (var em in eskaera.EskaeraMahaiak)
                    {
                        em.Mahaia.egoera = "libre";
                        session.Update(em.Mahaia);
                        session.Delete(em);
                    }
                }

                if (eskaeraProduktuak.Any())
                {
                    foreach (var ep in eskaeraProduktuak)
                    {
                        var produktua = session.Get<Produktua>(ep.Produktua.id, LockMode.Upgrade);
                        if (produktua != null)
                        {
                            produktua.stock_aktuala += ep.Kantitatea;
                            session.Update(produktua);
                        }
                        session.Delete(ep);
                    }
                }

                session.Delete(eskaera);

                tx.Commit();

                return new ErantzunaDTO<string>
                {
                    Code = 200,
                    Message = "Eskaera ezabatu da arrakastaz",
                    Datuak = new List<string>()
                };
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<string>()
                };
            }
        }

        public ErantzunaDTO<string> EguneratuEskaera(int eskaeraId, int komensalak, List<EskaeraProduktuaEditatuDTO> produktuak)
        {

            //1 eskaera lortu 
            
            //2 produktuak foreach 
            //eskaeran badago? => Ez: Gehitu + stocketik kendu 
            // => Bai: kantitatea aldatu da? (honen arabera zerbait egin) 
            
            //3 Eskaerako produktuetan badago baina bialitako produktuetan ez? BESTE FOREACH BAT KASU HONETAN ALDERANTZIZ

            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                // 1. Eskaera lortu
                var eskaera = session.Get<Eskaera>(eskaeraId);
                if (eskaera == null)
                {
                    return new ErantzunaDTO<string>
                    {
                        Code = 404,
                        Message = "Eskaera ez da aurkitu",
                        Datuak = new List<string>()
                    };
                }

                var eskaerakoProduktuak = eskaera.EskaeraProduktuak.ToList();

                if (komensalak > 0)
                {
                    eskaera.komensalak = komensalak;
                }

                // 2. Bidalitako produktuak foreach
                foreach (var dto in produktuak)
                {
                    var produktua = session.Get<Produktua>(dto.ProduktuaId, LockMode.Upgrade);
                    if (produktua == null)
                        throw new Exception($"Produktua ez da existitzen: {dto.ProduktuaId}");

                    var ep = eskaerakoProduktuak
                        .FirstOrDefault(x => x.Produktua.id == dto.ProduktuaId);

                    if (ep == null)
                    {
                        // EZ badago → Gehitu + stocketik kendu
                        if (produktua.stock_aktuala < dto.Kantitatea)
                            throw new Exception($"Stock nahikorik ez: {produktua.izena}");

                        produktua.stock_aktuala -= dto.Kantitatea;

                        var berria = new EskaeraProduktuak
                        {
                            Eskaera = eskaera,
                            Produktua = produktua,
                            Kantitatea = dto.Kantitatea,
                            PrezioUnitarioa = produktua.prezioa,
                            Guztira = produktua.prezioa * dto.Kantitatea
                        };

                        eskaera.EskaeraProduktuak.Add(berria);
                        session.Update(produktua);
                    }
                    else
                    {
                        // BADAGO → kantitatea aldatu da?
                        int diferentzia = dto.Kantitatea - ep.Kantitatea;

                        if (diferentzia != 0)
                        {
                            if (diferentzia > 0 && produktua.stock_aktuala < diferentzia)
                                throw new Exception($"Stock nahikorik ez: {produktua.izena}");

                            produktua.stock_aktuala -= diferentzia;

                            ep.Kantitatea = dto.Kantitatea;
                            ep.Guztira = ep.PrezioUnitarioa * ep.Kantitatea;

                            session.Update(produktua);
                            session.Update(ep);
                        }
                    }
                }

                // 3. Eskaeran badago baina bidalitakoetan ez
                foreach (var ep in eskaerakoProduktuak)
                {
                    bool badagoDTOan = produktuak
                        .Any(p => p.ProduktuaId == ep.Produktua.id);

                    if (!badagoDTOan)
                    {
                        var produktua = session.Get<Produktua>(ep.Produktua.id, LockMode.Upgrade);
                        produktua.stock_aktuala += ep.Kantitatea;

                        eskaera.EskaeraProduktuak.Remove(ep);

                        session.Update(produktua);
                        session.Delete(ep);
                    }
                }

                session.Update(eskaera);
                tx.Commit();

                return new ErantzunaDTO<string>
                {
                    Code = 200,
                    Message = "Eskaera eguneratu da arrakastaz",
                    Datuak = new List<string>()
                };
            }
            catch (Exception ex)
            {
                try { tx.Rollback(); } catch { }

                return new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = ex.Message,
                    Datuak = new List<string>()
                };
            }
        }

        public ErantzunaDTO<string> EguneratuSukaldeaEgoera(int eskaeraId, string sukaldeaEgoera)
        {
            if (string.IsNullOrWhiteSpace(sukaldeaEgoera))
            {
                return new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = "Sukaldea egoera derrigorrezkoa da",
                    Datuak = new List<string>()
                };
            }

            if (!SukaldeaEgoerakOnartuak.Contains(sukaldeaEgoera))
            {
                return new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = "Sukaldea egoera ez da baliozkoa (zain, hasi, prest)",
                    Datuak = new List<string>()
                };
            }

            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var eskaera = session.Get<Eskaera>(eskaeraId);
                if (eskaera == null)
                {
                    return new ErantzunaDTO<string>
                    {
                        Code = 404,
                        Message = "Eskaera ez da aurkitu",
                        Datuak = new List<string>()
                    };
                }

                eskaera.sukaldeaEgoera = sukaldeaEgoera.ToLowerInvariant();
                session.Update(eskaera);
                tx.Commit();

                return new ErantzunaDTO<string>
                {
                    Code = 200,
                    Message = "Sukaldea egoera eguneratu da",
                    Datuak = new List<string>()
                };
            }
            catch (Exception ex)
            {
                try { tx.Rollback(); } catch { }

                return new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = ex.Message,
                    Datuak = new List<string>()
                };
            }
        }
        public ErantzunaDTO<string> OrdaintzeraBidali(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var eskaera = session.Get<Eskaera>(eskaeraId);

                if (eskaera == null)
                {
                    return new ErantzunaDTO<string>
                    {
                        Code = 404,
                        Message = "Eskaera ez da aurkitu",
                        Datuak = new List<string>()
                    };
                }

                eskaera.egoera = "ordainketa_pendiente";

                session.Update(eskaera);
                tx.Commit();

                return new ErantzunaDTO<string>
                {
                    Code = 200,
                    Message = "Eskaera ordainketara bidali da",
                    Datuak = new List<string>()
                };
            }
            catch (Exception ex)
            {
                try { tx.Rollback(); } catch { }

                return new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<string>()
                };
            }
        }

        public ErantzunaDTO<string> SortuFaktura(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            try
            {
                var mahaiak = session.Query<Mahaia>()
                    .Where(m => m.EskaeraMahaiak.Any(em => em.Eskaera.id == eskaeraId))
                    .ToList();
                var eskaera = session.Get<Eskaera>(eskaeraId);

                if (eskaera == null)
                    return new ErantzunaDTO<string>
                    {
                        Code = 404,
                        Message = "Eskaera ez da aurkitu",
                        Datuak = new List<string>()
                    };

                var produktuak = session.Query<EskaeraProduktuak>()
                    .Where(p => p.Eskaera.id == eskaeraId)
                    .ToList();

                string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string carpetaFakturak = Path.Combine(escritorio, "fakturak");

                if (!Directory.Exists(carpetaFakturak))
                    Directory.CreateDirectory(carpetaFakturak);

                string filename = Path.Combine(carpetaFakturak, $"Faktura_Eskaera_{eskaeraId}.pdf");

                using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    float mmToPoints = 2.83465f;
                    var pageWidth = 80 * mmToPoints;
                    var pageHeight = 1000f;

                    var doc = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(pageWidth, pageHeight));
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
                    var normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8);

                    doc.Add(new iTextSharp.text.Paragraph("Beasain Jatetxea", titleFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingAfter = 3f });
                    doc.Add(new iTextSharp.text.Paragraph("NIF: X12345678", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph($"Faktura #: {eskaeraId}", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingAfter = 5f });
                    doc.Add(new iTextSharp.text.Paragraph($"Mahaia: {eskaera.mahaia_id}   Data: {eskaera.sortzeData:dd/MM/yyyy HH:mm}", normalFont) { SpacingAfter = 5f });

                    decimal total = 0;

                    foreach (var p in produktuak)
                    {
                        string produktuIzena = p.Produktua?.izena ?? "Ezezaguna";
                        decimal prezioa = p.PrezioUnitarioa;
                        int kantitatea = p.Kantitatea;
                        decimal lineaTotala = prezioa * kantitatea;
                        total += lineaTotala;

                        var namePara = new iTextSharp.text.Paragraph(produktuIzena, normalFont) { SpacingAfter = 1f };
                        doc.Add(namePara);

                        var detailPara = new iTextSharp.text.Paragraph($"{kantitatea} x {prezioa.ToString("C")}    {lineaTotala.ToString("C")}", normalFont) { SpacingAfter = 3f };
                        doc.Add(detailPara);
                    }

                    doc.Add(new iTextSharp.text.Paragraph(" ", normalFont));

                    // Erakutsi bakarrik TOTALA (prezioak BEZ barne daude)
                    doc.Add(new iTextSharp.text.Paragraph($"TOTALA: {total.ToString("C")}", titleFont) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingBefore = 5f });
                    doc.Add(new iTextSharp.text.Paragraph("Prezioak BEZ barne daude", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingBefore = 2f });

                    doc.Add(new iTextSharp.text.Paragraph(" ", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph("Enpresaren datuak: NIF: X12345678 | PV: 001", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingBefore = 8f });
                    doc.Add(new iTextSharp.text.Paragraph("ESKERRIK ASKO", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingBefore = 8f });

                    doc.Close();
                }

                eskaera.egoera = "itxita";
                eskaera.itxieraData = DateTime.Now;
                session.Update(eskaera);
                mahaiak.ForEach(m =>
                {
                    m.egoera = "libre";
                    session.Update(m);
                });
                tx.Commit();

                return new ErantzunaDTO<string>
                {
                    Code = 200,
                    Message = $"Faktura ongi sortuta: {filename}",
                    Datuak = new List<string> { filename }
                };
            }
            catch (Exception ex)
            {
                try { tx.Rollback(); } catch { }

                return new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = "Arazoa faktura sortzean: " + ex.Message,
                    Datuak = new List<string>()
                };
            }
        }


        public ErantzunaDTO<EskaeraDTO> LortuEskaerakOrdaintzeko()
        {
            using var session = _sessionFactory.OpenSession();
            try
            {
                var eskaerak = session.Query<Eskaera>()
                    .Where(e => e.egoera == "ordainketa_pendiente")
                    .OrderByDescending(e => e.sortzeData)
                    .ToList();

                var dtoak = eskaerak.Select(e => new EskaeraDTO
                {
                    Id = e.id,
                    Izena = $"Eskaera #{e.id} ({e.sortzeData:dd/MM/yyyy HH:mm})",
                    MahaiaId = e.mahaia_id,
                    Data = e.sortzeData.ToString("yyyy-MM-dd HH:mm"),
                    SukaldeaEgoera = e.sukaldeaEgoera
                }).ToList();

                return new ErantzunaDTO<EskaeraDTO>
                {
                    Code = 200,
                    Message = "Eskaerak lortu dira",
                    Datuak = dtoak
                };
            }
            catch (Exception ex)
            {
                return new ErantzunaDTO<EskaeraDTO>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = new List<EskaeraDTO>()
                };
            }
        }
    }
}
