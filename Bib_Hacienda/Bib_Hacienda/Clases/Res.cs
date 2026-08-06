using Bib_Hacienda.Eventos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases
{
    public abstract class Res
    {

        //Atributos
        private string nombre;
        private uint peso;
        private ushort edad;
        private List<Vacuna> l_vacunas_aplicadas;

        internal void EventHandler() { }

        //Constructor
        public Res(string nombre, uint peso, ushort edad)
        {
            this.Nombre = nombre;
            this.Peso = peso;
            this.Edad = edad;
            this.l_vacunas_aplicadas = new List<Vacuna>();
        }

        //Accesores
        public virtual ushort Edad 
        { 
            get => edad;
            set => edad = value; 
        }
        public List<Vacuna> L_vacunas_aplicadas { get => l_vacunas_aplicadas; set => l_vacunas_aplicadas = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public uint Peso { get => peso; set => peso = value; }
    }
}
