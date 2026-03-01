using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;

namespace ErronkaApi.Repositorioak
{
    public class ProduktuaRepository
    {
        public List<Produktua> GetAll()
        {
            using var session = NHibernateHelper.OpenSession();
            return session.Query<Produktua>().ToList();
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

