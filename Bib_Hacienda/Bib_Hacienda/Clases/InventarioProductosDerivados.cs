using System;
using System.Collections.Generic;
using System.Linq;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Clases
{
    public class InventarioProductosDerivados : IInventarioProductosDerivados
    {
        private readonly List<ProductoDerivado> productos;

        public InventarioProductosDerivados(IEnumerable<ProductoDerivado> productos)
        {
            this.productos = productos?.ToList() ?? new List<ProductoDerivado>();
        }

        public IReadOnlyCollection<ProductoDerivado> Productos => productos.AsReadOnly();

        public ProductoDerivado BuscarPorCodigo(string codigo)
        {
            return productos.FirstOrDefault(p => string.Equals(p.Codigo, codigo, StringComparison.OrdinalIgnoreCase));
        }

        public void Descontar(string codigo, uint cantidad)
        {
            var producto = BuscarPorCodigo(codigo);
            if (producto == null)
            {
                throw new KeyNotFoundException($"No se encontró el producto con código '{codigo}'");
            }

            producto.Descontar(cantidad);
            if (producto.Existencia == 0)
            {
                productos.Remove(producto);
            }
        }
    }
}
