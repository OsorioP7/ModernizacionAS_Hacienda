using Bib_Hacienda.Clases;
using static Bib_Hacienda.Clases.Potrero;

namespace p_mvcHacienda.Servicios
{
    public class PotreroService
    {
        // Atributos
        private readonly Hacienda _hacienda;
        private readonly PersistenciaService _persistencia;

        // Constructor
        public PotreroService(Hacienda hacienda, PersistenciaService persistencia)
        {
            _hacienda = hacienda;
            _persistencia = persistencia;
        }

        // Crear un nuevo potrero
        public string CrearPotrero(string identificacion, l_tipos_potreros tipo)
        {
            try
            {
                string validado;
                // Verificar si ya existe un potrero con esa identificación
                if (_hacienda.L_potreros.Any(p => p.Identificacion == identificacion))
                {
                    throw new InvalidOperationException($"Ya existe un potrero con la identificación '{identificacion}'");
                }

                // Intentar crear el potrero (mensaje de evento del dominio)
                string resultado = _hacienda.crear_potrero(identificacion, tipo);

                // Guardar los cambios CON VALIDACIÓN (mensaje del aspecto de persistencia)
                validado = _persistencia.GuardarPotreros(_hacienda.L_potreros);

                // Mensaje compuesto: evento + guardado
                return $"{resultado}. {validado}";
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Validación fallida: El potrero no cumple los requisitos");
            }
            catch (Exception ex)
            {
                // Re-lanzar la excepción para que el controlador la maneje
                throw new Exception($"Error al crear el potrero: {ex.Message}");
            }
        }

        // Obtener todos los potreros
        public List<Potrero> ObtenerTodosLosPotreros()
        {
            return _hacienda.L_potreros.OrderBy(p => p.Identificacion).ToList();
        }

        // Obtener un potrero por identificación
        public Potrero? ObtenerPotreroPorIdentificacion(string identificacion)
        {
            try
            {
                return _hacienda.buscar_potrero(identificacion);
            }
            catch
            {
                return null;
            }
        }

        // Agregar una res al potrero
        public string AgregarRes(string potreroId, string nombreRes, ushort edad, uint peso)
        {
            try
            {
                string validado;

                // Verificar que el potrero existe
                var potrero = _hacienda.buscar_potrero(potreroId);
                if (potrero == null)
                {
                    throw new InvalidOperationException($"No se encontró el potrero '{potreroId}'");
                }

                // Verificar que no existe una res con ese nombre en el potrero
                if (potrero.L_reses.Any(r => r.Nombre == nombreRes))
                {
                    throw new InvalidOperationException($"Ya existe una res con el nombre '{nombreRes}' en el potrero '{potreroId}'");
                }

                // Usar el método de Hacienda (mensaje de evento del dominio)
                string resultado = _hacienda.anadir_res_potrero(potreroId, nombreRes, edad, peso);

                // Guardar con validación (mensaje del aspecto)
                validado = _persistencia.GuardarReses(_hacienda.L_potreros);

                // Mensaje compuesto: evento + guardado
                return $"{resultado}. {validado}";
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Validación fallida: La res no cumple los requisitos");
            }
            catch (Exception ex)
            {
                // Re-lanzar la excepción para que el controlador la maneje
                throw new Exception($"Error al agregar la res: {ex.Message}");
            }
        }

        // Obtener estadísticas
        public Dictionary<string, object> ObtenerEstadisticas()
        {
            var potreros = _hacienda.L_potreros;

            return new Dictionary<string, object>
            {
                { "TotalPotreros", potreros.Count },
                { "TotalReses", potreros.Sum(p => p.L_reses.Count) },
                { "PotrerosVacios", potreros.Count(p => p.L_reses.Count ==0) },
                { "PotrerosConReses", potreros.Count(p => p.L_reses.Count >0) }
            };
        }
    }
}
