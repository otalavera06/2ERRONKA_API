using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using System.Collections.Generic;

namespace ErronkaApi.Interfaces
{
    public interface IEskaeraRepository
    {
        Eskaera? Get(int id);
        void Save(Eskaera eskaera);
        void Update(Eskaera eskaera);
        void Delete(Eskaera eskaera);
        List<Eskaera> LortuEskaerak2();
        List<EskaeraProduktuak> LortuEskaeraProduktuak2(int eskaeraId);
        List<Eskaera> LortuEskaerakOrdaintzeko();
    }
}
