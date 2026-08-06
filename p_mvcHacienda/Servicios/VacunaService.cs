using Bib_Hacienda.Clases;
using static Bib_Hacienda.Clases.Viva;

namespace p_mvcHacienda.Servicios
{
    public class VacunaService
    {
        // Atributos
        private readonly Hacienda _hacienda;
        private readonly PersistenciaService _persistencia;
        // Constructor
        public VacunaService(Hacienda hacienda, PersistenciaService persistencia)
        {
            _hacienda = hacienda;
            _persistencia = persistencia;
        }

        // Crear vacuna viva o bacteriana
        public string CrearVacuna(string nombre, string lote, DateTime fechaVencimiento, DateTime fechaAplicacion, uint? periodoAplicacion, enum_l_atenuaciones? atenuacion)
        {
            try
            {
                string validado;
                string resultadoDominio;

                // Bacteriana: requiere periodo y NO atenuación
                if (periodoAplicacion.HasValue && !atenuacion.HasValue)
                {
                    resultadoDominio = _hacienda.crear_vacuna(nombre, lote, fechaVencimiento, fechaAplicacion, periodoAplicacion.Value);
                }
                // Viva: requiere atenuación y NO periodo
                else if (!periodoAplicacion.HasValue && atenuacion.HasValue)
                {
                    resultadoDominio = _hacienda.crear_vacuna(nombre, lote, fechaVencimiento, fechaAplicacion, atenuacion.Value);
                }
                else
                {
                    return "Error: parámetros inválidos para crear la vacuna (revise tipo, período o atenuación)";
                }

                validado = _persistencia.GuardarVacunas(_hacienda.L_vacunas);

                // Mensaje compuesto: evento de dominio + mensaje del aspecto de persistencia
                return $"{resultadoDominio}. {validado}";
            }

            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        // Aplicar vacuna a una res
        public string AplicarVacuna(string potreroId, string nombreRes, string loteVacuna)
        {
            try
            {
                // Asegurar catálogo cargado
                if (_hacienda.L_vacunas.Count ==0)
                {
                    var cargadas = _persistencia.CargarVacunas();
                    foreach (var v in cargadas) _hacienda.L_vacunas.Add(v);
                }

                // Buscar la vacuna por su lote
                var vacuna = _hacienda.L_vacunas.FirstOrDefault(v => v.Lote == loteVacuna);
                if (vacuna == null)
                {
                    throw new Exception($"No se encontró una vacuna con el lote '{loteVacuna}'");
                }

                // Aplicar la vacuna desde la hacienda (dispara eventos del dominio)
                string resultadoDominio = _hacienda.aplicar_vacuna(vacuna, nombreRes, potreroId);

                // Remover del inventario disponible si aún existe (evitar duplicidad)
                var existente = _hacienda.L_vacunas.FirstOrDefault(v => v.Lote == loteVacuna);
                if (existente != null)
                {
                    _hacienda.L_vacunas.Remove(existente);
                }

                // Persistir cambios (aspecto devuelve mensaje)
                var validadoAplicadas = _persistencia.GuardarVacunasAplicadas(_hacienda.L_potreros);
                var validadoDisponibles = _persistencia.GuardarVacunas(_hacienda.L_vacunas);
                _persistencia.GuardarPotreros(_hacienda.L_potreros);
                _persistencia.GuardarReses(_hacienda.L_potreros);

                // Consolidar mensajes de guardado para evitar duplicados
                var validado = ConsolidarValidaciones(validadoAplicadas, validadoDisponibles);

                // Mensaje final sin repeticiones adicionales
                return AsegurarPuntoFinal($"{resultadoDominio}. {validado}".Trim());
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        // Obtener todas las vacunas disponibles
        public List<Vacuna> ObtenerVacunasDisponibles()
        {
            // Lazy-load desde archivo si el catálogo está vacío
            if (_hacienda.L_vacunas.Count ==0)
            {
                var cargadas = _persistencia.CargarVacunas();
                foreach (var v in cargadas) _hacienda.L_vacunas.Add(v);
            }
            return _hacienda.L_vacunas.OrderBy(v => v.Nombre).ToList();
        }

        // Obtener vacunas aplicadas a una res
        public List<Vacuna> ObtenerVacunasAplicadas(string potreroId, string nombreRes)
        {
            try
            {
                // Buscar el potrero por su identificación
                var potrero = _hacienda.buscar_potrero(potreroId);
                var res = potrero.buscar_res(nombreRes);
                return res.L_vacunas_aplicadas;
            }
            catch
            {
                return new List<Vacuna>();
            }
        }

        // Obtener estadísticas de vacunas
        public Dictionary<string, object> ObtenerEstadisticas()
        {
            // Asegurar catálogo cargado
            if (_hacienda.L_vacunas.Count ==0)
            {
                var cargadas = _persistencia.CargarVacunas();
                foreach (var v in cargadas) _hacienda.L_vacunas.Add(v);
            }

            var vacunas = _hacienda.L_vacunas;
            return new Dictionary<string, object>
            {
                { "TotalVacunas", vacunas.Count },
                { "Bacterianas", vacunas.Count(v => v is Bacteriana) },
                { "Vivas", vacunas.Count(v => v is Viva) },
                { "Vencidas", vacunas.Count(v => v.Fecha_vencimiento < DateTime.Now) },
                { "Vigentes", vacunas.Count(v => v.Fecha_vencimiento >= DateTime.Now) }
            };
        }

        // Consolidar mensajes de validación: evita duplicados y normaliza
        private string ConsolidarValidaciones(string a, string b)
        {
            a = (a ?? string.Empty).Trim();
            b = (b ?? string.Empty).Trim();
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) return a;
            if (a.Contains(b, StringComparison.OrdinalIgnoreCase)) return a;
            if (b.Contains(a, StringComparison.OrdinalIgnoreCase)) return b;
            // Si son distintos, priorizar el primero (aplicadas) para evitar repetición
            return a.Length >0 ? a : b;
        }

        private string AsegurarPuntoFinal(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje)) return mensaje;
            return mensaje.EndsWith(".") ? mensaje : mensaje + ".";
        }
    }
}
