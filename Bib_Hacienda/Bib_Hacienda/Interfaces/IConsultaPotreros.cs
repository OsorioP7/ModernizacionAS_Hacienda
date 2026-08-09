using Bib_Hacienda.Clases;
using System.Collections.Generic;
namespace Bib_Hacienda.Interfaces
{
    public interface IConsultaPotreros
    {
        IReadOnlyCollection<Potrero> ObtenerTodosLosPotreros();
        Potrero ObtenerPotreroPorIdentificacion(string identificacion);
        Potrero BuscarPotrero(string nombre);
    }
}
