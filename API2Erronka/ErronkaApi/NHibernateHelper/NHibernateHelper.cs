using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using ErronkaApi.Mapeoak;
using NH = NHibernate;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ErronkaApi.NHibernate
{
    public class NHibernateHelper
    {
        private static NH.ISessionFactory _sessionFactory;

        public static NH.ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    InitializeSessionFactory();
                return _sessionFactory;
            }
        }

        private static string? TryGetConnectionString()
        {
            var cwdConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = cwdConfig.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(cs)) return cs;

            var baseDirConfig = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            cs = baseDirConfig.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(cs)) return cs;

            return null;
        }

        private static void InitializeSessionFactory()
        {
            var connectionString = TryGetConnectionString() ?? "Server=localhost;Port=3306;Database=erronka2_2026;Uid=root;Pwd=1mg2024;";
            _sessionFactory = Fluently.Configure()
                .Database(
                    MySQLConfiguration.Standard
                        .ConnectionString(connectionString)
                )
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssemblyOf<ErabiltzaileaMap>();
                    m.FluentMappings.AddFromAssemblyOf<ProduktuaMap>();
                    m.FluentMappings.AddFromAssemblyOf<EskaeraMap>();
                    m.FluentMappings.AddFromAssemblyOf<MahaiaMap>();
                    m.FluentMappings.AddFromAssemblyOf<EskaeraMahaiakMap>();
                    m.FluentMappings.AddFromAssemblyOf<RolaMap>();
                    m.FluentMappings.AddFromAssemblyOf<EskaeraProduktuakMap>();
                    m.FluentMappings.AddFromAssemblyOf<KategoriaMap>();
                    m.FluentMappings.AddFromAssemblyOf<EskaeraHistorikoaMap>();
                    m.FluentMappings.AddFromAssemblyOf<ErreserbaMap>();
                })
                .ExposeConfiguration(cfg =>
                {
                    cfg.SetProperty("current_session_context_class", "call");
                })
                .BuildSessionFactory();
        }
        public static NH.ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
