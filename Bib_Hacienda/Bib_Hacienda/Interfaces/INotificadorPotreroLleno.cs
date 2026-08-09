using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface INotificadorPotreroLleno
    {
        string Notificar(ushort cantidad_reses, Potrero potrero);
    }
}
