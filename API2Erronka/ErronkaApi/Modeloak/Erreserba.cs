namespace ErronkaApi.Modeloak
{
    public class Erreserba
    {
        public virtual int Id { get; set; }
        public virtual System.DateTime Data { get; set; }
        public virtual bool Mota { get; set; }
        public virtual int? ErabiltzaileakId { get; set; }
        public virtual int MahaiakId { get; set; }
    }
}
