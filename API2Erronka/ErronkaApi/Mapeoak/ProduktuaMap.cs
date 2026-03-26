using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErronkaApi.Mapeoak
{

    public class ProduktuaMap : ClassMap<Produktua>
    {
        public ProduktuaMap()
        {
            Table("produktuak");
            Id(x => x.id).Column("id").GeneratedBy.Identity();
            Map(x => x.izena).Column("izena");
            Map(x => x.prezioa).Column("prezioa");
            References(x => x.kategoria).Column("kategoria_id");
            Map(x => x.stock_min).Column("stock_min");
            Map(x => x.stock_max).Column("stock_max");
            Map(x => x.stock_aktuala).Formula("stock_max").ReadOnly();
            Map(x => x.irudia).Column("irudia");
            Map(x => x.produktuen_motak_id).Column("produktuen_motak_id");
        }
    }
}
