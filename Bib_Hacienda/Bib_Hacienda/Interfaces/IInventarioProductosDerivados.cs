using System.Collections.Generic;
using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface IInventarioProductosDerivados
    {
        IReadOnlyCollection<ProductoDerivado> Productos { get; }

        ProductoDerivado BuscarPorCodigo(string codigo);

        void Descontar(string codigo, uint cantidad);
    }
}
