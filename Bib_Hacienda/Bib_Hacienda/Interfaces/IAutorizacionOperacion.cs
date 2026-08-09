using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface IAutorizacionOperacion
    {
        void AutorizarOperacion(Usuario usuario, string operacion);
    }
}
