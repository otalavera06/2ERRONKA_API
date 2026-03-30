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

            var eskaera = new Eskaera
            {
                erabiltzaileId = 1,
                mahaia_id = dto.MahaiakId!.Value,
                komensalak = 0,
                egoera = "irekita",
                sukaldeaEgoera = "zain",
                sortzeData = dto.Data == default ? DateTime.Now : dto.Data,
                itxieraData = null
            };

            session.Save(eskaera);

            var mahaia = session.Query<Mahaia>().FirstOrDefault(m => m.id == dto.MahaiakId.Value);
            if (mahaia != null)
            {
                var link = new EskaeraMahaiak
                {
                    Eskaera = eskaera,
                    Mahaia = mahaia
                };
                session.Save(link);
            }

            foreach (var e in dto.Eskaerak)
            {
                var produktua = session.Query<Produktua>().FirstOrDefault(p => p.id == e.ProduktuaId);
                if (produktua == null) throw new InvalidOperationException($"Produktua ez da existitzen: {e.ProduktuaId}");

                var ep = new EskaeraProduktuak
                {
                    Eskaera = eskaera,
                    Produktua = produktua,
                    Kantitatea = 1,
                    PrezioUnitarioa = e.Prezioa,
                    Guztira = e.Prezioa
                };
                session.Save(ep);
            }

            tx.Commit();
            return eskaera.id;
        }

        public List<ZerbitzuaMahaiDTO> GetByMahai(int mahaiaId)
        {
            using var session = NHibernateHelper.OpenSession();
            var eskaerak = session.Query<Eskaera>()
                .Where(e => e.mahaia_id == mahaiaId)
                .OrderByDescending(e => e.sortzeData)
                .Take(50)
                .ToList();

            var result = eskaerak.Select(e =>
            {
                var produktuak = session.Query<EskaeraProduktuak>()
                    .Where(ep => ep.Eskaera.id == e.id)
                    .ToList();

                var egoeraInt = string.Equals(e.egoera, "itxita", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
                var prezioTotala = produktuak.Sum(ep => ep.PrezioUnitarioa * ep.Kantitatea);

                return new ZerbitzuaMahaiDTO
                {
                    Id = e.id,
                    PrezioTotala = prezioTotala,
                    Data = e.sortzeData,
                    ErreserbaId = null,
                    MahaiakId = e.mahaia_id,
                    Eskaerak = produktuak.Select(ep => new ZerbitzuaEskaeraDTO
                    {
                        Id = ep.Id,
                        ProduktuaId = ep.Produktua.id,
                        Izena = ep.Produktua.izena,
                        Irudia = BuildIrudiUrl(ep.Produktua.irudia),
                        Data = e.sortzeData,
                        Prezioa = ep.PrezioUnitarioa,
                        Egoera = egoeraInt
                    }).ToList()
                };
            }).ToList();

            return result;
        }

        public bool Ordaindu(int id)
        {
            using var session = NHibernateHelper.OpenSession();
            using var tx = session.BeginTransaction();

            var eskaera = session.Query<Eskaera>().FirstOrDefault(e => e.id == id);
            if (eskaera == null) return false;

            eskaera.egoera = "itxita";
            eskaera.itxieraData = DateTime.Now;
            session.Update(eskaera);

            tx.Commit();
            return true;
        }
    }
}
