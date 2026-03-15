using ErronkaApi.Modeloak;
using NHibernate;
using ErronkaApi.Interfaces;

namespace ErronkaApi.Repositorioak
{
    public class EskaeraMahaiakRepository : IEskaeraMahaiakRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public EskaeraMahaiakRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Delete(EskaeraMahaiak em)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            session.Delete(em);

            tx.Commit();
        }
    }
}
