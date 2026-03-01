using FluentNHibernate.Mapping;
using ErronkaApi.Modeloak;

namespace ErronkaApi.Mapeoak
{
    public class EskaeraHistorikoaMap : ClassMap<EskaeraHistorikoa>
    {
        public EskaeraHistorikoaMap()
        {
            Table("eskaera_historikoa");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.EskaeraId).Column("eskaera_id");
            Map(x => x.ItxieraData).Column("itxiera_data");
        }
    }
}
