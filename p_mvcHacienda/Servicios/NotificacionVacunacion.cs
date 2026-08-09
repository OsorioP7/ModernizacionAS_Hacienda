using Bib_Hacienda.Clases;
using Bib_Hacienda.Eventos;

namespace p_mvcHacienda.Servicios
{
    public class NotificacionVacunacion
    {
        private readonly PublisherVacunacionCompletada _publisherVacunacionCompleta = new PublisherVacunacionCompletada();
        private readonly PublisherVacunaVencida _publisherVacunaVencida = new PublisherVacunaVencida();

        public bool InformarVacunaVencida(Vacuna vacuna, out string mensaje)
        {
            string mensajeCapturado = "";
            _publisherVacunaVencida.evt_vacuna_vencida += (mensajeEvento) =>
            {
                mensajeCapturado = mensajeEvento;
            };

            bool vacunaVencida = _publisherVacunaVencida.Informar_Vacuna_Vencida(vacuna);
            mensaje = mensajeCapturado;
            return vacunaVencida;
        }

        public bool InformarVacunacionCompletada(Res res, byte contadorBacterianas, byte contadorVivas, out string mensaje)
        {
            string mensajeCapturado = "";
            _publisherVacunacionCompleta.evt_vacunacion_completada += (mensajeEvento) =>
            {
                mensajeCapturado = mensajeEvento;
            };

            bool esquemaCompleto = _publisherVacunacionCompleta.Informar_Vacunacion_Completada(
                res,
                contadorBacterianas,
                contadorVivas);
            mensaje = mensajeCapturado;
            return esquemaCompleto;
        }

        public string ConsolidarValidaciones(string a, string b)
        {
            a = (a ?? string.Empty).Trim();
            b = (b ?? string.Empty).Trim();
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) return a;
            if (a.Contains(b, StringComparison.OrdinalIgnoreCase)) return a;
            if (b.Contains(a, StringComparison.OrdinalIgnoreCase)) return b;
            return a.Length > 0 ? a : b;
        }

        public string AsegurarPuntoFinal(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje)) return mensaje;
            return mensaje.EndsWith(".") ? mensaje : mensaje + ".";
        }
    }
}
