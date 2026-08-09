using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface IReglasVacunacionRes
    {
        void ValidarLimiteBacterianas(
            ushort contadorBacterianas,
            string nombreRes);

        void ValidarLimiteVivas(
            ushort contadorVivas,
            string nombreRes);

        bool EsquemaCompleto(
            ushort contadorBacterianas,
            ushort contadorVivas);
    }
}
