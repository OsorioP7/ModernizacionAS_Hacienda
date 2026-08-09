using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class RegistroVacunacion
    {
        private readonly IActualizacionInventarioVacunas _actualizacionInventarioVacunas;
        private readonly IActualizacionHistorialVacunacion _actualizacionHistorialVacunacion;
        private readonly IActualizacionPotreros _actualizacionPotreros;
        private readonly IActualizacionReses _actualizacionReses;

        public RegistroVacunacion(
            IActualizacionInventarioVacunas actualizacionInventarioVacunas,
            IActualizacionHistorialVacunacion actualizacionHistorialVacunacion,
            IActualizacionPotreros actualizacionPotreros,
            IActualizacionReses actualizacionReses)
        {
            _actualizacionInventarioVacunas = actualizacionInventarioVacunas;
            _actualizacionHistorialVacunacion = actualizacionHistorialVacunacion;
            _actualizacionPotreros = actualizacionPotreros;
            _actualizacionReses = actualizacionReses;
        }

        public (string validadoAplicadas, string validadoDisponibles) Registrar(
            List<Potrero> potreros,
            List<Vacuna> vacunas)
        {
            var validadoAplicadas = _actualizacionHistorialVacunacion.GuardarVacunasAplicadas(potreros);
            var validadoDisponibles = _actualizacionInventarioVacunas.GuardarVacunas(vacunas);
            _actualizacionPotreros.ActualizarPotreros(potreros);
            _actualizacionReses.ActualizarReses(potreros);
            return (validadoAplicadas, validadoDisponibles);
        }
    }
}
