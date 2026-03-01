namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskaera baten informazioa transferitzeko DTOa.
    /// </summary>
    public class EskaeraDTO
    {
        /// <summary>
        /// Eskaeraren identifikatzaile bakarra.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Eskaeraren izena edo deskribapena.
        /// </summary>
        public string Izena { get; set; }

        /// <summary>
        /// Eskaera egin den mahaiaren identifikatzailea.
        /// </summary>
        public int MahaiaId { get; set; }

        /// <summary>
        /// Eskaerako komensal kopurua.
        /// </summary>
        public int Komensalak { get; set; }

        /// <summary>
        /// Eskaera sortu den data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Sukaldeko egoera (adibidez: "Prestatzen", "Eginda").
        /// </summary>
        public string SukaldeaEgoera { get; set; }
    }
}
