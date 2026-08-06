using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases.Validaciones
{
    //Valida objetos de tipo Potrero
    public class ValidadorPotrero : Validacion
    {
        public override bool ValidarPotrero(Potrero potrero)
        {
            if (potrero == null || string.IsNullOrWhiteSpace(potrero.Identificacion))
            {
                return false;
            }
            return true;
        }

        public override bool ValidarRes(Res res)
        {
            throw new NotImplementedException("Use ValidadorRes");
        }

        public override bool ValidarVacuna(Vacuna vacuna)
        {
            throw new NotImplementedException("Use ValidadorVacuna");
        }

        public override bool ValidarVenta(Venta venta)
        {
            throw new NotImplementedException("Use ValidadorVenta");
        }
    }
}
