using ErronkaApi.DTOak;
using ErronkaApi.Interfaces;
using NHibernate;
using System;
using System.Collections.Generic;

namespace ErronkaApi.Repositorioak
{
    public class PlateraRepository : IPlateraRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public PlateraRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public List<PlateraDTO> GetAll(string baseUrl)
        {
            using var session = _sessionFactory.OpenSession();

            var rows = session.CreateSQLQuery(@"
                    SELECT
                        pl.id,
                        pl.izena,
                        pl.mota,
                        pl.prezioa,
                        pl.argazkia,
                        pr.id AS osagaia_id,
                        pr.izena AS osagaia_izena,
                        pr.stock AS osagaia_stock
                    FROM platerak pl
                    LEFT JOIN produktuak_has_platerak php ON php.platerak_id = pl.id
                    LEFT JOIN produktuak pr ON pr.id = php.produktuak_id
                        AND pr.produktuen_motak_id = 8
                    ORDER BY pl.mota, pl.id, pr.izena")
                .List<object[]>();

            var platerak = new Dictionary<int, PlateraDTO>();

            foreach (var row in rows)
            {
                var plateraId = Convert.ToInt32(row[0]);

                if (!platerak.TryGetValue(plateraId, out var dto))
                {
                    var argazkia = row[4] == DBNull.Value ? null : row[4]?.ToString();

                    dto = new PlateraDTO
                    {
                        Id = plateraId,
                        Izena = row[1] == DBNull.Value ? string.Empty : row[1]?.ToString(),
                        Mota = row[2] == DBNull.Value ? string.Empty : row[2]?.ToString(),
                        Prezioa = row[3] == DBNull.Value ? 0 : Convert.ToSingle(row[3]),
                        Argazkia = argazkia,
                        ArgazkiaUrl = string.IsNullOrWhiteSpace(argazkia) ? null : $"{baseUrl}/irudiak/{argazkia}"
                    };

                    platerak[plateraId] = dto;
                }

                if (row[5] != null && row[5] != DBNull.Value)
                {
                    dto.Osagaiak.Add(new PlateraOsagaiaDTO
                    {
                        Id = Convert.ToInt32(row[5]),
                        Izena = row[6] == DBNull.Value ? string.Empty : row[6]?.ToString(),
                        Stock = row[7] == DBNull.Value ? 0 : Convert.ToInt32(row[7])
                    });
                }
            }

            return new List<PlateraDTO>(platerak.Values);
        }
    }
}
