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
            Map(x => x.erabiltzailea).Formula("email").ReadOnly();
            Map(x => x.emaila).Column("email").Length(100);
            Map(x => x.pasahitza).Column("pasahitza").Length(255);
            References(x => x.rola).Formula("(1)").ReadOnly().LazyLoad();
            Map(x => x.ezabatua).Formula("(0)").ReadOnly();
            Map(x => x.txat).Formula("(0)").ReadOnly();
        }
    }
}
