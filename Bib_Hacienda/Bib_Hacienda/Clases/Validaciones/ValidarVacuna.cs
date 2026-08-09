using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Clases.Validaciones
{
    //Valida objetos de tipo Vacuna
    public class ValidadorVacuna : IValidador<Vacuna>
    {
        public virtual bool ValidarVacuna(Vacuna vacuna)
        {
            if (vacuna == null || string.IsNullOrWhiteSpace(vacuna.Nombre) || string.IsNullOrWhiteSpace(vacuna.Lote))
            {
                return false;
            }
            return true;
        }

        public virtual bool Validar(Vacuna elemento)
            => ValidarVacuna(elemento);
    }
}
