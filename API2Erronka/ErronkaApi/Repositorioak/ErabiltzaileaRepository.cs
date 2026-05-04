using ErronkaApi.DTOak;
using NHibernate;
using ErronkaApi.Modeloak;
using System;
using System.Collections.Generic;
using System.Linq;
using ErronkaApi.Interfaces;

namespace ErronkaApi.Repositorioak
{
    /// <summary>
    /// Langile eta erabiltzaileen autentifikazio-datuetarako sarbidea ematen duen biltegia.
    /// </summary>
    public class ErabiltzaileaRepository : IErabiltzaileaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// `ErabiltzaileaRepository` instantzia berri bat sortzen du.
        /// </summary>
        /// <param name="sessionFactory">NHibernate saio-fabrika.</param>
        public ErabiltzaileaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public List<LangileaDTO> GetAll()
        {
            using var session = _sessionFactory.OpenSession();

            var rows = session.CreateSQLQuery(
                    @"SELECT id, izena, abizena, erabiltzailea, email, telefonoa, baimena, mahaiak_id, chat_baimena
                      FROM langileak
                      ORDER BY id")
                .List<object[]>();

            return rows.Select(row => new LangileaDTO
            {
                Id = Convert.ToInt32(row[0]),
                Izena = row[1] == DBNull.Value ? null : row[1]?.ToString(),
                Abizena = row[2] == DBNull.Value ? null : row[2]?.ToString(),
                Erabiltzailea = row[3] == DBNull.Value ? null : row[3]?.ToString(),
                Email = row[4] == DBNull.Value ? null : row[4]?.ToString(),
                Telefonoa = row[5] == DBNull.Value ? null : row[5]?.ToString(),
                Baimena = row[6] != null && row[6] != DBNull.Value && Convert.ToInt32(row[6]) != 0,
                MahaiakId = row[7] == null || row[7] == DBNull.Value ? null : Convert.ToInt32(row[7]),
                ChatBaimena = row[8] != null && row[8] != DBNull.Value && Convert.ToInt32(row[8]) != 0
            }).ToList();
        }

        /// <summary>
        /// Erabiltzaile aktibo bat bilatzen du emandako kredentzialekin.
        /// </summary>
        /// <param name="erabiltzailea">Saioa hasteko erabiltzaile-izena.</param>
        /// <param name="pasahitza">Saioa hasteko pasahitza.</param>
        /// <returns>Aurkitutako erabiltzailea edo `null`.</returns>
        public Erabiltzailea? Login(string erabiltzailea, string pasahitza)
        {
            using var session = _sessionFactory.OpenSession();

            return session.Query<Erabiltzailea>()
                .FirstOrDefault(e => e.erabiltzailea == erabiltzailea && e.pasahitza == pasahitza && !e.ezabatua);
        }

        public Erabiltzailea? LortuErabiltzailea(int id)
        {
            using var session = _sessionFactory.OpenSession();
            return session.Get<Erabiltzailea>(id);
        }
    }
}
