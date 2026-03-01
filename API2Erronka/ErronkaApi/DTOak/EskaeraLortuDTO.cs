namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskaera baten xehetasunak lortzeko DTOa.
    /// </summary>
    public class EskaeraLortuDTO
    {
        /// <summary>
        /// Produktuaren identifikatzailea.
        /// </summary>
        public int ProduktuaId { get; set; }

        /// <summary>
        /// Produktuaren izena.
        /// </summary>
        public string ProduktuaIzena { get; set; }

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
