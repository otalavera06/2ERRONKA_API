using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using System.Collections.Generic;

namespace ErronkaApi.Interfaces{
    public interface IMahaiaRepository
    {
        Mahaia? Get(int id);
        void Update(Mahaia mahaia);
        void Delete(Mahaia mahaia);
        List<MahaiaDTO> LortuMahaiLibre();
    }
}