using ErronkaApi.Kontrollerrak;
using ErronkaApi.Modeloak;

namespace ErronkaApi.Interfaces
{
    public interface IErreserbaRepository
    {
        List<Erreserba> GetByDate(DateTime eguna, bool mota);
        Erreserba Create(ErreserbakController.ErreserbakSortuDto dto);
        bool UpdateByMahai(int mahaiaId, DateTime eguna, bool mota, ErreserbakController.ErreserbakUpdateDto dto);
        bool DeleteByMahai(int mahaiaId, DateTime eguna, bool mota);
    }
}
