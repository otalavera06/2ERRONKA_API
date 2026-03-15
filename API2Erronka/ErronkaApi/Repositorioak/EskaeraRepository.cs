using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

using ErronkaApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Repositorioak
{
    public class EskaeraRepository : ControllerBase
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
            session.Save(eskaera);
            tx.Commit();
        }

        public void Update(Eskaera eskaera)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            session.Update(eskaera);
            tx.Commit();
        }

        public void Delete(Eskaera eskaera)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            session.Delete(eskaera);
            tx.Commit();
        }

        public List<Eskaera> LortuEskaerak()
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<Eskaera>()
                .Where(e => e.egoera == "irekita")
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

        public List<Eskaera> LortuEskaerakOrdaintzeko()
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<Eskaera>()
                .Where(e => e.egoera == "ordainketa_pendiente")
                .OrderByDescending(e => e.sortzeData)
                .ToList();
        }
    }
}
