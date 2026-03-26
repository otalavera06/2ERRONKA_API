using ErronkaApi.Modeloak;

namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Produktu baten informazioa duen DTOa.
    /// </summary>
    public class ProduktuaDTO
    {
        /// <summary>
        /// Produktuaren identifikatzailea.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Produktuaren izena.
        /// </summary>
        public string izena { get; set; }

        /// <summary>
        /// Produktuaren prezioa.
        /// </summary>
        public decimal prezioa { get; set; }

        /// <summary>
        /// Produktuaren kategoriaren IDa.
        /// </summary>
        public int kategoria_id { get; set; }

        /// <summary>
        /// Uneko stock kantitatea.
        /// </summary>
        public int stock_aktuala { get; set; }


        public ProduktuaDTO(Produktua produktua)
        {
            id = produktua.id;
            izena = produktua.izena;
            prezioa = produktua.prezioa;
            kategoria_id = produktua.kategoria?.id ?? 0;
            stock_aktuala = produktua.stock_aktuala ?? 0;
        }
    }
}
