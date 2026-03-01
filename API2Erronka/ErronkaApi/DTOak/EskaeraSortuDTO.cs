namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskaera berri bat sortzeko DTOa.
    /// </summary>
    public class EskaeraSortuDTO
    {
        /// <summary>
        /// Eskaera sortzen duen erabiltzailearen IDa.
        /// </summary>
        public int ErabiltzaileId { get; set; }

        /// <summary>
        /// Eskaera zein mahaitan egiten den.
        /// </summary>
        public int MahaiaId { get; set; }

        /// <summary>
        /// Komensal kopurua.
        /// </summary>
        public int Komensalak { get; set; }

        /// <summary>
        /// Eskaeran sartutako produktuen zerrenda.
        /// </summary>
        public List<EskaeraProduktuaSortuDTO> Produktuak { get; set; }
    }
}
