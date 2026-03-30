using NHibernate;
using ErronkaApi.Modeloak;
using System.Linq;
using ErronkaApi.Interfaces;

namespace ErronkaApi.Repositorioak
{
    public class ErabiltzaileaRepository : IErabiltzaileaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public ErabiltzaileaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public Erabiltzailea? Login(string erabiltzailea, string pasahitza)
        {
            using var session = _sessionFactory.OpenSession();

            return session.Query<Erabiltzailea>()
                .FirstOrDefault(e => e.erabiltzailea == erabiltzailea && e.pasahitza == pasahitza && !e.ezabatua);
        }
    }
}
