namespace ErronkaApi.Modeloak
{
    public class Kategoria
    {
        public virtual int id { get; set; }
        public virtual string izena { get; set; }

        public Kategoria()
        {
        }

        public Kategoria(int id, string izena)
        {
            this.id = id;
            this.izena = izena;
        }
    }
}
