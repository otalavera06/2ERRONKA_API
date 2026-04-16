using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
using ErronkaApi.Modeloak;
using MySqlX.XDevAPI;
using NHibernate;
using System.Collections.Generic;
using System.Linq;


namespace ErronkaApi.Repositorioak
{
    /// <summary>
    /// Kategorien datu-sarbidea kudeatzen duen biltegia.
    /// Entitate osoak nahiz APIrako DTO arinak itzultzen ditu.
    /// </summary>
    public class KategoriaRepository : IKategoriaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// `KategoriaRepository` instantzia berri bat sortzen du.
        /// </summary>
        /// <param name="sessionFactory">NHibernate saio-fabrika.</param>
        public KategoriaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }
        
        /// <summary>
        /// Kategoria guztiak izenaren arabera ordenatuta lortzen ditu.
        /// </summary>
        /// <returns>Kategoria entitateen zerrenda.</returns>
        public List<Kategoria> GetAll()
        {
            using var session = _sessionFactory.OpenSession();

            return session.Query<Kategoria>()
                          .OrderBy(k => k.izena)
                          .ToList();
        }

        /// <summary>
        /// Kategoria guztiak APIrako DTO formatuan lortzen ditu.
        /// </summary>
        /// <returns>`KategoriaDTO` zerrenda bat.</returns>
        public List<KategoriaDTO> GetAllDTO()
        {
            return GetAll()
                .Select(k => new KategoriaDTO
                {
                    id = k.id,
                    izena = k.izena
                })
                .ToList();
        }
    }
}
