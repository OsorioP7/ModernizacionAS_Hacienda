using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Reglas.ReglasPesoRes
{
    public class ReglaPesoTernero : IReglaPesoRes
    {
        public ushort PesoMinimo => 150;
        public ushort PesoVenta => 250;
    }
}
