using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;

public class MahaiaMap : ClassMap<Mahaia>
{
    public MahaiaMap()
    {
        Table("mahaiak");

        Id(x => x.id).Column("id").GeneratedBy.Identity();
        Map(x => x.zenbakia).Formula("id").ReadOnly();
        Map(x => x.kapazitatea).Column("kapazitatea");
        Map(x => x.egoera).Column("egoera");
        Map(x => x.izena).Column("izena");

        HasMany(x => x.EskaeraMahaiak)
            .KeyColumn("mahaia_id")
            .Cascade.All()
            .Inverse();
    }
}
