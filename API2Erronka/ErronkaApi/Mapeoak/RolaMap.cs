using ErronkaApi.Modeloak;

using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErronkaApi.Mapeoak
{
    internal class RolaMap : ClassMap<Rola>
    {
        public RolaMap()
        {
            Table("rolak");
            Id(x => x.id).Column("ID").GeneratedBy.Identity();
            Map(x => x.izena).Column("izena").Length(45);
        }
    }
}
