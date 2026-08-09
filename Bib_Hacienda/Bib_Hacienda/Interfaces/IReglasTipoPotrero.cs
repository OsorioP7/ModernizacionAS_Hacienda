using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface IReglasTipoPotrero
    {
        string TipoPotrero { get; }

        void ValidarEdad(ushort edad, string identificacionPotrero);
        Res CrearRes(string nombre, uint peso, ushort edad);
    }
}
