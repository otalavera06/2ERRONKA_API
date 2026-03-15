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
            Map(x => x.stock_aktuala).Column("stock_aktuala");
            References(x => x.kategoria).Column("kategoria_id");
        }
    }
}