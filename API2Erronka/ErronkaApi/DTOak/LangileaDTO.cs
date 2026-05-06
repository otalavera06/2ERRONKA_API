namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Langile baten datuak eta aplikazioko baimenak garraiatzeko DTOa.
    /// </summary>
    public class LangileaDTO
    {
        /// <summary>
        /// Langilearen identifikatzailea.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Langilearen izena.
        /// </summary>
        public string? Izena { get; set; }
        /// <summary>
        /// Langilearen abizena.
        /// </summary>
        public string? Abizena { get; set; }
        /// <summary>
        /// Saioa hasteko erabiltzaile izena.
        /// </summary>
        public string? Erabiltzailea { get; set; }
        /// <summary>
        /// Langilearen emaila.
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Langilearen telefono zenbakia.
        /// </summary>
        public string? Telefonoa { get; set; }
        /// <summary>
        /// Langileak aplikazioan sartzeko baimena duen adierazten du.
        /// </summary>
        public bool Baimena { get; set; }
        /// <summary>
        /// Langileari lotutako mahaiaren identifikatzailea, baldin badago.
        /// </summary>
        public int? MahaiakId { get; set; }
        /// <summary>
        /// Langileak txata erabiltzeko baimena duen adierazten du.
        /// </summary>
        public bool ChatBaimena { get; set; }
    }
}
