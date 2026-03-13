using ErronkaApi.Modeloak;
using Mysqlx.Crud;
using NHibernate;

public class EskaeraProduktuakRepository
{
    private readonly ISessionFactory _sessionFactory;

    public EskaeraProduktuakRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public List<EskaeraProduktuak> GetByEskaeraId(int eskaeraId)
    {
        using var session = _sessionFactory.OpenSession();

        return session.Query<EskaeraProduktuak>()
            .Where(ep => ep.Eskaera.id == eskaeraId)
            .ToList();
    }

    public void Delete (EskaeraProduktuak ep)
    {
        using var session = _sessionFactory.OpenSession();
        using var tx = session.BeginTransaction();

        session.Delete(ep);

        tx.Commit();
    }
}