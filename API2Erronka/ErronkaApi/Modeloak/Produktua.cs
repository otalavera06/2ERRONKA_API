namespace ErronkaApi.Modeloak
{
    public class Produktua
    {
        public virtual int id { get; set; }
        public virtual string izena { get; set; }
        public virtual Kategoria kategoria { get; set; }
        public virtual decimal prezioa { get; set; }
        public virtual int stock_aktuala { get; set; }
    }
}
