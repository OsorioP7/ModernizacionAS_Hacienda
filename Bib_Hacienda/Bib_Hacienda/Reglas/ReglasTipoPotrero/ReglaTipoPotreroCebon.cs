using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;
using System;

namespace Bib_Hacienda.Reglas.ReglasTipoPotrero
{
    public class ReglaTipoPotreroCebon : IReglasTipoPotrero
    {
        private readonly IReglaEdadRes regla_edad;
        private readonly IReglaPesoRes regla_peso;

        public string TipoPotrero => "cebon";

        public ReglaTipoPotreroCebon(IReglaEdadRes regla_edad, IReglaPesoRes regla_peso)
        {
            this.regla_edad = regla_edad;
            this.regla_peso = regla_peso;
        }

        public void ValidarEdad(ushort edad, string identificacionPotrero)
        {
            if (!regla_edad.EsEdadValida(edad))
                throw new Exception($"La res no puede ser añadida al potrero {identificacionPotrero} porque su edad no corresponde al tipo de potrero");
        }

        public Res CrearRes(string nombre, uint peso, ushort edad) =>
            new Cebon(nombre, peso, edad, regla_edad, regla_peso);
    }
}
