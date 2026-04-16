using ErronkaApi.Interfaces;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Modeloak;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ErronkaApi.Repositorioak
{
    public class ErreserbaRepository : IErreserbaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public ErreserbaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public List<Erreserba> GetByDate(DateTime eguna, bool mota)
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<Erreserba>()
                .Where(r => r.Data.Date == eguna.Date && r.Mota == mota)
                .ToList();
        }

        public Erreserba Create(ErreserbakController.ErreserbakSortuDto dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            var erabiltzaileakId = NormalizeErabiltzaileaId(session, dto.ErabiltzaileakId);

            var entity = new Erreserba
            {
                Data = dto.Data,
                Mota = dto.Mota,
                ErabiltzaileakId = erabiltzaileakId,
                MahaiakId = dto.MahaiakId
            };

            session.Save(entity);
            tx.Commit();
            return entity;
        }

        public bool UpdateByMahai(int mahaiaId, DateTime eguna, bool mota, ErreserbakController.ErreserbakUpdateDto dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            var entity = session.Query<Erreserba>()
                .FirstOrDefault(r => r.MahaiakId == mahaiaId && r.Data.Date == eguna.Date && r.Mota == mota);
            if (entity == null) return false;

            if (dto.Data.HasValue) entity.Data = dto.Data.Value;
            if (dto.Mota.HasValue) entity.Mota = dto.Mota.Value;
            if (dto.ErabiltzaileakId.HasValue) entity.ErabiltzaileakId = NormalizeErabiltzaileaId(session, dto.ErabiltzaileakId);
            if (dto.MahaiakId.HasValue) entity.MahaiakId = dto.MahaiakId.Value;

            session.Update(entity);
            tx.Commit();
            return true;
        }

        public bool DeleteByMahai(int mahaiaId, DateTime eguna, bool mota)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            var entity = session.Query<Erreserba>()
                .FirstOrDefault(r => r.MahaiakId == mahaiaId && r.Data.Date == eguna.Date && r.Mota == mota);
            if (entity == null) return false;

            session.Delete(entity);
            tx.Commit();
            return true;
        }

        private static int? NormalizeErabiltzaileaId(global::NHibernate.ISession session, int? erabiltzaileakId)
        {
            if (!erabiltzaileakId.HasValue) return null;

            var exists = session.CreateSQLQuery("SELECT COUNT(*) FROM erabiltzaileak WHERE id = :id")
                .SetParameter("id", erabiltzaileakId.Value)
                .UniqueResult();

            return Convert.ToInt32(exists) > 0 ? erabiltzaileakId : null;
        }
    }
}
