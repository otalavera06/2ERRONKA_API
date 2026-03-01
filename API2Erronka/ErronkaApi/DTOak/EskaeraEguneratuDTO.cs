namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskaera bat eguneratzeko erabiltzen den DTOa.
    /// </summary>
    public class EskaeraEguneratuDTO
    {
        /// <summary>
        /// Komensal kopuru berria.
        /// </summary>
        public int Komensalak { get; set; }

        /// <summary>
        /// Eguneratuko diren produktuen zerrenda.
        /// </summary>
        public List<EskaeraProduktuaEditatuDTO> Produktuak { get; set; }
    }
}
