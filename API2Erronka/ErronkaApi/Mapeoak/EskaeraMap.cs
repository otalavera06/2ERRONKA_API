using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;

public class EskaeraMap : ClassMap<Eskaera>
{
    public EskaeraMap()
    {
        Table("zerbitzua");
        DynamicUpdate();
        Id(x => x.id).Column("id").GeneratedBy.Identity();
        Map(x => x.mahaia_id).Column("mahaiak_id");
        Map(x => x.erabiltzaileId).Formula("(0)").ReadOnly();
        Map(x => x.komensalak).Formula("(0)").ReadOnly();
        Map(x => x.egoera).Formula("(case when coalesce(ordainduta,0) = 1 then 'itxita' else 'irekita' end)").ReadOnly();
        Map(x => x.sukaldeaEgoera).Formula("(select case coalesce(max(e.egoera),0) when 2 then 'prest' when 1 then 'hasi' else 'zain' end from eskaerak e where e.zerbitzua_id = id)").ReadOnly();
        Map(x => x.sortzeData).Column("data");
        HasMany(x => x.EskaeraProduktuak)
            .KeyColumn("zerbitzua_id")
            .Cascade.All()
            .Inverse();
    }
}
