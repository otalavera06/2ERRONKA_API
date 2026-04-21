using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;

public class MahaiaMap : ClassMap<Mahaia>
{
    public MahaiaMap()
    {
        Table("mahaiak");

        Id(x => x.id).Column("id").GeneratedBy.Identity();
        Map(x => x.izena).Column("izena");
        Map(x => x.erabiltzailea).Column("erabiltzailea");
        Map(x => x.pasahitza).Column("pasahitza");
        Map(x => x.chat_baimena).Column("chat_baimena");

    }
}
