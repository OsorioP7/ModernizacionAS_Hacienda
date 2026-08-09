using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class PotreroService
    {
        private readonly IConsultaPotreros _consultaPotreros;
        private readonly IRegistroPotrero _registroPotrero;
        private readonly ITiposPotreroDisponibles _tiposPotrero;
        private readonly IActualizacionPotreros _actualizacionPotreros;
        private readonly IActualizacionReses _actualizacionReses;
        private readonly INotificacionIncorporacionRes _notificacionIncorporacion;

        public PotreroService(
            IConsultaPotreros consultaPotreros,
            IRegistroPotrero registroPotrero,
            ITiposPotreroDisponibles tiposPotrero,
            IActualizacionPotreros actualizacionPotreros,
            IActualizacionReses actualizacionReses,
            INotificacionIncorporacionRes notificacionIncorporacion)
        {
            _consultaPotreros = consultaPotreros;
            _registroPotrero = registroPotrero;
            _tiposPotrero = tiposPotrero;
            _actualizacionPotreros = actualizacionPotreros;
            _actualizacionReses = actualizacionReses;
            _notificacionIncorporacion = notificacionIncorporacion;
        }

        public string CrearPotrero(string identificacion, string tipo)
        {
            try
            {
                string validado;
                var potreros = _consultaPotreros.ObtenerTodosLosPotreros();

                if (potreros.Any(p => p.Identificacion == identificacion))
                {
                    throw new InvalidOperationException($"Ya existe un potrero con la identificación '{identificacion}'");
                }

                string resultado;
                try
                {
                    if (string.IsNullOrWhiteSpace(identificacion))
                    {
                        throw new ArgumentException("El nombre de la res no puede estar vacío", nameof(identificacion));
                    }

                    if (potreros.Any(p => p.Identificacion.Equals(identificacion, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new InvalidOperationException($"Ya existe un potrero con el nombre '{identificacion}'.");
                    }

                    IReglasTipoPotrero reglas = _tiposPotrero.Obtener(tipo);
                    Potrero nuevoPotrero = new Potrero(identificacion, reglas, _notificacionIncorporacion);
                    _registroPotrero.Agregar(nuevoPotrero);

                    resultado = $"El potrero {identificacion} se a añadido a la hacienda. ";
                }
                catch (Exception er)
                {
                    throw new Exception("Error inesperado en el metodo crear_potrero: " + er.Message);
                }

                validado = _actualizacionPotreros.ActualizarPotreros(
                    _consultaPotreros.ObtenerTodosLosPotreros().ToList());

                return $"{resultado}. {validado}";
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Validación fallida: El potrero no cumple los requisitos");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear el potrero: {ex.Message}");
            }
        }

        public string AgregarRes(string potreroId, string nombreRes, ushort edad, uint peso)
        {
            try
            {
                string validado;
                var potrero = _consultaPotreros.BuscarPotrero(potreroId);

                if (potrero == null)
                {
                    throw new InvalidOperationException($"No se encontró el potrero '{potreroId}'");
                }

                if (potrero.L_reses.Any(r => r.Nombre == nombreRes))
                {
                    throw new InvalidOperationException($"Ya existe una res con el nombre '{nombreRes}' en el potrero '{potreroId}'");
                }

                string resultado;
                try
                {
                    resultado = potrero.anadir_res(nombreRes, edad, peso);
                }
                catch (Exception er)
                {
                    throw new Exception("Error inesperado en el método anadir_res_potrero: " + er.Message);
                }

                validado = _actualizacionReses.ActualizarReses(
                    _consultaPotreros.ObtenerTodosLosPotreros().ToList());

                return $"{resultado}. {validado}";
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Validación fallida: La res no cumple los requisitos");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar la res: {ex.Message}");
            }
        }
    }
}
