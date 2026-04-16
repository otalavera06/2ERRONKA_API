using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;

public class MahaiaMap : ClassMap<Mahaia>
{
    public MahaiaMap()
    {
        Table("mahaiak");

        Id(x => x.id).Column("id").GeneratedBy.Identity();
        Map(x => x.zenbakia).Formula("id").ReadOnly();
        Map(x => x.kapazitatea).Formula("4").ReadOnly();
        Map(x => x.egoera).Column("egoera");
        Map(x => x.izena).Column("izena");

    }
}
