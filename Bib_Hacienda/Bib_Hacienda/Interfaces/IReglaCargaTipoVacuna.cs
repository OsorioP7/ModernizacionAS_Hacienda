using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface IReglaCargaTipoVacuna
    {
        string TipoVacuna { get; }
        Vacuna Cargar(string[] partes);
    }
}
