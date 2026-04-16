namespace ErronkaApi.DTOak
{
    public class PlateraDTO
    {
        public int Id { get; set; }
        public string Izena { get; set; }
        public string Mota { get; set; }
        public float Prezioa { get; set; }
        public string Argazkia { get; set; }
        public string ArgazkiaUrl { get; set; }
        public List<PlateraOsagaiaDTO> Osagaiak { get; set; } = new();
    }

    public class PlateraOsagaiaDTO
    {
        public int Id { get; set; }
        public string Izena { get; set; }
        public int Stock { get; set; }
    }
}
