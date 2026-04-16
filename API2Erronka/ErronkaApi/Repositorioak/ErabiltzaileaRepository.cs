using NHibernate;
using ErronkaApi.Modeloak;
using System.Linq;
using ErronkaApi.Interfaces;

namespace ErronkaApi.Repositorioak
{
    /// <summary>
    /// Langile eta erabiltzaileen autentifikazio-datuetarako sarbidea ematen duen biltegia.
    /// </summary>
    public class ErabiltzaileaRepository : IErabiltzaileaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// `ErabiltzaileaRepository` instantzia berri bat sortzen du.
        /// </summary>
        /// <param name="sessionFactory">NHibernate saio-fabrika.</param>
        public ErabiltzaileaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// Erabiltzaile aktibo bat bilatzen du emandako kredentzialekin.
        /// </summary>
        /// <param name="erabiltzailea">Saioa hasteko erabiltzaile-izena.</param>
        /// <param name="pasahitza">Saioa hasteko pasahitza.</param>
        /// <returns>Aurkitutako erabiltzailea edo `null`.</returns>
        public Erabiltzailea? Login(string erabiltzailea, string pasahitza)
        {
            using var session = _sessionFactory.OpenSession();

            return session.Query<Erabiltzailea>()
                .FirstOrDefault(e => e.erabiltzailea == erabiltzailea && e.pasahitza == pasahitza && !e.ezabatua);
        }
    }
}
