using ErronkaApi.DTOak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Modeloak
{
    /// <summary>
    /// Logak kudeatzeko kontroladorea.
    /// Aplikazioko ekintzen erregistroa gordetzeko balio du.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class Log : ControllerBase
    {
        private readonly string logKarpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"TPV_Logs");

        public Log()
        {
            if (!Directory.Exists(logKarpeta))
                Directory.CreateDirectory(logKarpeta);
        }

        /// <summary>
        /// Log berri bat gordetzen du.
        /// </summary>
        /// <param name="log">Log datuak.</param>
        /// <returns>Emaitza mezua.</returns>
        [HttpPost("gorde")]
        public IActionResult GordeLog([FromBody] LogDTO log)
        {
            try
            {
                string eguna = DateTime.Now.ToString("yyyy-MM-dd");
                string fitxategia = Path.Combine(logKarpeta, $"TPV_Log_{eguna}.log");
                string ilara = $"{DateTime.Now:HH:mm:ss} | {log.Erabiltzailea} | {log.Ekintza}";

                System.IO.File.AppendAllText(fitxategia, ilara + Environment.NewLine);

                return Ok(new { Mezua = "Loga gorde da" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mezua = "Errorea loga gordetzean", Error = ex.Message });
            }
        }
    }
}
