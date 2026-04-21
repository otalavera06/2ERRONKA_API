using ErronkaApi.Modeloak;

namespace ErronkaApi.Interfaces
{
    public interface IErabiltzaileaRepository
    {
        Erabiltzailea? Login(string erabiltzailea, string pasahitza);
        Erabiltzailea? LortuErabiltzailea(int id);
    }
}
