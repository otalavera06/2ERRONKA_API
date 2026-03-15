using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;

namespace ErronkaApi.Mapeoak
{
    public class EskaeraMahaiakMap : ClassMap<EskaeraMahaiak>
    {
        public EskaeraMahaiakMap()
        {
            Table("eskaera_mahaiak");

            Id(x => x.Id).GeneratedBy.Identity();

            References(x => x.Eskaera)
                .Column("eskaera_id")
                .Not.Nullable();

            References(x => x.Mahaia)
                .Column("mahaia_id")
                .Not.Nullable();
        }
    }
}