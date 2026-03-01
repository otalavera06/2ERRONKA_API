using FluentNHibernate.Mapping;
using ErronkaApi.Modeloak;

namespace ErronkaApi.Mapeoak
{
    public class KategoriaMap : ClassMap<Kategoria>

    {
        public KategoriaMap()
        {
            Table("kategoriak");
            Id(x => x.id).Column("ID").GeneratedBy.Identity();
            Map(x => x.izena).Column("izena").Length(100);
        }
    }
}
