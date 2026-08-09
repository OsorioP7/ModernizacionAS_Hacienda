using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases
{
    public class Bacteriana : Vacuna //Hereda de Vacuna
    {

        public override string TipoVacuna => "Bacterianas";

        public override void AcumularContador(ref byte contadorBacterianas, ref byte contadorVivas)
        {
            contadorBacterianas++;
        }

        public override void ValidarLimite(IReglasVacunacionRes reglas, byte contadorBacterianas, byte contadorVivas, string nombreRes)
        {
            reglas.ValidarLimiteBacterianas(contadorBacterianas, nombreRes);
        }

        //Atributos
        private uint periodo_aplicacion;

        //Constructor
        public Bacteriana(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion) : base(nombre, lote, fecha_vencimiento, fecha_aplicacion)
        {
            this.Periodo_aplicacion = periodo_aplicacion;
        }

        //Accesores
        public uint Periodo_aplicacion { get => periodo_aplicacion;
            set => periodo_aplicacion = value>=ReglaVacuna.periodo_min_bac_aplic && value<=ReglaVacuna.periodo_max_bac_aplic? value :
                throw new Exception($"el valor del periodo de aplicacion debe estar entre {ReglaVacuna.periodo_min_bac_aplic} y {ReglaVacuna.periodo_max_bac_aplic} semanas"); }
    }
}
