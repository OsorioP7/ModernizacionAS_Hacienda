using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas;
using Bib_Hacienda.Reglas.ReglasVacunacion;

namespace Bib_Hacienda.Clases
{
    public class Cebon : Res, IReglasVacunacionRes //Hereda de Res
    {
        //Constructor
        public Cebon(string nombre, uint peso, ushort edad, IReglaEdadRes regla_edad, IReglaPesoRes regla_peso)
            : base(nombre, peso, edad, regla_edad, regla_peso, "El cebon excedió la edad maxima")
        {
        }

        public override string TipoRes => "Cebones";

        public void ValidarLimiteBacterianas(ushort contadorBacterianas, string nombreRes)
            => ReglaVacunacionCebon.ValidarLimiteBacterianas(contadorBacterianas, nombreRes);

        public void ValidarLimiteVivas(ushort contadorVivas, string nombreRes)
            => ReglaVacunacionCebon.ValidarLimiteVivas(contadorVivas, nombreRes);

        public bool EsquemaCompleto(ushort contadorBacterianas, ushort contadorVivas)
            => ReglaVacunacionCebon.EsquemaCompleto(contadorBacterianas, contadorVivas);
    }
}
