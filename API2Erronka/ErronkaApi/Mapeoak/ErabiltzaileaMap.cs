using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErronkaApi.Mapeoak
{
    internal class ErabiltzaileaMap : ClassMap<Erabiltzailea>
    {
        public ErabiltzaileaMap()
        {
            Table("erabiltzaileak");
            Id(x => x.id).Column("id").GeneratedBy.Identity();
            Map(x => x.erabiltzailea).Column("erabiltzailea").Length(45);
            Map(x => x.emaila).Column("email").Length(100);
            Map(x => x.pasahitza).Column("pasahitza").Length(45);
            References(x => x.rola).Column("rola_id").Not.Nullable().Not.LazyLoad();
            Map(x => x.ezabatua).Column("ezabatua");
            Map(x => x.txat).Column("chat").Not.Nullable();
        }
    }
}
