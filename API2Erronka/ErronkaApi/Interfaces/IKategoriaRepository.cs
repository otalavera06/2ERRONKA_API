using ErronkaApi.Modeloak;
using System.Collections.Generic;

namespace ErronkaApi.Interfaces
{
    public interface IKategoriaRepository
    {
        public abstract IList<Kategoria> GetAll();
    }
}