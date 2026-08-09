using System.Collections.Generic;
using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface IEstadoVentas
    {
        IReadOnlyCollection<Venta> Ventas { get; }
    }
}
