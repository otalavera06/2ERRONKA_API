namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Plater baten informazio osoa eta bere osagaiak garraiatzeko DTOa.
    /// </summary>
    public class PlateraDTO
    {
        /// <summary>
        /// Plateraren identifikatzailea.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Plateraren izena.
        /// </summary>
        public string Izena { get; set; }
        /// <summary>
        /// Plateraren mota edo taldea.
        /// </summary>
        public string Mota { get; set; }
        /// <summary>
        /// Plateraren prezioa.
        /// </summary>
        public float Prezioa { get; set; }
        /// <summary>
        /// Datu-basean gordetako argazkiaren izena.
        /// </summary>
        public string Argazkia { get; set; }
        /// <summary>
        /// Bezeroak irudia deskargatzeko URL osoa.
        /// </summary>
        public string ArgazkiaUrl { get; set; }
        /// <summary>
        /// Platera prestatzeko erabiltzen diren osagaien zerrenda.
        /// </summary>
        public List<PlateraOsagaiaDTO> Osagaiak { get; set; } = new();
    }

    /// <summary>
    /// Plater bati lotutako osagai baten datuak.
    /// </summary>
    public class PlateraOsagaiaDTO
    {
        /// <summary>
        /// Osagaiaren identifikatzailea.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Osagaiaren izena.
        /// </summary>
        public string Izena { get; set; }
        /// <summary>
        /// Osagaiaren uneko stocka.
        /// </summary>
        public int Stock { get; set; }
    }
}
