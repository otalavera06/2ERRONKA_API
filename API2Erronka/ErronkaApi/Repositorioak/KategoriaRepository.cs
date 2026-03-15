using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
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
        
        public List<Kategoria> GetAll()
        {
            using var session = _sessionFactory.OpenSession();

            return session.Query<Kategoria>()
                          .OrderBy(k => k.izena)
                          .ToList();
        }

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
