using ErronkaApi.Modeloak;
using System.Collections.Generic;

namespace ErronkaApi.Interfaces
{
    public interface IEskaeraProduktuakRepository
    {
        List<EskaeraProduktuak> GetByEskaeraId(int eskaeraId);
        void Update(EskaeraProduktuak ep);
        void Delete(EskaeraProduktuak ep);
    }
}
