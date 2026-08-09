using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface INotificadorPotreroMitad
    {
        string Notificar(ushort cantidad_reses, Potrero potrero);
    }
}
