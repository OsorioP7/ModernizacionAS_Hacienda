using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases.Validaciones
{
    //Valida objetos de tipo Res
    public class ValidadorRes : Validacion
    {
        public override bool ValidarRes(Res res)
        {
            if (res == null || string.IsNullOrWhiteSpace(res.Nombre) || res.Peso <= 0 || res.Edad <= 0)
            {
                return false;
            }
            return true;
        }

        public override bool ValidarPotrero(Potrero potrero)
        {
            throw new NotImplementedException("Use ValidadorPotrero");
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
