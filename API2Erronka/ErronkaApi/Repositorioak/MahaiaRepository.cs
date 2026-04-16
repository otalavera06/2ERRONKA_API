using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using FluentNHibernate.Testing.Values;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ErronkaApi.Interfaces;

namespace ErronkaApi.Repositorioak
{
    /// <summary>
    /// Mahaien egoera eta kontsultak kudeatzen dituen datu-biltegia.
    /// </summary>
    public class MahaiaRepository : IMahaiaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// `MahaiaRepository` instantzia berri bat sortzen du.
        /// </summary>
        /// <param name="sessionFactory">NHibernate saio-fabrika.</param>
        public MahaiaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// Mahaia bat datu-basean ezabatzen du.
        /// </summary>
        /// <param name="mahaia">Ezabatu beharreko mahaia.</param>
        public void Delete(Mahaia mahaia)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            session.Delete(mahaia);

            tx.Commit();
        }

        /// <summary>
        /// Bere identifikatzailearen arabera mahaia bat lortzen du.
        /// </summary>
        /// <param name="id">Bilatu beharreko mahaiaren identifikatzailea.</param>
        /// <returns>Aurkitutako mahaia edo `null`.</returns>
        public Mahaia? Get(int id)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            var query = session.Query<Mahaia>()
                .Where(x => x.id == id);

            var mahaia = query.SingleOrDefault();
            return mahaia;

        }

        /// <summary>
        /// Mahaia baten egoera edo atributuak eguneratzen ditu.
        /// </summary>
        /// <param name="mahaia">Eguneratu beharreko mahaia.</param>
        public void Update(Mahaia mahaia)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            session.Update(mahaia);

            tx.Commit();
        }



        /// <summary>
        /// Une honetan libre dauden mahaiak DTO formatuan lortzen ditu.
        /// </summary>
        /// <returns>Mahai libreen zerrenda edo `null` errorea gertatzen bada.</returns>
        public List<MahaiaDTO> LortuMahaiLibre()
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                return session.Query<Mahaia>()
                    .Where(m => m.egoera == "libre")
                    .Select(m => new MahaiaDTO
                    {
                        Id = m.id,
                        Zenbakia = m.zenbakia,
                        kapazitatea = m.kapazitatea
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
