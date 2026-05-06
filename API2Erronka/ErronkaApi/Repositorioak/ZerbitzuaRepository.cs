using System;
using System.Collections.Generic;
using System.Linq;
using ErronkaApi.Interfaces;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;

namespace ErronkaApi.Repositorioak
{
    /// <summary>
    /// Zerbitzuen datu-sarbidea kudeatzen du, eskema berriko `zerbitzua` eta `eskaerak`
    /// tauletara egokituta, lehengo API kontratua mantenduz.
    /// </summary>
    public class ZerbitzuaRepository : IZerbitzuaRepository
    {
        private static string? BuildIrudiUrl(string? irudia)
        {
            if (string.IsNullOrWhiteSpace(irudia)) return null;
            if (irudia.StartsWith("/")) return irudia;
            return "/irudiak/" + irudia;
        }

        private static bool IsPlaceholderPlatera(object? produktuaId, object? produktuaIzena)
        {
            return produktuaId != null
                && produktuaId != DBNull.Value
                && Convert.ToInt32(produktuaId) == 1
                && string.Equals(produktuaIzena?.ToString(), "Platera", StringComparison.OrdinalIgnoreCase);
        }

        private Produktua EnsurePlateraProduktua(global::NHibernate.ISession session)
        {
            var produktua = session.Query<Produktua>().FirstOrDefault(p => p.id == 1);
            if (produktua != null) return produktua;

            session.CreateSQLQuery("INSERT IGNORE INTO produktuak (id, izena, prezioa, stock, irudia, produktuen_motak_id) VALUES (1, 'Platera', 0, 999, 'platera.png', 8)")
                .ExecuteUpdate();

            produktua = session.Query<Produktua>().FirstOrDefault(p => p.id == 1);
            if (produktua == null) throw new InvalidOperationException("Platerak gordetzeko produktu teknikoa ez da existitzen.");
            return produktua;
        }

