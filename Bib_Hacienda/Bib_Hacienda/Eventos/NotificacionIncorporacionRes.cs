using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Eventos
{
    public class NotificacionIncorporacionRes : INotificacionIncorporacionRes
    {
        private readonly INotificadorPotreroMitad notificador_potrero_mitad;
        private readonly INotificadorPotreroLleno notificador_potrero_lleno;
        private readonly INotificadorPesoMin notificador_peso_min;
        private readonly INotificadorPesoVenta notificador_peso_venta;

        public NotificacionIncorporacionRes(
            INotificadorPotreroMitad notificador_potrero_mitad,
            INotificadorPotreroLleno notificador_potrero_lleno,
            INotificadorPesoMin notificador_peso_min,
            INotificadorPesoVenta notificador_peso_venta)
        {
            this.notificador_potrero_mitad = notificador_potrero_mitad;
            this.notificador_potrero_lleno = notificador_potrero_lleno;
            this.notificador_peso_min = notificador_peso_min;
            this.notificador_peso_venta = notificador_peso_venta;
        }

        public string ObtenerMensajes(
            Potrero potrero,
            Res res,
            ushort cantidad_reses)
        {
            string mensajes_eventos = "";

            string mensaje = notificador_potrero_mitad.Notificar(cantidad_reses, potrero);
            if (!string.IsNullOrEmpty(mensaje))
                mensajes_eventos += mensaje + "\n";

            mensaje = notificador_potrero_lleno.Notificar(cantidad_reses, potrero);
            if (!string.IsNullOrEmpty(mensaje))
                mensajes_eventos += mensaje + "\n";

            mensaje = notificador_peso_min.Notificar(res);
            if (!string.IsNullOrEmpty(mensaje))
                mensajes_eventos += mensaje + "\n";

            mensaje = notificador_peso_venta.Notificar(res);
            if (!string.IsNullOrEmpty(mensaje))
                mensajes_eventos += mensaje + "\n";

            return mensajes_eventos;
        }
    }
}
