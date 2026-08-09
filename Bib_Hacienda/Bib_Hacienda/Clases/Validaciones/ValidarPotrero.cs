using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Clases.Validaciones
{
    //Valida objetos de tipo Potrero
    public class ValidadorPotrero : IValidador<Potrero>
    {
        public virtual bool ValidarPotrero(Potrero potrero)
        {
            if (potrero == null || string.IsNullOrWhiteSpace(potrero.Identificacion))
            {
                return false;
            }
            return true;
        }

        public virtual bool Validar(Potrero elemento)
            => ValidarPotrero(elemento);
    }
}
