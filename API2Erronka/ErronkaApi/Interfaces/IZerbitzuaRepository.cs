using System.Collections.Generic;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.DTOak;

namespace ErronkaApi.Interfaces
{
    public interface IZerbitzuaRepository
    {
        int Create(ZerbitzuaController.ZerbitzuaSortuDto dto);
        List<ZerbitzuaMahaiDTO> GetByMahai(int mahaiaId);
        bool Ordaindu(int id);
    }
}
