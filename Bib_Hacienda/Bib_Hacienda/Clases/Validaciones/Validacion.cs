using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases.Validaciones
{
    //Clase base abstracta para validaciones
    public abstract class Validacion : IValidarInformacion
    {
        //Métodos abstractos que las clases hijas deben implementar
        public abstract bool ValidarRes(Res res);
        public abstract bool ValidarPotrero(Potrero potrero);
        public abstract bool ValidarVacuna(Vacuna vacuna);
        public abstract bool ValidarVenta(Venta venta);
    }
}
