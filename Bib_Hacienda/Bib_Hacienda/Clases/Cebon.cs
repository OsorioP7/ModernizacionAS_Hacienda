using Bib_Hacienda.Reglas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases
{
    public class Cebon : Res //Hereda de Res
    {

        //Constructor
        public Cebon(string nombre, uint peso, ushort edad) : base(nombre, peso, edad)
        {
        }

        //Accesor sobrescrito para diferenciar la edad del cebon
        public override ushort Edad
        {
            get => base.Edad;
            set => base.Edad = (value > ReglaRes.edad_max_ternero && value <= ReglaRes.edad_max_cebon) ? value :
                throw new Exception("El cebon excedió la edad maxima");
        }
    }
}
