using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ErronkaApi.Interfaces;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Fakturak kudeatzeko kontroladorea.
    /// Eskaera baten faktura sortzeko eta ordainketa pendienteak kudeatzeko balio du.
    /// </summary>
    [ApiController]
    [Route("api/fakturak")]
    public class FakturaKontrollerra : ControllerBase
    {
        private readonly IEskaeraRepository _repoEskaera;
        private readonly IMahaiaRepository _repoMahaia;
        private readonly IProduktuaRepository _repoProduktua;
        private readonly IEskaeraProduktuakRepository _repoEskaeraProduktuak;

        public FakturaKontrollerra(
            IEskaeraRepository repoEskaera,
            IMahaiaRepository repoMahaia,
            IProduktuaRepository repoProduktua,
            IEskaeraProduktuakRepository repoEskaeraProduktuak)
        {
            _repoEskaera = repoEskaera;
            _repoMahaia = repoMahaia;
            _repoProduktua = repoProduktua;
            _repoEskaeraProduktuak = repoEskaeraProduktuak;
        }

        public FakturaKontrollerra()
        {
            _repoEskaera = (IEskaeraRepository)new EskaeraRepository(NHibernateHelper.SessionFactory);
            _repoMahaia = new MahaiaRepository(NHibernateHelper.SessionFactory);
            _repoProduktua = new ProduktuaRepository(NHibernateHelper.SessionFactory);
            _repoEskaeraProduktuak = new EskaeraProduktuakRepository(NHibernateHelper.SessionFactory);
        }

        /// <summary>
        /// Eskaera baten faktura sortzen du.
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <returns>Emaitza.</returns>
        [HttpPost("{eskaeraId}/sortu")]
        public IActionResult SortuFaktura(int eskaeraId)
        {
            try
            {
                var eskaera = _repoEskaera.Get(eskaeraId);
                if (eskaera == null)
                {
                    return NotFound(new ErantzunaDTO<string>
                    {
                        Code = 404,
                        Message = "Eskaera ez da aurkitu",
                        Datuak = new List<string>()
                    });
                }

                var produktuak = _repoEskaera.LortuEskaeraProduktuak2(eskaeraId);

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
                    doc.Add(new iTextSharp.text.Paragraph($"TOTALA: {total.ToString("C")}", titleFont) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingBefore = 5f });
                    doc.Add(new iTextSharp.text.Paragraph("Prezioak BEZ barne daude", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingBefore = 2f });

                    doc.Add(new iTextSharp.text.Paragraph(" ", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph("Enpresaren datuak: NIF: X12345678 | PV: 001", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingBefore = 8f });
                    doc.Add(new iTextSharp.text.Paragraph("ESKERRIK ASKO", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingBefore = 8f });

                    doc.Close();
                }

                
                eskaera.egoera = "itxita";
                eskaera.itxieraData = DateTime.Now;
                _repoEskaera.Update(eskaera);

                
                foreach (var em in eskaera.EskaeraMahaiak)
                {
                    var mahaia = em.Mahaia;
                    mahaia.egoera = "libre";
                    _repoMahaia.Update(mahaia);
                }

                return Ok(new ErantzunaDTO<string>
                {
                    Code = 200,
                    Message = $"Faktura ongi sortuta: {filename}",
                    Datuak = new List<string> { filename }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = "Arazoa faktura sortzean: " + ex.Message,
                    Datuak = new List<string>()
                });
            }
        }
    }
}
