    namespace ErronkaApi.Modeloak
{
    public class Mahaia
        {
            public virtual int id { get; set; }

            public virtual string? izena { get; set; }
            public virtual string? erabiltzailea { get; set; }
            public virtual string? pasahitza { get; set; }
            public virtual string? chat_baimena { get; set; }

            public virtual IList<EskaeraMahaiak> EskaeraMahaiak { get; set; } = new List<EskaeraMahaiak>();
        }
    }
