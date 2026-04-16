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
                var stockUpdate = session.CreateSQLQuery(
                        @"UPDATE produktuak
                          SET stock = stock - 1
                          WHERE id = :produktuId AND COALESCE(stock, 0) >= 1")
                    .SetParameter("produktuId", e.ProduktuaId)
                    .ExecuteUpdate();

                if (stockUpdate == 0)
                {
                    var produktua = session.Query<Produktua>().FirstOrDefault(p => p.id == e.ProduktuaId);
                    if (produktua == null) throw new InvalidOperationException($"Produktua ez da existitzen: {e.ProduktuaId}");
                    throw new InvalidOperationException($"Stock nahikorik ez: {produktua.izena}");
                }

                session.CreateSQLQuery(
                        @"INSERT INTO eskaerak (izena, prezioa, data, egoera, zerbitzua_id, produktua_id)
                          VALUES (:izena, :prezioa, :data, :egoera, :zerbitzuaId, :produktuaId)")
                    .SetParameter("izena", e.Izena)
                    .SetParameter("prezioa", e.Prezioa)
                    .SetParameter("data", e.Data == default ? zerbitzuData : e.Data)
                    .SetParameter("egoera", e.Egoera)
                    .SetParameter("zerbitzuaId", zerbitzuaId)
                    .SetParameter("produktuaId", e.ProduktuaId)
                    .ExecuteUpdate();
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
                        @"SELECT e.id, e.produktua_id, COALESCE(p.izena, e.izena), p.irudia, e.data, e.prezioa, e.egoera
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
                    Eskaerak = produktuak.Select(ep => new ZerbitzuaEskaeraDTO
                    {
                        Id = Convert.ToInt32(ep[0]),
                        ProduktuaId = Convert.ToInt32(ep[1]),
                        Izena = ep[2]?.ToString() ?? string.Empty,
                        Irudia = BuildIrudiUrl(ep[3] == DBNull.Value ? null : ep[3]?.ToString()),
                        Data = ep[4] == DBNull.Value || ep[4] == null ? DateTime.MinValue : Convert.ToDateTime(ep[4]),
                        Prezioa = ep[5] == DBNull.Value || ep[5] == null ? 0 : Convert.ToDecimal(ep[5]),
                        Egoera = ordainduta ? 1 : (ep[6] == DBNull.Value || ep[6] == null ? 0 : Convert.ToInt32(ep[6]))
                    }).ToList()
                };
            }).ToList();

            return result;
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
