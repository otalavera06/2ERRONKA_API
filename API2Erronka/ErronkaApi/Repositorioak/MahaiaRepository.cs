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
    public class MahaiaRepository : IMahaiaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public MahaiaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Delete(Mahaia mahaia)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            session.Delete(mahaia);

            tx.Commit();
        }

        public Mahaia? Get(int id)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            var query = session.Query<Mahaia>()
                .Where(x => x.id == id);

            var mahaia = query.SingleOrDefault();
            return mahaia;

        }

        public void Update(Mahaia mahaia)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            session.Update(mahaia);

            tx.Commit();
        }



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
