namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Saioa hasteko kredentzialak dituen DTOa.
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// Erabiltzaile izena.
        /// </summary>
        public string erabiltzailea { get; set; }

        /// <summary>
        /// Pasahitza.
        /// </summary>
        public string pasahitza { get; set; }

        /// <summary>
        /// Txata gaituta dagoen ala ez adierazten du.
        /// </summary>
        public Boolean txat { get; set; }
    }
}
