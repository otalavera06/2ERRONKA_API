    namespace ErronkaApi.Modeloak
{
    public class Mahaia
        {
            public virtual int id { get; set; }
            public virtual int zenbakia { get; set; }

            public virtual int kapazitatea { get; set; }
            public virtual string egoera { get; set; }

            public virtual IList<EskaeraMahaiak> EskaeraMahaiak { get; set; } = new List<EskaeraMahaiak>();
        }
    }
