using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;

namespace ErronkaApi.Mapeoak
{
    public class ErreserbaMap : ClassMap<Erreserba>
    {
        public ErreserbaMap()
        {
            Table("erreserbak");
            Id(x => x.Id).Column("id").GeneratedBy.Identity();
            Map(x => x.Data).Column("data");
            Map(x => x.Mota).Column("mota");
            Map(x => x.ErabiltzaileakId).Column("erabiltzaileak_id");
            Map(x => x.MahaiakId).Column("mahaiak_id");
        }
    }
}
