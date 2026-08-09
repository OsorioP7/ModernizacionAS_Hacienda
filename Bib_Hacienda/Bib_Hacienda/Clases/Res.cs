using Bib_Hacienda.Eventos;
using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases
{
    public abstract class Res : IVendible
    {

        //Atributos
        private string nombre;
        private uint peso;
        private ushort edad;
        private readonly IReglaEdadRes regla_edad;
        private readonly IReglaPesoRes regla_peso;
        private readonly string mensaje_edad_invalida;
        private List<Vacuna> l_vacunas_aplicadas;

        internal void EventHandler() { }

        //Constructor
        public Res(
            string nombre,
            uint peso,
            ushort edad,
            IReglaEdadRes regla_edad,
            IReglaPesoRes regla_peso,
            string mensaje_edad_invalida)
        {
            this.regla_edad = regla_edad;
            this.regla_peso = regla_peso;
            this.mensaje_edad_invalida = mensaje_edad_invalida;
            this.Nombre = nombre;
            this.Peso = peso;
            this.Edad = edad;
            this.l_vacunas_aplicadas = new List<Vacuna>();
        }

        //Accesores
        public ushort Edad
        { 
            get => edad;
            set
            {
                if (!regla_edad.EsEdadValida(value))
                {
                    throw new Exception(mensaje_edad_invalida);
                }

                edad = value;
            }
        }
        public List<Vacuna> L_vacunas_aplicadas { get => l_vacunas_aplicadas; set => l_vacunas_aplicadas = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public uint Peso { get => peso; set => peso = value; }
        public IReglaPesoRes ReglaPeso { get => regla_peso; }
        public abstract string TipoRes { get; }
        public string TipoVendible => TipoRes;
    }
}
