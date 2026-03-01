namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Mahai baten informazioa duen DTOa.
    /// </summary>
    public class MahaiaDTO
    {
        /// <summary>
        /// Mahaiaren identifikatzailea.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Mahaiaren zenbakia.
        /// </summary>
        public int Zenbakia { get; set; }

        /// <summary>
        /// Mahaiaren kapazitatea (pertsona kopurua).
        /// </summary>
        public int kapazitatea { get; set; }
    }
}
