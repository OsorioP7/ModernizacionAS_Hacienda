using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases.Validaciones
{
    //Valida objetos de tipo Vacuna
    public class ValidadorVacuna : Validacion
    {
        public override bool ValidarVacuna(Vacuna vacuna)
        {
            if (vacuna == null || string.IsNullOrWhiteSpace(vacuna.Nombre) || string.IsNullOrWhiteSpace(vacuna.Lote))
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

        public override bool ValidarVenta(Venta venta)
        {
            throw new NotImplementedException("Use ValidadorVenta");
        }
    }
}
