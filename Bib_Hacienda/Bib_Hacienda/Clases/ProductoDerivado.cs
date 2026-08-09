using System;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Clases
{
    public abstract class ProductoDerivado : IVendible
    {
        protected ProductoDerivado(
            string codigo,
            string nombre,
            uint existencia,
            uint precioUnitario)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentException("El código del producto es requerido", nameof(codigo));
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del producto es requerido", nameof(nombre));
            }

            Codigo = codigo;
            Nombre = nombre;
            Existencia = existencia;
            PrecioUnitario = precioUnitario;
        }

        public string Codigo { get; }
        public string Nombre { get; }
        public uint Existencia { get; private set; }
        public uint PrecioUnitario { get; }
        public abstract string TipoProducto { get; }
        public abstract string UnidadMedida { get; }
        public string TipoVendible => TipoProducto;

        public void Descontar(uint cantidad)
        {
            if (cantidad == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad debe ser mayor que cero");
            }

            if (cantidad > Existencia)
            {
                throw new InvalidOperationException("La cantidad solicitada supera la existencia disponible");
            }

            Existencia -= cantidad;
        }
    }
}
