using Bib_Hacienda.Clases;
using Bib_Hacienda.Eventos;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class ResService
    {
        // Atributos
        private readonly IConsultaPotreros _consultaPotreros;
        private readonly IActualizacionReses _actualizacionReses;

        private PublisherPesoMin publisherPesoMin = new PublisherPesoMin();
        private PublisherPesoVenta publisherPesoVenta = new PublisherPesoVenta();

        // Constructor
        public ResService(IConsultaPotreros consultaPotreros, IActualizacionReses actualizacionReses)
        {
            _consultaPotreros = consultaPotreros;
            _actualizacionReses = actualizacionReses;
        }

        // Obtener todas las reses de todos los potreros
        public List<(Potrero Potrero, Res Res)> ObtenerTodasLasReses()
        {
            // Lista para almacenar las reses junto con su potrero
            var resesConPotrero = new List<(Potrero, Res)>();

            // Recorrer cada potrero y sus reses
            foreach (var potrero in _consultaPotreros.ObtenerTodosLosPotreros())
            {
                // Agregar cada res junto con su potrero
                foreach (var res in potrero.L_reses)
                {
                    resesConPotrero.Add((potrero, res));
                }
            }

            return resesConPotrero;
        }

        // Buscar res en un potrero
        public Res? BuscarRes(string potreroId, string nombreRes) //signo de pregunta porque es nulleable o sea que
                                                                 //busca una res y si no la encuentra devuelve null
        {
            try
            {
                // Buscar el potrero por su identificación
                var potrero = _consultaPotreros.BuscarPotrero(potreroId);
                return potrero.buscar_res(nombreRes);
            }
            catch
            {
                return null;
            }
        }

        // Método para alimentar una res trasladado desde Hacienda
        public string AlimentarRes(string idPotrero, string nombre)
        {
            try
            {
                var potrero = _consultaPotreros.BuscarPotrero(idPotrero);
                var res = potrero.buscar_res(nombre);
                string mensajeFinal = "";

                // Validar parámetros
                if (potrero == null) throw new ArgumentNullException(nameof(potrero));
                if (res == null) throw new ArgumentNullException(nameof(res));

                // Alimentar la res (incrementa el peso)
                res.Peso++;

                string mensajeEventos = "";

                // Suscribirse a los eventos con lambdas para acumular mensajes
                publisherPesoMin.evt_peso_min += (mensaje) =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                        mensajeEventos += mensaje + "\n";
                };

                publisherPesoVenta.evt_peso_venta += (mensaje) =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                        mensajeEventos += mensaje + "\n";
                };

                // Disparar los eventos con la res actualizada
                publisherPesoMin.Informar_Peso_Min(res);
                publisherPesoVenta.Informar_Peso_Venta(res);

                // Construir mensaje de retorno
                mensajeFinal = $"La res '{res.Nombre}' ha sido alimentada, ahora pesa {res.Peso} kg.";
                if (!string.IsNullOrEmpty(mensajeEventos))
                {
                    mensajeFinal += "\n" + mensajeEventos.TrimEnd();
                }
                _actualizacionReses.ActualizarReses(_consultaPotreros.ObtenerTodosLosPotreros().ToList());
                return mensajeFinal;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo alimentar_res: " + er.Message);
            }
        }

        // Método sobrecargado para alimentar una res con una cantidad específica
        public string AlimentarRes(string idPotrero, string nombre, uint cantidadAlimento)
        {
            try
            {
                var potrero = _consultaPotreros.BuscarPotrero(idPotrero);
                var res = potrero.buscar_res(nombre);

                // Validar parámetros
                if (potrero == null) throw new ArgumentNullException(nameof(potrero));
                if (res == null) throw new ArgumentNullException(nameof(res));

                res.Peso += cantidadAlimento;

                string mensajeEventos = "";

                // Suscribirse a los eventos con lambdas para acumular mensajes
                publisherPesoMin.evt_peso_min += (mensaje) =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                        mensajeEventos += mensaje + "\n";
                };

                publisherPesoVenta.evt_peso_venta += (mensaje) =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                        mensajeEventos += mensaje + "\n";
                };

                // Disparar los eventos con la res actualizada
                publisherPesoMin.Informar_Peso_Min(res);
                publisherPesoVenta.Informar_Peso_Venta(res);

                // Construir mensaje de retorno
                string mensajeFinal = $"La res '{res.Nombre}' ha sido alimentada, ahora pesa {res.Peso} kg.";
                if (!string.IsNullOrEmpty(mensajeEventos))
                {
                    mensajeFinal += "\n" + mensajeEventos.TrimEnd();
                }

                _actualizacionReses.ActualizarReses(_consultaPotreros.ObtenerTodosLosPotreros().ToList());
                return mensajeFinal;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo alimentar_res: " + er.Message);
            }
        }

        // Obtener estadísticas de reses
        public Dictionary<string, object> ObtenerEstadisticas()
        {
            // Obtener todas las reses
            var todasLasReses = ObtenerTodasLasReses();

            var estadisticas = new Dictionary<string, object>
            {
                { "TotalReses", todasLasReses.Count },
                { "Terneros", 0 },
                { "Cebones", 0 },
                { "Novillos", 0 }
            };

            var grupos = todasLasReses.GroupBy(r => r.Res.TipoRes);
            foreach (var grupo in grupos)
            {
                estadisticas[grupo.Key] = grupo.Count();
            }

            estadisticas.Add(
                "PesoPromedio",
                todasLasReses.Any() ? todasLasReses.Average(r => r.Res.Peso) : 0);

            return estadisticas;
        }
    }
}
