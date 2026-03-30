using System;
using System.Collections.Generic;

namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Zerbitzu baten barruan dagoen eskaera bakoitzaren informazioa.
    /// </summary>
    public class ZerbitzuaEskaeraDTO
    {
        /// <summary>
        /// Eskaeraren identifikatzailea.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Produktuaren identifikatzailea.
        /// </summary>
        public int ProduktuaId { get; set; }
        /// <summary>
        /// Produktuaren izena.
        /// </summary>
        public string Izena { get; set; }
        /// <summary>
        /// Produktuaren irudiaren helbidea (adibidez: /irudiak/fitxategia.jpg).
        /// </summary>
        public string? Irudia { get; set; }
        /// <summary>
        /// Eskaeraren data.
        /// </summary>
        public DateTime Data { get; set; }
        /// <summary>
        /// Produktuaren prezioa (unitate edo guztira).
        /// </summary>
        public decimal Prezioa { get; set; }
        /// <summary>
        /// Eskaeraren egoera zenbakiz adierazita.
        /// </summary>
        public int Egoera { get; set; }
    }

    /// <summary>
    /// Mahai bateko zerbitzuaren laburpena eta bere eskaeren zerrenda.
    /// </summary>
    public class ZerbitzuaMahaiDTO
    {
        /// <summary>
        /// Zerbitzuaren identifikatzailea.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Zerbitzuaren prezio totala.
        /// </summary>
        public decimal PrezioTotala { get; set; }
        /// <summary>
        /// Zerbitzuaren data.
        /// </summary>
        public DateTime Data { get; set; }
        /// <summary>
        /// Erreserbaren identifikatzailea, baldin badago.
        /// </summary>
        public int? ErreserbaId { get; set; }
        /// <summary>
        /// Mahaiaren identifikatzailea.
        /// </summary>
        public int? MahaiakId { get; set; }
        /// <summary>
        /// Zerbitzuan dauden eskaeren zerrenda.
        /// </summary>
        public List<ZerbitzuaEskaeraDTO> Eskaerak { get; set; } = new List<ZerbitzuaEskaeraDTO>();
    }
}
