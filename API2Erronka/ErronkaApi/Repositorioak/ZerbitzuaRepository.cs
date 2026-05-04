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

        private static List<int> LortuPlaterarenOsagaiak(global::NHibernate.ISession session, int plateraId)
        {
            return session.CreateSQLQuery(
                    @"SELECT produktuak_id
                      FROM produktuak_has_platerak
                      WHERE platerak_id = :plateraId
                      ORDER BY produktuak_id")
                .SetParameter("plateraId", plateraId)
                .List<object>()
                .Select(Convert.ToInt32)
                .ToList();
        }

        private static void AldatuPlaterarenStocka(global::NHibernate.ISession session, int plateraId, int kantitatea, bool kendu)
        {
            var osagaiak = LortuPlaterarenOsagaiak(session, plateraId);
            if (!osagaiak.Any())
            {
                throw new InvalidOperationException($"Platerak ez dauka osagairik: {plateraId}");
            }

            foreach (var osagaiId in osagaiak)
            {
                var sql = kendu
                    ? @"UPDATE produktuak
                        SET stock = stock - :kantitatea
                        WHERE id = :produktuId AND COALESCE(stock, 0) >= :kantitatea"
                    : @"UPDATE produktuak
                        SET stock = stock + :kantitatea
                        WHERE id = :produktuId";

                var stockUpdate = session.CreateSQLQuery(sql)
                    .SetParameter("kantitatea", kantitatea)
                    .SetParameter("produktuId", osagaiId)
                    .ExecuteUpdate();

                if (kendu && stockUpdate == 0)
                {
                    var produktua = session.Query<Produktua>().FirstOrDefault(p => p.id == osagaiId);
                    throw new InvalidOperationException($"Stock nahikorik ez: {produktua?.izena ?? osagaiId.ToString()}");
                }
            }
        }

        private static void AldatuProduktuarenStocka(global::NHibernate.ISession session, int produktuaId, int kantitatea, bool kendu)
        {
            var sql = kendu
                ? @"UPDATE produktuak
                    SET stock = stock - :kantitatea
                    WHERE id = :produktuId AND COALESCE(stock, 0) >= :kantitatea"
                : @"UPDATE produktuak
                    SET stock = stock + :kantitatea
                    WHERE id = :produktuId";

            var stockUpdate = session.CreateSQLQuery(sql)
                .SetParameter("kantitatea", kantitatea)
                .SetParameter("produktuId", produktuaId)
                .ExecuteUpdate();

            if (kendu && stockUpdate == 0)
            {
                var produktua = session.Query<Produktua>().FirstOrDefault(p => p.id == produktuaId);
                if (produktua == null) throw new InvalidOperationException($"Produktua ez da existitzen: {produktuaId}");
                throw new InvalidOperationException($"Stock nahikorik ez: {produktua.izena}");
            }
        }

        private static int LortuEskaerarenProduktuId(global::NHibernate.ISession session, ZerbitzuaController.EskaeraSortuDto eskaera)
        {
            if (!eskaera.IsPlatera)
            {
                return eskaera.ProduktuaId;
            }

            return LortuPlaterarenOsagaiak(session, eskaera.ProduktuaId).First();
        }

        public int Create(ZerbitzuaController.ZerbitzuaSortuDto dto)
        {
            using var session = NHibernateHelper.OpenSession();
            using var tx = session.BeginTransaction();
            var zerbitzuData = dto.Data == default ? DateTime.Now : dto.Data;

            session.CreateSQLQuery(
                    @"INSERT INTO zerbitzua (prezioTotala, data, ordainduta, erreserba_id, mahaiak_id)
                      VALUES (:prezioTotala, :data, :ordainduta, :erreserbaId, :mahaiakId)")
                .SetParameter("prezioTotala", dto.PrezioTotala)
                .SetParameter("data", zerbitzuData)
                .SetParameter("ordainduta", 0)
                .SetParameter("erreserbaId", dto.ErreserbaId.HasValue ? dto.ErreserbaId.Value : (int?)null, global::NHibernate.NHibernateUtil.Int32)
                .SetParameter("mahaiakId", dto.MahaiakId!.Value)
                .ExecuteUpdate();

            var zerbitzuaId = Convert.ToInt32(session.CreateSQLQuery("SELECT LAST_INSERT_ID()").UniqueResult());

            foreach (var e in dto.Eskaerak)
            {
                if (e.IsPlatera) AldatuPlaterarenStocka(session, e.ProduktuaId, 1, true);
                else AldatuProduktuarenStocka(session, e.ProduktuaId, 1, true);

                var produktuaId = LortuEskaerarenProduktuId(session, e);

                session.CreateSQLQuery(
                        @"INSERT INTO eskaerak (izena, prezioa, data, egoera, zerbitzua_id, produktua_id)
                          VALUES (:izena, :prezioa, :data, :egoera, :zerbitzuaId, :produktuaId)")
                    .SetParameter("izena", e.Izena)
                    .SetParameter("prezioa", e.Prezioa)
                    .SetParameter("data", e.Data == default ? zerbitzuData : e.Data)
                    .SetParameter("egoera", e.Egoera)
                    .SetParameter("zerbitzuaId", zerbitzuaId)
                    .SetParameter("produktuaId", produktuaId)
                    .ExecuteUpdate();

                if (e.IsPlatera)
                {
                    var eskaeraId = Convert.ToInt32(session.CreateSQLQuery("SELECT LAST_INSERT_ID()").UniqueResult());
                    session.CreateSQLQuery(
                            @"INSERT INTO platerak_has_eskaerak (platerak_id, eskaerak_id)
                              VALUES (:plateraId, :eskaeraId)")
                        .SetParameter("plateraId", e.ProduktuaId)
                        .SetParameter("eskaeraId", eskaeraId)
                        .ExecuteUpdate();
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
                        @"SELECT e.id,
                                 COALESCE(phe.platerak_id, e.produktua_id),
                                 COALESCE(e.izena, pl.izena, p.izena),
                                 COALESCE(pl.argazkia, p.irudia),
                                 e.data,
                                 e.prezioa,
                                 e.egoera,
                                 CASE WHEN phe.platerak_id IS NULL THEN 0 ELSE 1 END
                          FROM eskaerak e
                          LEFT JOIN produktuak p ON p.id = e.produktua_id
                          LEFT JOIN platerak_has_eskaerak phe ON phe.eskaerak_id = e.id
                          LEFT JOIN platerak pl ON pl.id = phe.platerak_id
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
                    Eskaerak = produktuak.Select(ep => new ZerbitzuaEskaeraDTO
                    {
                        Id = Convert.ToInt32(ep[0]),
                        ProduktuaId = Convert.ToInt32(ep[1]),
                        Izena = ep[2]?.ToString() ?? string.Empty,
                        Irudia = BuildIrudiUrl(ep[3] == DBNull.Value ? null : ep[3]?.ToString()),
                        Data = ep[4] == DBNull.Value || ep[4] == null ? DateTime.MinValue : Convert.ToDateTime(ep[4]),
                        Prezioa = ep[5] == DBNull.Value || ep[5] == null ? 0 : Convert.ToDecimal(ep[5]),
                        Egoera = ordainduta ? 1 : (ep[6] == DBNull.Value || ep[6] == null ? 0 : Convert.ToInt32(ep[6])),
                        IsPlatera = ep[7] != DBNull.Value && Convert.ToInt32(ep[7]) == 1
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
                    @"SELECT e.produktua_id, phe.platerak_id
                      FROM eskaerak e
                      LEFT JOIN platerak_has_eskaerak phe ON phe.eskaerak_id = e.id
                      WHERE e.zerbitzua_id = :id")
                .SetParameter("id", id)
                .List<object[]>();

            foreach (var item in oraingoProduktuak.GroupBy(row => new
            {
                ProduktuaId = Convert.ToInt32(row[0]),
                PlateraId = row[1] == null || row[1] == DBNull.Value ? (int?)null : Convert.ToInt32(row[1])
            }))
            {
                if (item.Key.PlateraId.HasValue) AldatuPlaterarenStocka(session, item.Key.PlateraId.Value, item.Count(), false);
                else AldatuProduktuarenStocka(session, item.Key.ProduktuaId, item.Count(), false);
            }

            var taldekatuta = dto.Eskaerak
                .GroupBy(e => new { e.ProduktuaId, e.IsPlatera })
                .Select(g => new
                {
                    ProduktuaId = g.Key.ProduktuaId,
                    Kantitatea = g.Count(),
                    Produktua = g.First()
                })
                .ToList();

            foreach (var item in taldekatuta)
            {
                if (item.Produktua.IsPlatera) AldatuPlaterarenStocka(session, item.ProduktuaId, item.Kantitatea, true);
                else AldatuProduktuarenStocka(session, item.ProduktuaId, item.Kantitatea, true);
            }

            session.CreateSQLQuery(
                    @"DELETE phe
                      FROM platerak_has_eskaerak phe
                      INNER JOIN eskaerak e ON e.id = phe.eskaerak_id
                      WHERE e.zerbitzua_id = :id")
                .SetParameter("id", id)
                .ExecuteUpdate();

            session.CreateSQLQuery("DELETE FROM eskaerak WHERE zerbitzua_id = :id")
                .SetParameter("id", id)
                .ExecuteUpdate();

            var data = DateTime.Now;
            foreach (var e in dto.Eskaerak)
            {
                var produktuaId = LortuEskaerarenProduktuId(session, e);
                session.CreateSQLQuery(
                        @"INSERT INTO eskaerak (izena, prezioa, data, egoera, zerbitzua_id, produktua_id)
                          VALUES (:izena, :prezioa, :data, :egoera, :zerbitzuaId, :produktuaId)")
                    .SetParameter("izena", e.Izena)
                    .SetParameter("prezioa", e.Prezioa)
                    .SetParameter("data", e.Data == default ? data : e.Data)
                    .SetParameter("egoera", e.Egoera)
                    .SetParameter("zerbitzuaId", id)
                    .SetParameter("produktuaId", produktuaId)
                    .ExecuteUpdate();

                if (e.IsPlatera)
                {
                    var eskaeraId = Convert.ToInt32(session.CreateSQLQuery("SELECT LAST_INSERT_ID()").UniqueResult());
                    session.CreateSQLQuery(
                            @"INSERT INTO platerak_has_eskaerak (platerak_id, eskaerak_id)
                              VALUES (:plateraId, :eskaeraId)")
                        .SetParameter("plateraId", e.ProduktuaId)
                        .SetParameter("eskaeraId", eskaeraId)
                        .ExecuteUpdate();
                }
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
