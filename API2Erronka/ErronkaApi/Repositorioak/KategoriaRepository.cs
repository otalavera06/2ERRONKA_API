using ErronkaApi.Modeloak;
using MySqlX.XDevAPI;
using NHibernate;
using System.Collections.Generic;
using System.Linq;


namespace ErronkaApi.Repositorioak
{
    public class KategoriaRepository
    {
        private readonly ISessionFactory _sessionFactory;
        public KategoriaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }
        
        public IList<Kategoria> GetAll()
        {
            using var session = _sessionFactory.OpenSession();

            return session.Query<Kategoria>()
                          .OrderBy(k => k.izena)
                          .ToList();
        }
    }
}
