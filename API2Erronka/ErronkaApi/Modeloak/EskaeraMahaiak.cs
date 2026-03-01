using ErronkaApi.Modeloak;

namespace Api.Modeloak
{
    public class EskaeraMahaiak
    {
        public virtual int Id { get; set; }
        public virtual Eskaera Eskaera { get; set; }
        public virtual Mahaia Mahaia { get; set; }
    }
}