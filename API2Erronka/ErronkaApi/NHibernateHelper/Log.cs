using System;
using System.IO;
public class Log
{
    public void RegistrarLog(string mensaje)
    {
        string directorioApp = AppDomain.CurrentDomain.BaseDirectory;
        string rutaArchivo = Path.Combine(directorioApp, "logTpv.txt");



        try
        {
            using (StreamWriter sw = new StreamWriter(rutaArchivo, true))
            {
                string hora = DateTime.Now.ToString("HH:mm:ss");

                sw.WriteLine($"[{hora}] Exekuzioa: {mensaje}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errorea log a idaztean: {ex.Message}");
        }
    }
}




