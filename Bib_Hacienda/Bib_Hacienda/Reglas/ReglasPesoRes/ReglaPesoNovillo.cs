using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Reglas.ReglasPesoRes
{
    public class ReglaPesoNovillo : IReglaPesoRes
    {
        public ushort PesoMinimo => 400;
        public ushort PesoVenta => 550;
    }
}
