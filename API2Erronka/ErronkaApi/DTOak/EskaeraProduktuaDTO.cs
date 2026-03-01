namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskaera bateko produktu baten informazioa.
    /// </summary>
    public class EskaeraProduktuaDTO
    {
        /// <summary>
        /// Produktuaren identifikatzailea.
        /// </summary>
        public int ProduktuaId { get; set; }

        /// <summary>
        /// Produktuaren unitateko prezioa.
        /// </summary>
        public decimal PrezioUnitarioa { get; set; }

        /// <summary>
        /// Produktuaren kantitatea.
        /// </summary>
        public int Kantitatea { get; set; }
    }
}
