using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;
using Microsoft.OpenApi.Validations;
using NHibernate;

namespace ErronkaApi.Repositorioak
{
    public class ProduktuaRepository
    {

        private readonly ISessionFactory _sessionFactory;

        public ProduktuaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public Produktua? Get(int id)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            var query = session.Query<Produktua>()
                .Where(x => x.id == id);

            var produktua = query.SingleOrDefault();
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
                        .Where(p => p.kategoria.id == katId)
                        .Select(p => new ProduktuaDTO(p))
                        .ToList();
        }

        public List<Produktua> GetByKategoria(int kategoriaId)
        {
            using var session = NHibernateHelper.OpenSession();
            return session.Query<Produktua>()
                          .Where(p => p.kategoria.id == kategoriaId)
                          .ToList();
        }
    }
}

