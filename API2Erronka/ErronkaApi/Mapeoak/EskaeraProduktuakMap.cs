using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;

public class EskaeraProduktuakMap : ClassMap<EskaeraProduktuak>
{
    public EskaeraProduktuakMap()
    {
        Table("eskaerak");

        Id(x => x.Id).GeneratedBy.Identity();

        References(x => x.Eskaera)
            .Column("zerbitzua_id")
            .Not.Nullable();

        References(x => x.Produktua)
            .Column("produktua_id")
            .Not.Nullable();

        Map(x => x.Kantitatea)
            .Formula("1")
            .ReadOnly();

        Map(x => x.PrezioUnitarioa)
            .Column("prezioa");

        Map(x => x.Guztira)
            .Formula("prezioa")
            .ReadOnly();
    }
}
