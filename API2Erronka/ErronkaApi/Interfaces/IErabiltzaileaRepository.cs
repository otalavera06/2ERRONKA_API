using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;

namespace ErronkaApi.Interfaces
{
    public interface IErabiltzaileaRepository
    {
        List<LangileaDTO> GetAll();
        Erabiltzailea? Login(string erabiltzailea, string pasahitza);
        Erabiltzailea? LortuErabiltzailea(int id);
    }
}
