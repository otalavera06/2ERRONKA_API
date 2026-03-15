using ErronkaApi.Modeloak;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using ErronkaApi.Interfaces;

namespace ErronkaApi.Repositorioak
{
    public class EskaeraProduktuakRepository : IEskaeraProduktuakRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public EskaeraProduktuakRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public List<EskaeraProduktuak> GetByEskaeraId(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();

            return session.Query<EskaeraProduktuak>()
                .Where(ep => ep.Eskaera.id == eskaeraId)
                .ToList();
        }

        public void Update(EskaeraProduktuak ep)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            session.Update(ep);

            tx.Commit();
        }

        public void Delete(EskaeraProduktuak ep)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            session.Delete(ep);

            tx.Commit();
        }
    }
}
