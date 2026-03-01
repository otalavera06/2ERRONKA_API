using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErronkaApi.Repositorioak
{
    public class MahaiaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public MahaiaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task<ErantzunaDTO<MahaiaDTO>> LortuMahaiLibreAsync()
        {
            try
            {
                return await Task.Run(() =>
                {
                    using var session = _sessionFactory.OpenSession();

                    var mahaiakLibreak = session.Query<Mahaia>()
                        .Where(m => m.egoera == "libre")
                        .Select(m => new MahaiaDTO
                        {
                            Id = m.id,
                            Zenbakia = m.zenbakia
                        })
                        .ToList();

                    if (!mahaiakLibreak.Any())
                    {
                        return new ErantzunaDTO<MahaiaDTO>
                        {
                            Code = 404,
                            Message = "Ez dago mahai librerik",
                            Datuak = null
                        };
                    }

                    return new ErantzunaDTO<MahaiaDTO>
                    {
                        Code = 200,
                        Message = "Mahai libreak lortu dira",
                        Datuak = mahaiakLibreak
                    };
                });
            }
            catch (Exception ex)
            {
                return new ErantzunaDTO<MahaiaDTO>
                {
                    Code = 500,
                    Message = "Errore bat egon da: " + ex.Message,
                    Datuak = null
                };
            }
        }
    }
}
