using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Reglas.ReglasEdadRes
{
    public class ReglaEdadCebon : IReglaEdadRes
    {
        public bool EsEdadValida(ushort edad)
        {
            return edad >= 13 && edad <= 48;
        }
    }
}
