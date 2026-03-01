namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Log bat sortzeko DTOa.
    /// </summary>
    public class LogDTO
    {
        /// <summary>
        /// Erabiltzailearen identifikatzailea.
        /// </summary>
        public int Erabiltzailea { get; set; }

        /// <summary>
        /// Egindako ekintzaren deskribapena.
        /// </summary>
        public string Ekintza { get; set; }
    }
}