        public int Create(ZerbitzuaController.ZerbitzuaSortuDto dto)
        {
            using var session = NHibernateHelper.OpenSession();
            using var tx = session.BeginTransaction();
            var zerbitzuData = dto.Data == default ? DateTime.Now : dto.Data;

            var existingIdObj = session.CreateSQLQuery(
                    "SELECT id FROM zerbitzua WHERE mahaiak_id = :mahaiakId AND ordainduta = 0 ORDER BY data DESC LIMIT 1")
                .SetParameter("mahaiakId", dto.MahaiakId!.Value)
                .UniqueResult();

            int zerbitzuaId;

            if (existingIdObj != null)
            {
                zerbitzuaId = Convert.ToInt32(existingIdObj);
                session.CreateSQLQuery(
                        "UPDATE zerbitzua SET prezioTotala = prezioTotala + :prezioTotala WHERE id = :id")
                    .SetParameter("prezioTotala", dto.PrezioTotala)
                    .SetParameter("id", zerbitzuaId)
                    .ExecuteUpdate();
            }
            else
            {
                session.CreateSQLQuery(
                        @"INSERT INTO zerbitzua (prezioTotala, data, ordainduta, erreserba_id, mahaiak_id)
                          VALUES (:prezioTotala, :data, :ordainduta, :erreserbaId, :mahaiakId)")
                    .SetParameter("prezioTotala", dto.PrezioTotala)
                    .SetParameter("data", zerbitzuData)
                    .SetParameter("ordainduta", 0)
                    .SetParameter("erreserbaId", dto.ErreserbaId.HasValue ? dto.ErreserbaId.Value : (int?)null, global::NHibernate.NHibernateUtil.Int32)
                    .SetParameter("mahaiakId", dto.MahaiakId!.Value)
                    .ExecuteUpdate();

                zerbitzuaId = Convert.ToInt32(session.CreateSQLQuery("SELECT LAST_INSERT_ID()").UniqueResult());
            }

            foreach (var e in dto.Eskaerak)
            {
                int dbProduktuaId = e.IsPlatera ? 1 : e.ProduktuaId;
                var produktua = e.IsPlatera
                    ? EnsurePlateraProduktua(session)
                    : session.Query<Produktua>().FirstOrDefault(p => p.id == dbProduktuaId);

                if (produktua == null) throw new InvalidOperationException($"Produktua ez da existitzen: {dbProduktuaId}");

                var finalIzena = string.IsNullOrWhiteSpace(e.Izena) ? produktua.izena : e.Izena;
                var finalPrezioa = e.Prezioa == 0m ? (decimal)produktua.prezioa : e.Prezioa;

                if (!e.IsPlatera)
                {
                    var stockUpdate = session.CreateSQLQuery(
                            @"UPDATE produktuak
                              SET stock = stock - 1
                              WHERE id = :id AND stock >= 1")
                        .SetParameter("id", dbProduktuaId)
                        .ExecuteUpdate();

                    if (stockUpdate == 0)
                    {
                        throw new InvalidOperationException($"Stock nahikorik ez: {produktua.izena}");
                    }
                }

                try
                {
                    session.CreateSQLQuery(
                            @"INSERT INTO eskaerak (izena, prezioa, data, egoera, zerbitzua_id, produktua_id)
                              VALUES (:izena, :prezioa, :data, :egoera, :zerbitzuaId, :produktuaId)")
                        .SetParameter("izena", finalIzena)
                        .SetParameter("prezioa", finalPrezioa)
                        .SetParameter("data", e.Data == default ? zerbitzuData : e.Data)
                        .SetParameter("egoera", e.Egoera)
                        .SetParameter("zerbitzuaId", zerbitzuaId)
                        .SetParameter("produktuaId", dbProduktuaId)
                        .ExecuteUpdate();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error inserting eskaerak (izena={finalIzena}, prezioa={finalPrezioa}, zerbitzuaId={zerbitzuaId}, produktuaId={dbProduktuaId}): {ex.InnerException?.Message ?? ex.Message}", ex);
                }
            }

            tx.Commit();
            return zerbitzuaId;
        }

        public List<ZerbitzuaMahaiDTO> GetByMahai(int mahaiaId)
        {
            using var session = NHibernateHelper.OpenSession();
            var zerbitzuRows = session.CreateSQLQuery(
                    @"SELECT id, prezioTotala, data, erreserba_id, mahaiak_id, ordainduta
                      FROM zerbitzua
                      WHERE mahaiak_id = :mahaiaId
                      ORDER BY data DESC
                      LIMIT 50")
                .SetParameter("mahaiaId", mahaiaId)
                .List<object[]>();

            var result = zerbitzuRows.Select(z =>
            {
                var zerbitzuaId = Convert.ToInt32(z[0]);
                var produktuak = session.CreateSQLQuery(
                    @"SELECT e.id, e.produktua_id, COALESCE(e.izena, p.izena), p.irudia, e.data, e.prezioa, e.egoera, p.izena
                          FROM eskaerak e
                          LEFT JOIN produktuak p ON p.id = e.produktua_id
                          WHERE e.zerbitzua_id = :zerbitzuaId
                          ORDER BY e.id")
                    .SetParameter("zerbitzuaId", zerbitzuaId)
                    .List<object[]>();

                var ordainduta = z[5] != null && z[5] != DBNull.Value && Convert.ToInt32(z[5]) != 0;

                return new ZerbitzuaMahaiDTO
                {
                    Id = zerbitzuaId,
                    PrezioTotala = z[1] == DBNull.Value || z[1] == null ? 0 : Convert.ToDecimal(z[1]),
                    Data = z[2] == DBNull.Value || z[2] == null ? DateTime.MinValue : Convert.ToDateTime(z[2]),
                    ErreserbaId = z[3] == DBNull.Value || z[3] == null ? null : Convert.ToInt32(z[3]),
                    MahaiakId = z[4] == DBNull.Value || z[4] == null ? null : Convert.ToInt32(z[4]),
                    Ordainduta = ordainduta,
                    Eskaerak = produktuak.Select(ep => new ZerbitzuaEskaeraDTO
                    {
                        Id = Convert.ToInt32(ep[0]),
                        ProduktuaId = Convert.ToInt32(ep[1]),
                        Izena = ep[2]?.ToString() ?? string.Empty,
                        Irudia = BuildIrudiUrl(ep[3] == DBNull.Value ? null : ep[3]?.ToString()),
                        Data = ep[4] == DBNull.Value || ep[4] == null ? DateTime.MinValue : Convert.ToDateTime(ep[4]),
                        Prezioa = ep[5] == DBNull.Value || ep[5] == null ? 0 : Convert.ToDecimal(ep[5]),
                        Egoera = ordainduta ? 1 : (ep[6] == DBNull.Value || ep[6] == null ? 0 : Convert.ToInt32(ep[6])),
                        IsPlatera = IsPlaceholderPlatera(ep[1], ep[7])
                    }).ToList()
                };
            }).ToList();

            return result;
        }

        public bool Update(int id, ZerbitzuaController.ZerbitzuaSortuDto dto)
        {
            using var session = NHibernateHelper.OpenSession();
            using var tx = session.BeginTransaction();

            var zerbitzua = session.CreateSQLQuery("SELECT id, ordainduta FROM zerbitzua WHERE id = :id")
                .SetParameter("id", id)
                .UniqueResult<object[]>();

            if (zerbitzua == null) return false;

            var ordainduta = zerbitzua[1] != null && zerbitzua[1] != DBNull.Value && Convert.ToInt32(zerbitzua[1]) != 0;
            if (ordainduta)
            {
                throw new InvalidOperationException("Zerbitzua jada ordainduta dago; ezin da editatu.");
            }

            var oraingoProduktuak = session.CreateSQLQuery(
                    @"SELECT produktua_id, COUNT(*)
                      FROM eskaerak
                      WHERE zerbitzua_id = :id AND produktua_id <> 1
                      GROUP BY produktua_id")
                .SetParameter("id", id)
                .List<object[]>();

            foreach (var row in oraingoProduktuak)
            {
                session.CreateSQLQuery(
                        @"UPDATE produktuak
                          SET stock = stock + :kantitatea
                          WHERE id = :produktuId")
                    .SetParameter("kantitatea", Convert.ToInt32(row[1]))
                    .SetParameter("produktuId", Convert.ToInt32(row[0]))
                    .ExecuteUpdate();
            }

            EnsurePlateraProduktua(session);

            var taldekatuta = dto.Eskaerak
                .Where(e => !e.IsPlatera)
                .GroupBy(e => e.ProduktuaId)
                .Select(g => new
                {
                    ProduktuaId = g.Key,
                    Kantitatea = g.Count(),
                    Produktua = g.First()
                })
                .ToList();

            foreach (var item in taldekatuta)
            {
                var stockUpdate = session.CreateSQLQuery(
                        @"UPDATE produktuak
                          SET stock = stock - :kantitatea
                          WHERE id = :produktuId AND COALESCE(stock, 0) >= :kantitatea")
                    .SetParameter("kantitatea", item.Kantitatea)
                    .SetParameter("produktuId", item.ProduktuaId)
                    .ExecuteUpdate();

                if (stockUpdate == 0)
                {
                    var produktua = session.Query<Produktua>().FirstOrDefault(p => p.id == item.ProduktuaId);
                    if (produktua == null) throw new InvalidOperationException($"Produktua ez da existitzen: {item.ProduktuaId}");
                    throw new InvalidOperationException($"Stock nahikorik ez: {produktua.izena}");
                }
            }

            session.CreateSQLQuery("DELETE FROM eskaerak WHERE zerbitzua_id = :id")
                .SetParameter("id", id)
                .ExecuteUpdate();

            var data = DateTime.Now;
            foreach (var e in dto.Eskaerak)
            {
                var dbProduktuaId = e.IsPlatera ? 1 : e.ProduktuaId;
                session.CreateSQLQuery(
                        @"INSERT INTO eskaerak (izena, prezioa, data, egoera, zerbitzua_id, produktua_id)
                          VALUES (:izena, :prezioa, :data, :egoera, :zerbitzuaId, :produktuaId)")
                    .SetParameter("izena", e.Izena)
                    .SetParameter("prezioa", e.Prezioa)
                    .SetParameter("data", e.Data == default ? data : e.Data)
                    .SetParameter("egoera", e.Egoera)
                    .SetParameter("zerbitzuaId", id)
                    .SetParameter("produktuaId", dbProduktuaId)
                    .ExecuteUpdate();
            }

            session.CreateSQLQuery("UPDATE zerbitzua SET prezioTotala = :prezioTotala WHERE id = :id")
                .SetParameter("prezioTotala", dto.PrezioTotala)
                .SetParameter("id", id)
                .ExecuteUpdate();

            tx.Commit();
            return true;
        }

        public bool Ordaindu(int id)
        {
            using var session = NHibernateHelper.OpenSession();
            using var tx = session.BeginTransaction();
            var totalObj = session.CreateSQLQuery("SELECT prezioTotala FROM zerbitzua WHERE id = :id")
                .SetParameter("id", id)
                .UniqueResult();
            if (totalObj == null) return false;

            var pendingPlaterak = Convert.ToInt32(session.CreateSQLQuery(
                    @"SELECT COUNT(*)
                      FROM eskaerak e
                      WHERE e.zerbitzua_id = :id
                        AND e.produktua_id = 1
                        AND COALESCE(e.egoera, 0) = 0")
                .SetParameter("id", id)
                .UniqueResult());

            if (pendingPlaterak > 0)
            {
                throw new InvalidOperationException("Ezin da ordaindu: plater guztiak Prestatuta egon behar dira.");
            }

            session.CreateSQLQuery("UPDATE zerbitzua SET ordainduta = 1 WHERE id = :id")
                .SetParameter("id", id)
                .ExecuteUpdate();

            var fakturaExists = session.CreateSQLQuery("SELECT id FROM fakturak WHERE zerbitzua_id = :id LIMIT 1")
                .SetParameter("id", id)
                .UniqueResult();
            if (fakturaExists == null)
            {
                session.CreateSQLQuery(
                        @"INSERT INTO fakturak (prezio_totala, zerbitzua_id)
                          VALUES (:prezioTotala, :zerbitzuaId)")
                    .SetParameter("prezioTotala", Convert.ToDecimal(totalObj))
                    .SetParameter("zerbitzuaId", id)
                    .ExecuteUpdate();
            }

            tx.Commit();
            return true;
        }
    }
}
