namespace ErronkaApi.DTOak
{
    /// <summary>
    /// APIren erantzun estandarra definitzen duen DTOa.
    /// </summary>
    /// <typeparam name="T">Datuen mota generikoa.</typeparam>
    public class ErantzunaDTO<T>
    {
        /// <summary>
        /// HTTP egoera kodea edo aplikazioaren barneko kodea.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Erantzunaren mezua (adibidez, errorea edo arrakasta mezua).
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Itzulitako datuen zerrenda.
        /// </summary>
        public List<T>? Datuak { get; set; }
    }
}
