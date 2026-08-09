using System;

namespace Bib_Hacienda.Reglas.ReglasVacunacion
{
    public class ReglaLoteVacunas
    {
        private static readonly uint maximo_por_lote = 100;

        public static void ValidarCantidad(uint cantidad)
        {
            if (cantidad > maximo_por_lote)
            {
                throw new ArgumentException("No se pueden crear más de 100 vacunas en un solo lote", nameof(cantidad));
            }
        }
    }
}
