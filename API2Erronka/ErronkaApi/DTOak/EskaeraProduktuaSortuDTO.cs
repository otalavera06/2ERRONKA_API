namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskaera berri batean produktu bat gehitzeko DTOa.
    /// </summary>
    public class EskaeraProduktuaSortuDTO
    {
        /// <summary>
        /// Produktuaren identifikatzailea.
        /// </summary>
        public int ProduktuaId { get; set; }

        /// <summary>
        /// Produktuaren kantitatea.
        /// </summary>
        public int Kantitatea { get; set; }

        /// <summary>
        /// Produktuaren unitateko prezioa.
        /// </summary>
        public decimal PrezioUnitarioa { get; set; }
    }
}
