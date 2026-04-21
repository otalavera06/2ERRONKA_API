using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NHibernate.Linq;
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

        [HttpGet]
        public IActionResult GetAll()
        {
            using var session = NHibernateHelper.OpenSession();
            var rows = session.CreateSQLQuery(
                    @"SELECT f.id, f.zerbitzua_id, f.prezio_totala, z.data, m.izena,
                             (SELECT GROUP_CONCAT(CONCAT(ag.izena, ' x', ag.kant) SEPARATOR ', ')
                              FROM (SELECT e3.zerbitzua_id, e3.izena, COUNT(*) as kant
                                    FROM eskaerak e3
                                    GROUP BY e3.zerbitzua_id, e3.izena) ag
                              WHERE ag.zerbitzua_id = f.zerbitzua_id) AS eskaera_xehetasunak
                      FROM fakturak f
                      LEFT JOIN zerbitzua z ON f.zerbitzua_id = z.id
                      LEFT JOIN mahaiak m ON z.mahaiak_id = m.id
                      ORDER BY f.id DESC
                      LIMIT 200")
                .List<object[]>();

            var list = rows.Select(r => new
            {
                Id = Convert.ToInt32(r[0]),
                ZerbitzuaId = r[1] == DBNull.Value || r[1] == null ? 0 : Convert.ToInt32(r[1]),
                PrezioTotala = r[2] == DBNull.Value || r[2] == null ? 0 : Convert.ToDecimal(r[2]),
                Data = r[3] == DBNull.Value || r[3] == null ? "" : Convert.ToDateTime(r[3]).ToString("yyyy-MM-dd HH:mm"),
                MahaiaIzena = r[4] == DBNull.Value || r[4] == null ? "Ezezaguna" : r[4].ToString(),
                EskaeraXehetasunak = r[5] == DBNull.Value || r[5] == null ? "" : r[5].ToString(),
                Sortuta = true,
                Path = $"/api/fakturak/{Convert.ToInt32(r[0])}/pdf"
            }).ToList();

            return Ok(list);
        }

        [HttpGet("{fakturaId:int}/pdf")]
        public IActionResult Pdf(int fakturaId)
        {
            using var session = NHibernateHelper.OpenSession();
            var faktura = session.CreateSQLQuery(
                    @"SELECT f.id, f.zerbitzua_id, f.prezio_totala
                      FROM fakturak f
                      WHERE f.id = :id")
                .SetParameter("id", fakturaId)
                .UniqueResult<object[]>();
            if (faktura == null) return NotFound();

            var zerbitzuaId = faktura[1] == DBNull.Value || faktura[1] == null ? 0 : Convert.ToInt32(faktura[1]);
            var zerbitzua = session.CreateSQLQuery(
                    @"SELECT z.id, z.mahaiak_id, z.data, z.prezioTotala
                      FROM zerbitzua z
                      WHERE z.id = :id")
                .SetParameter("id", zerbitzuaId)
                .UniqueResult<object[]>();
            if (zerbitzua == null) return NotFound();

            var produktuak = session.CreateSQLQuery(
                    @"SELECT e.izena, e.prezioa, COUNT(*) AS kantitatea
                      FROM eskaerak e
                      WHERE e.zerbitzua_id = :id
                      GROUP BY e.izena, e.prezioa
                      ORDER BY e.izena")
                .SetParameter("id", zerbitzuaId)
                .List<object[]>();

            using var ms = new MemoryStream();
            float mmToPoints = 2.83465f;
            var pageWidth = 80 * mmToPoints;
            var pageHeight = 1000f;

            var doc = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(pageWidth, pageHeight));
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
            var normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8);

            doc.Add(new iTextSharp.text.Paragraph("Beasain Jatetxea", titleFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingAfter = 3f });
            doc.Add(new iTextSharp.text.Paragraph("NIF: X12345678", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER });
            doc.Add(new iTextSharp.text.Paragraph($"Faktura #: {fakturaId}", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingAfter = 5f });
            var mahaiaId = zerbitzua[1] == DBNull.Value || zerbitzua[1] == null ? 0 : Convert.ToInt32(zerbitzua[1]);
            var data = zerbitzua[2] == DBNull.Value || zerbitzua[2] == null ? DateTime.MinValue : Convert.ToDateTime(zerbitzua[2]);
            doc.Add(new iTextSharp.text.Paragraph($"Mahaia: {mahaiaId}   Data: {data:dd/MM/yyyy HH:mm}", normalFont) { SpacingAfter = 5f });

            decimal total = 0;

            foreach (var p in produktuak)
            {
                string produktuIzena = p[0]?.ToString() ?? "Ezezaguna";
                decimal prezioa = p[1] == DBNull.Value || p[1] == null ? 0 : Convert.ToDecimal(p[1]);
                int kantitatea = p[2] == DBNull.Value || p[2] == null ? 1 : Convert.ToInt32(p[2]);
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
            writer.Close();

            var bytes = ms.ToArray();
            Response.Headers["Content-Disposition"] = $"inline; filename=\"Faktura_{fakturaId}.pdf\"";
            return new FileStreamResult(new MemoryStream(bytes), "application/pdf");
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
                using var session = NHibernateHelper.OpenSession();
                using var tx = session.BeginTransaction();

                var zerbitzua = session.CreateSQLQuery(
                        @"SELECT id, mahaiak_id, prezioTotala
                          FROM zerbitzua
                          WHERE id = :id")
                    .SetParameter("id", eskaeraId)
                    .UniqueResult<object[]>();

                if (zerbitzua == null)
                {
                    return NotFound(new ErantzunaDTO<string>
                    {
                        Code = 404,
                        Message = "Eskaera ez da aurkitu",
                        Datuak = new List<string>()
                    });
                }

                session.CreateSQLQuery("UPDATE zerbitzua SET ordainduta = 1 WHERE id = :id")
                    .SetParameter("id", eskaeraId)
                    .ExecuteUpdate();

                
                session.CreateSQLQuery("UPDATE eskaerak SET egoera = 2 WHERE zerbitzua_id = :id")
                    .SetParameter("id", eskaeraId)
                    .ExecuteUpdate();

                var fakturaExists = session.CreateSQLQuery("SELECT id FROM fakturak WHERE zerbitzua_id = :id LIMIT 1")
                    .SetParameter("id", eskaeraId)
                    .UniqueResult();

                if (fakturaExists == null)
                {
                    session.CreateSQLQuery(
                            @"INSERT INTO fakturak (prezio_totala, zerbitzua_id)
                              VALUES (:prezioTotala, :zerbitzuaId)")
                        .SetParameter("prezioTotala", zerbitzua[2] == DBNull.Value || zerbitzua[2] == null ? 0 : Convert.ToDecimal(zerbitzua[2]))
                        .SetParameter("zerbitzuaId", eskaeraId)
                        .ExecuteUpdate();
                }

                tx.Commit();

                var fakturaId = fakturaExists == null
                    ? Convert.ToInt32(session.CreateSQLQuery("SELECT LAST_INSERT_ID()").UniqueResult())
                    : Convert.ToInt32(fakturaExists);

                return Ok(new ErantzunaDTO<string>
                {
                    Code = 200,
                    Message = "Faktura ongi sortuta",
                    Datuak = new List<string> { $"/api/fakturak/{fakturaId}/pdf" }
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
