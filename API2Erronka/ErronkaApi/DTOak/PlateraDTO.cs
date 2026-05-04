namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Plater baten datu publikoak eta bere osagaien stock egoera biltzen ditu.
    /// </summary>
    public class PlateraDTO
    {
        /// <summary>Plateraren identifikatzailea.</summary>
        public int Id { get; set; }
        /// <summary>Plateraren izena.</summary>
        public string Izena { get; set; }
        /// <summary>Plater mota edo atala.</summary>
        public string Mota { get; set; }
        /// <summary>Plateraren prezioa.</summary>
        public float Prezioa { get; set; }
        /// <summary>Datu-basean gordetako argazki fitxategia.</summary>
        public string Argazkia { get; set; }
        /// <summary>API bidez eskuragarri dagoen argazkiaren URL osoa.</summary>
        public string ArgazkiaUrl { get; set; }
        /// <summary>Platera prestatzeko erabiltzen diren produktuak.</summary>
        public List<PlateraOsagaiaDTO> Osagaiak { get; set; } = new();
    }

    /// <summary>
    /// Plater baten osagai den produktuaren stock informazioa.
    /// </summary>
    public class PlateraOsagaiaDTO
    {
        /// <summary>Produktu/osagaiaren identifikatzailea.</summary>
        public int Id { get; set; }
        /// <summary>Produktu/osagaiaren izena.</summary>
        public string Izena { get; set; }
        /// <summary>Uneko stocka.</summary>
        public int Stock { get; set; }
    }
}
