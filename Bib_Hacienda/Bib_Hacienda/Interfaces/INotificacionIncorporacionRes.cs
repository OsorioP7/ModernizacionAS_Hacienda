using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface INotificacionIncorporacionRes
    {
        string ObtenerMensajes(
            Potrero potrero,
            Res res,
            ushort cantidad_reses);
    }
}
