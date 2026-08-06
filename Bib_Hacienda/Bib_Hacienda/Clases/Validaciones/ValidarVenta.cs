using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases.Validaciones
{
    // Valida objetos de tipo Venta
    public class ValidadorVenta : Validacion
    {
        public override bool ValidarVenta(Venta venta)
        {
            if (venta == null || venta.Potrero == null || venta.Res == null || venta.Monto <= 0)
            {
                return false;
            }
            return true;
        }

        public override bool ValidarRes(Res res)
        {
            throw new NotImplementedException("Use ValidadorRes");
        }

        public override bool ValidarPotrero(Potrero potrero)
        {
            throw new NotImplementedException("Use ValidadorPotrero");
        }

        public override bool ValidarVacuna(Vacuna vacuna)
        {
            throw new NotImplementedException("Use ValidadorVacuna");
        }
    }
}
