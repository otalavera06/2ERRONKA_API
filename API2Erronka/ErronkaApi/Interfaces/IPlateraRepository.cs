using ErronkaApi.DTOak;

namespace ErronkaApi.Interfaces
{
    public interface IPlateraRepository
    {
        List<PlateraDTO> GetAll(string baseUrl);
    }
}
