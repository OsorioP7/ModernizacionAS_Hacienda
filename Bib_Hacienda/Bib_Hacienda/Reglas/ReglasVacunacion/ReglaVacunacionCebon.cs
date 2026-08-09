using System;

namespace Bib_Hacienda.Reglas.ReglasVacunacion
{
    public static class ReglaVacunacionCebon
    {
        private static readonly byte max_bacterianas = 1;
        private static readonly byte max_vivas = 4;

        public static void ValidarLimiteBacterianas(ushort contadorBacterianas, string nombreRes)
        {
            if (contadorBacterianas >= max_bacterianas)
                throw new Exception($"No se puede aplicar más vacunas bacterianas a la res '{nombreRes}'. Ya tiene las {max_bacterianas} permitidas.");
        }

        public static void ValidarLimiteVivas(ushort contadorVivas, string nombreRes)
        {
            if (contadorVivas >= max_vivas)
                throw new Exception($"No se puede aplicar más vacunas vivas a la res '{nombreRes}'. Ya tiene las {max_vivas} permitidas.");
        }

        public static bool EsquemaCompleto(ushort contadorBacterianas, ushort contadorVivas)
        {
            return contadorBacterianas >= max_bacterianas && contadorVivas >= max_vivas;
        }
    }
}
