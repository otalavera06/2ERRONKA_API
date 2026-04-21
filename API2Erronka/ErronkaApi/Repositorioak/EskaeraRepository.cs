using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

using ErronkaApi.Interfaces;

namespace ErronkaApi.Repositorioak
{
    /// <summary>
    /// Eskaeren biltegia.
    /// Eskema berrian, `Eskaera` zaharra `zerbitzua` + `eskaerak` egituran oinarritzen da.
    /// </summary>
    public class EskaeraRepository : IEskaeraRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public EskaeraRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// Eskaera bat IDaren arabera lortzen du.
        /// </summary>
        /// <param name="id">Eskaeraren IDa.</param>
        /// <returns>Eskaera objektua edo null.</returns>
        public Eskaera? Get(int id)
        {
            using var session = _sessionFactory.OpenSession();
            var eskaera = session.Get<Eskaera>(id);
            return eskaera;
        }

        /// <summary>
        /// Eskaera bat gordetzen du datu-basean.
        /// </summary>
        /// <param name="eskaera">Gordetzeko eskaera.</param>
        public void Save(Eskaera eskaera)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            session.CreateSQLQuery(
                    @"INSERT INTO zerbitzua (prezioTotala, data, ordainduta, erreserba_id, mahaiak_id)
                      VALUES (:prezioTotala, :data, :ordainduta, :erreserbaId, :mahaiakId)")
                .SetParameter("prezioTotala", 0m)
                .SetParameter("data", eskaera.sortzeData == default ? DateTime.Now : eskaera.sortzeData)
                .SetParameter("ordainduta", 0)
                .SetParameter("erreserbaId", (int?)null, global::NHibernate.NHibernateUtil.Int32)
                .SetParameter("mahaiakId", eskaera.mahaia_id)
                .ExecuteUpdate();
            eskaera.id = Convert.ToInt32(session.CreateSQLQuery("SELECT LAST_INSERT_ID()").UniqueResult());
            tx.Commit();
        }

        /// <summary>
        /// Eskaera bat eguneratzen du datu-basean.
        /// </summary>
        /// <param name="eskaera">Eguneratzeko eskaera.</param>
        public void Update(Eskaera eskaera)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            if (string.Equals(eskaera.egoera, "itxita", StringComparison.OrdinalIgnoreCase))
            {
                session.CreateSQLQuery("UPDATE zerbitzua SET ordainduta = 1 WHERE id = :id")
                    .SetParameter("id", eskaera.id)
                    .ExecuteUpdate();
            }
            else if (string.Equals(eskaera.egoera, "irekita", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(eskaera.egoera, "ordainketa_pendiente", StringComparison.OrdinalIgnoreCase))
            {
                session.CreateSQLQuery("UPDATE zerbitzua SET ordainduta = 0 WHERE id = :id")
                    .SetParameter("id", eskaera.id)
                    .ExecuteUpdate();
            }

            tx.Commit();
        }

        /// <summary>
        /// Eskaera baten sukaldeko egoera eguneratzen du.
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <param name="egoera">Egoera berria.</param>
        public void EguneratuSukaldeaEgoera(int eskaeraId, string egoera)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            var egoeraInt = egoera.ToLower() switch
            {
                "zain" => 0,
                "prest" => 1,
                "ordainduta" => 2,
                _ => 0
            };

            session.CreateSQLQuery("UPDATE eskaerak SET egoera = :egoera WHERE zerbitzua_id = :id AND egoera = 0")
                .SetParameter("egoera", egoeraInt)
                .SetParameter("id", eskaeraId)
                .ExecuteUpdate();
            
