using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas;
using Bib_Hacienda.Reglas.ReglasVacunacion;

namespace Bib_Hacienda.Clases
{
    public class Novillo : Res, IReglasVacunacionRes //Hereda de Res
    {
        //Constructor
        public Novillo(string nombre, uint peso, ushort edad, IReglaEdadRes regla_edad, IReglaPesoRes regla_peso)
            : base(nombre, peso, edad, regla_edad, regla_peso, "El ternero excedió la edad maxima")
        {
        }

        public override string TipoRes => "Novillos";

        public void ValidarLimiteBacterianas(ushort contadorBacterianas, string nombreRes)
            => ReglaVacunacionNovillo.ValidarLimiteBacterianas(contadorBacterianas, nombreRes);

        public void ValidarLimiteVivas(ushort contadorVivas, string nombreRes)
            => ReglaVacunacionNovillo.ValidarLimiteVivas(contadorVivas, nombreRes);

        public bool EsquemaCompleto(ushort contadorBacterianas, ushort contadorVivas)
            => ReglaVacunacionNovillo.EsquemaCompleto(contadorBacterianas, contadorVivas);
    }
}
