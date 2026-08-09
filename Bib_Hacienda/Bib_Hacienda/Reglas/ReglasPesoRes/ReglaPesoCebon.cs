using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Reglas.ReglasPesoRes
{
    public class ReglaPesoCebon : IReglaPesoRes
    {
        public ushort PesoMinimo => 290;
        public ushort PesoVenta => 420;
    }
}
