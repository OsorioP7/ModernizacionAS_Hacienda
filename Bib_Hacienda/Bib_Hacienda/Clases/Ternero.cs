using Bib_Hacienda.Reglas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases
{
    public class Ternero : Res //Hereda de Res
    {

        // Constructor
        public Ternero(string nombre, uint peso, ushort edad) : base(nombre, peso, edad)
        {
        }

        //Accesor sobrescrito para diferenciar la edad del ternero
        public override ushort Edad
        {
            get => base.Edad;
            set => base.Edad = value <= ReglaRes.edad_max_ternero ? value : 
                throw new Exception("El ternero excedió la edad maxima");
        }
    }
}
