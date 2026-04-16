using FluentNHibernate.Mapping;
using ErronkaApi.Modeloak;
using System;

namespace ErronkaApi.Mapeoak
{
    public class KategoriaMap : ClassMap<Kategoria>

    {
        public KategoriaMap()
        {
            Table("produktuen_motak");
            Id(x => x.id).Column("id").GeneratedBy.Identity();
            Map(x => x.izena).Column("izena").Length(100);
        }
    }
}
