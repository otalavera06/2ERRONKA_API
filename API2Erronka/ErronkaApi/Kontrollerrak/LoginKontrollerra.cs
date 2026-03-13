using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Erabiltzaileen autentifikazioa kudeatzeko kontroladorea.
    /// </summary>
    [ApiController]
    [Route("api/Logina")]
    public class LoginKontrollera : ControllerBase
    {
        private readonly ErabiltzaileaRepository _repo;
        private readonly string logKarpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TPV_Logs");

        public LoginKontrollera(ErabiltzaileaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Erabiltzaile bat autentifikatzen du.
        /// </summary>
        /// <param name="loginDto">Saioa hasteko datuak.</param>
        /// <returns>Autentifikazioaren emaitza eta erabiltzailearen datuak.</returns>
        [HttpPost]

        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            var erabiltzailea = _repo.Login(loginDto.erabiltzailea, loginDto.pasahitza);
            if (erabiltzailea == null)
            {
                // Gorde login faltsua logean (erabiltzaile 0)
                try
                {
                    string logKarpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TPV_Logs");
                    if (!Directory.Exists(logKarpeta)) Directory.CreateDirectory(logKarpeta);
                    string eguna = DateTime.Now.ToString("yyyy-MM-dd");
                    string fitxategia = Path.Combine(logKarpeta, $"TPV_Log_{eguna}.log");
                    string ilara = $"{DateTime.Now:HH:mm:ss} | 0 | Login faltsua: {loginDto.erabiltzailea}";
                    System.IO.File.AppendAllText(fitxategia, ilara + Environment.NewLine);
                }
                catch
                {
                    // Login erroreak baztertu autentifikazioa ez oztopatzeko.
                }

                return Unauthorized(new ErantzunaDTO<object>
                {
                    Code = 401,
                    Message = "Erabiltzaile edo pasahitz okerra.",
                    Datuak = null
                });
            }

            var erabiltzaileDatuak = new Erabiltzailea
            {
                id = erabiltzailea.id,
                erabiltzailea = erabiltzailea.erabiltzailea,
                emaila = erabiltzailea.emaila,
                ezabatua = erabiltzailea.ezabatua,
                txat = erabiltzailea.txat,
                rola = new Rola { id = erabiltzailea.rola.id }
            };

            // Gorde login arrakastatsua logean
            try
            {
                if (!Directory.Exists(logKarpeta)) Directory.CreateDirectory(logKarpeta);
                string eguna = DateTime.Now.ToString("yyyy-MM-dd");
                string fitxategia = Path.Combine(logKarpeta, $"TPV_Log_{eguna}.log");
                string ilara = $"{DateTime.Now:HH:mm:ss} | {erabiltzailea.id} | Login ondo eginda";
                System.IO.File.AppendAllText(fitxategia, ilara + Environment.NewLine);
            }
            catch
            {
                // Login erroreak baztertu
            }

            return Ok(new ErantzunaDTO<Erabiltzailea>
            {
                Code = 200,
                Message = "Login ondo eginda",
                Datuak = new List<Erabiltzailea> { erabiltzaileDatuak }
            });

        }

    }
}
