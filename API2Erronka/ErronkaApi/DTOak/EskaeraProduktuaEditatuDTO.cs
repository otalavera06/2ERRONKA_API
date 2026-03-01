namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskaera bateko produktu bat editatzeko DTOa.
    /// </summary>
    public class EskaeraProduktuaEditatuDTO
    {
        /// <summary>
        /// Produktuaren identifikatzailea.
        /// </summary>
        public int ProduktuaId { get; set; }

        /// <summary>
        /// Produktuaren kantitate berria.
        /// </summary>
        public int Kantitatea { get; set; }
    }
}
