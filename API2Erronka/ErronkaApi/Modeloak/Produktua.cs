namespace ErronkaApi.Modeloak
{
    public class Produktua
    {
        public virtual int id { get; set; }
        public virtual string izena { get; set; }
        public virtual Kategoria? kategoria { get; set; }
        public virtual decimal prezioa { get; set; }
        public virtual int? stock_aktuala { get; set; }
        public virtual int? stock_min { get; set; }
        public virtual int? stock_max { get; set; }
        public virtual string? irudia { get; set; }
        public virtual int? produktuen_motak_id { get; set; }

        public Produktua() { }

        public Produktua(int id, string izena, Kategoria kategoria, decimal prezioa, int stock_aktuala)
        {
            this.id = id;
            this.izena = izena;
            this.kategoria = kategoria;
            this.prezioa = prezioa;
            this.stock_aktuala = stock_aktuala;
        }

    }


}
