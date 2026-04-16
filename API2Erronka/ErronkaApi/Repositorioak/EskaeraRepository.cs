using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate;
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

        public Eskaera? Get(int id)
        {
            using var session = _sessionFactory.OpenSession();
            var eskaera = session.Get<Eskaera>(id);
            return eskaera;
        }

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

        public void EguneratuSukaldeaEgoera(int eskaeraId, string egoera)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            var egoeraInt = egoera.ToLower() switch
            {
                "zain" => 0,
                "hasi" => 1,
                "prest" => 2,
                _ => 0
            };

            session.CreateSQLQuery("UPDATE eskaerak SET egoera = :egoera WHERE zerbitzua_id = :id")
                .SetParameter("egoera", egoeraInt)
                .SetParameter("id", eskaeraId)
                .ExecuteUpdate();
            
            tx.Commit();
        }

        public void Delete(Eskaera eskaera)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            session.CreateSQLQuery("DELETE FROM eskaerak WHERE zerbitzua_id = :id")
                .SetParameter("id", eskaera.id)
                .ExecuteUpdate();
            session.CreateSQLQuery("DELETE FROM zerbitzua WHERE id = :id")
                .SetParameter("id", eskaera.id)
                .ExecuteUpdate();
            tx.Commit();
        }

        public List<Eskaera> LortuEskaerak()
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<Eskaera>()
                .Where(e => e.egoera != "itxita")
                .OrderByDescending(e => e.sortzeData)
                .ToList();
        }

        public List<EskaeraProduktuak> LortuEskaeraProduktuak(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<EskaeraProduktuak>()
                .Where(ep => ep.Eskaera.id == eskaeraId)
                .ToList();
        }

        public List<Eskaera> LortuEskaerak2()
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<Eskaera>()
                .OrderByDescending(e => e.sortzeData)
                .ToList();
        }

        public List<EskaeraProduktuak> LortuEskaeraProduktuak2(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<EskaeraProduktuak>()
                .Where(ep => ep.Eskaera.id == eskaeraId)
                .ToList();
        }

        public List<Eskaera> LortuEskaerakOrdaintzeko()
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<Eskaera>()
                .Where(e => e.egoera != "itxita")
                .OrderByDescending(e => e.sortzeData)
                .ToList();
        }
    }
}
