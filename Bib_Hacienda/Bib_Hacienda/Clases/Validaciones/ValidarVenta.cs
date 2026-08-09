using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Clases.Validaciones
{
    // Valida objetos de tipo Venta
    public class ValidadorVenta : IValidador<Venta>
    {
        public virtual bool ValidarVenta(Venta venta)
        {
            if (venta == null || string.IsNullOrWhiteSpace(venta.NombreVendible) ||
                string.IsNullOrWhiteSpace(venta.TipoVendible) || venta.Monto <= 0)
            {
                return false;
            }
            return true;
        }

        public virtual bool Validar(Venta elemento)
            => ValidarVenta(elemento);
    }
}
