using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Reglas.ReglasEdadRes
{
    public class ReglaEdadNovillo : IReglaEdadRes
    {
        public bool EsEdadValida(ushort edad)
        {
            return edad >= 49;
        }
    }
}
