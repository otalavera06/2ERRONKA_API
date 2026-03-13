using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using ErronkaApi.NHibernate;
using Microsoft.OpenApi.Validations;
using NHibernate;

namespace ErronkaApi.Interfaces
{
    public interface IProduktuaRepository
    {

        public abstract Produktua? Get(int id);
        public abstract void Update(Produktua produktua);

        public abstract void Delete(Produktua produktua);

        public abstract List<Produktua> GetAll();

        public abstract List<ProduktuaDTO> GetAllByKategoriaId(int katId);

        public abstract List<Produktua> GetByKategoria(int kategoriaId);
    }
}

