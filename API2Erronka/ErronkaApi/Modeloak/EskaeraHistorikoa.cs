namespace ErronkaApi.Modeloak
{
    public class EskaeraHistorikoa
    {
        public virtual int Id { get; set; }
        public virtual int EskaeraId { get; set; }
        public virtual DateTime ItxieraData { get; set; }
    }
}