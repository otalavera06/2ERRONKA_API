using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;
using Microsoft.OpenApi.Validations;
using NHibernate;

using ErronkaApi.Interfaces;

namespace ErronkaApi.Repositorioak
{
    /// <summary>
    /// Produktuen datu-sarbidea kudeatzen duen biltegia.
    /// Eskema berriko produktuak eta stock datuak bateratzen ditu.
    /// </summary>
    public class ProduktuaRepository : IProduktuaRepository
    {

        private readonly ISessionFactory _sessionFactory;

        public ProduktuaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public Produktua? Get(int id)
        {
            using var session = _sessionFactory.OpenSession();
            var produktua = session.Query<Produktua>().SingleOrDefault(x => x.id == id);
            return produktua;
        }
        public void Update(Produktua produktua)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            session.Update(produktua);

            tx.Commit();
        }

        public void Delete(Produktua produktua)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            session.Delete(produktua);

            tx.Commit();
        }

        public List<Produktua> GetAll()
        {
            using var session = NHibernateHelper.OpenSession();
            return session.Query<Produktua>().ToList();
        }

        public List<ProduktuaDTO> GetAllByKategoriaId(int katId)
        {
            return this.GetAll()
                        .Where(p => (p.produktuen_motak_id ?? 0) == katId)
                        .Select(p => new ProduktuaDTO(p))
                        .ToList();
        }

        public List<Produktua> GetByKategoria(int kategoriaId)
        {
            using var session = NHibernateHelper.OpenSession();
            return session.Query<Produktua>()
                          .Where(p => p.produktuen_motak_id == kategoriaId)
                          .ToList();
        }
    }
}

