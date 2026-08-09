using System.Collections.Generic;
using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface IActualizacionHistorialVacunacion
    {
        string GuardarVacunasAplicadas(List<Potrero> potreros);
    }
}
