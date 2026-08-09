using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Clases.Validaciones
{
    //Valida objetos de tipo Res
    public class ValidadorRes : IValidador<Res>
    {
        public virtual bool ValidarRes(Res res)
        {
            if (res == null || string.IsNullOrWhiteSpace(res.Nombre) || res.Peso <= 0 || res.Edad <= 0)
            {
                return false;
            }
            return true;
        }

        public virtual bool Validar(Res elemento)
            => ValidarRes(elemento);
    }
}