            tx.Commit();
        }

        /// <summary>
        /// Eskaera bat ezabatzen du datu-basean.
        /// </summary>
        /// <param name="eskaera">Ezabatzeko eskaera.</param>
        public void Delete(Eskaera eskaera)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            session.CreateSQLQuery("DELETE FROM eskaerak WHERE zerbitzua_id = :id")
                .SetParameter("id", eskaera.id)
                .ExecuteUpdate();
            session.CreateSQLQuery("DELETE FROM zerbitzua WHERE id = :id");
            tx.Commit();
        }

        /// <summary>
        /// Eskaera guztiak lortzen ditu, itxita ez daudenak.
        /// </summary>
        /// <returns>Eskaeren zerrenda.</returns>
        public List<Eskaera> LortuEskaerak()
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<Eskaera>()
                .Where(e => e.egoera != "itxita")
                .OrderByDescending(e => e.sortzeData)
                .ToList();
        }

        /// <summary>
        /// Eskaera baten produktuak lortzen ditu.
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <returns>Eskaera-produktuen zerrenda.</returns>
        public List<EskaeraProduktuak> LortuEskaeraProduktuak(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<EskaeraProduktuak>()
                .Fetch(ep => ep.Produktua)
                .Where(ep => ep.Eskaera.id == eskaeraId)
                .ToList();
        }

        /// <summary>
        /// Eskaera guztiak lortzen ditu ordenatuta.
        /// </summary>
        /// <returns>Eskaeren zerrenda.</returns>
        public List<Eskaera> LortuEskaerak2()
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<Eskaera>()
                .OrderByDescending(e => e.sortzeData)
                .ToList();
        }

        /// <summary>
        /// Eskaera baten produktuak lortzen ditu (beste metodoa).
        /// </summary>
        /// <param name="eskaeraId">Eskaeraren IDa.</param>
        /// <returns>Eskaera-produktuen zerrenda.</returns>
        public List<EskaeraProduktuak> LortuEskaeraProduktuak2(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<EskaeraProduktuak>()
                .Fetch(ep => ep.Produktua)
                .Where(ep => ep.Eskaera.id == eskaeraId)
                .ToList();
        }

        /// <summary>
        /// Ordaintzeko dauden eskaerak lortzen ditu.
        /// </summary>
        /// <returns>Eskaeren zerrenda.</returns>
        public List<Eskaera> LortuEskaerakOrdaintzeko()
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<Eskaera>()
                .Where(e => e.egoera != "itxita")
                .OrderByDescending(e => e.sortzeData)
                .ToList();
        }

        /// <summary>
        /// Sukaldeko eskaerak lortzen ditu.
        /// </summary>
        /// <returns>Eskaera-produktuen zerrenda sukaldearentzat.</returns>
        public List<EskaeraProduktuak> LortuSukaldekoEskaerak()
        {
            using var session = _sessionFactory.OpenSession();
            var query = session.Query<EskaeraProduktuak>()
                .Where(ep => ep.Egoera == 0 && ep.Eskaera.mahaia_id >= 1 && ep.Eskaera.mahaia_id <= 5);
            query.Fetch(ep => ep.Produktua).ToFuture();
            query.Fetch(ep => ep.Eskaera).ToFuture();
            return query.ToFuture().ToList();
        }

        /// <summary>
        /// Eskaera berri bat sortzen du DTOtik.
        /// </summary>
        /// <param name="dto">Eskaera sortzeko datuak.</param>
        /// <returns>Sortutako eskaera.</returns>
        public Eskaera SortuEskaera(EskaeraSortuDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            var eskaera = new Eskaera
            {
                mahaia_id = dto.MahaiaId,
                komensalak = dto.Komensalak,
                sortzeData = DateTime.Now,
                egoera = "irekita",
                EskaeraProduktuak = new List<EskaeraProduktuak>(),
                EskaeraMahaiak = new List<EskaeraMahaiak>()
            };

            session.Save(eskaera);

            foreach (var p in dto.Produktuak)
            {
                var produktua = session.Get<Produktua>(p.ProduktuaId);
                if (produktua != null && produktua.stock_aktuala >= p.Kantitatea)
                {
                    var ep = new EskaeraProduktuak
                    {
                        Eskaera = eskaera,
                        Produktua = produktua,
                        Kantitatea = p.Kantitatea,
                        PrezioUnitarioa = produktua.prezioa,
                        Guztira = produktua.prezioa * p.Kantitatea,
                        Egoera = 0
                    };
                    eskaera.EskaeraProduktuak.Add(ep);
                    session.Save(ep);

                    produktua.stock_aktuala -= p.Kantitatea;
                    session.Update(produktua);
                }
            }

            //var mahaia = session.Get<Mahaia>(dto.MahaiaId);
            //if (mahaia != null)
            //{
            //    mahaia = "okupatuta";
            //    session.Update(mahaia);
            //}

            tx.Commit();
            return eskaera;
        }
    }
}
