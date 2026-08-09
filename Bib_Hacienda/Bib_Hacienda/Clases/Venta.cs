using System;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Clases
{
    public class Venta
    {
        public Venta(
            DateTime fecha,
            IVendible vendible,
            uint cantidad,
            uint precioUnitarioVenta,
            string referenciaOrigen)
            : this(
                fecha,
                vendible == null ? null : vendible.Nombre,
                vendible == null ? null : vendible.TipoVendible,
                cantidad,
                precioUnitarioVenta,
                referenciaOrigen)
        {
            if (vendible == null)
            {
                throw new ArgumentNullException(nameof(vendible));
            }
        }

        public Venta(
            DateTime fecha,
            string nombreVendible,
            string tipoVendible,
            uint cantidad,
            uint precioUnitarioVenta,
            string referenciaOrigen)
        {
            if (string.IsNullOrWhiteSpace(nombreVendible))
            {
                throw new ArgumentException("El nombre del vendible es requerido", nameof(nombreVendible));
            }

            if (string.IsNullOrWhiteSpace(tipoVendible))
            {
                throw new ArgumentException("El tipo del vendible es requerido", nameof(tipoVendible));
            }

            Fecha = fecha;
            NombreVendible = nombreVendible;
            TipoVendible = tipoVendible;
            Cantidad = cantidad;
            PrecioUnitarioVenta = precioUnitarioVenta;
            Monto = checked(cantidad * precioUnitarioVenta);
            ReferenciaOrigen = referenciaOrigen ?? string.Empty;
        }

        public DateTime Fecha { get; }
        public uint Monto { get; }
        public string NombreVendible { get; }
        public string TipoVendible { get; }
        public uint Cantidad { get; }
        public uint PrecioUnitarioVenta { get; }
        public string ReferenciaOrigen { get; }
    }
}
