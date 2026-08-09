using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class AplicacionVacuna : IVacunacion
    {
        private readonly InventarioVacunas _inventarioVacunas;
        private readonly IConsultaPotreros _consultaPotreros;
        private readonly ConsultaVacunas _consultaVacunas;
        private readonly NotificacionVacunacion _notificacionVacunacion;
        private readonly RegistroVacunacion _registroVacunacion;

        public AplicacionVacuna(
            InventarioVacunas inventarioVacunas,
            IConsultaPotreros consultaPotreros,
            ConsultaVacunas consultaVacunas,
            NotificacionVacunacion notificacionVacunacion,
            RegistroVacunacion registroVacunacion)
        {
            _inventarioVacunas = inventarioVacunas;
            _consultaPotreros = consultaPotreros;
            _consultaVacunas = consultaVacunas;
            _notificacionVacunacion = notificacionVacunacion;
            _registroVacunacion = registroVacunacion;
        }

        public string AplicarVacuna(string potreroId, string nombreRes, string loteVacuna)
        {
            try
            {
                if (_inventarioVacunas.Vacunas.Count == 0)
                {
                    _consultaVacunas.ObtenerVacunasDisponibles();
                }

                var vacuna = _inventarioVacunas.BuscarPorLote(loteVacuna);
                if (vacuna == null)
                {
                    throw new Exception($"No se encontró una vacuna con el lote '{loteVacuna}'");
                }

                string resultadoDominio = aplicar_vacuna(vacuna, nombreRes, potreroId);
                var potreros = _consultaPotreros.ObtenerTodosLosPotreros().ToList();
                var validaciones = _registroVacunacion.Registrar(potreros, _inventarioVacunas.Vacunas);
                var validado = _notificacionVacunacion.ConsolidarValidaciones(
                    validaciones.validadoAplicadas,
                    validaciones.validadoDisponibles);

                return _notificacionVacunacion.AsegurarPuntoFinal(
                    $"{resultadoDominio}. {validado}".Trim());
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        public string aplicar_vacuna(Vacuna vacuna, string nombre, string id_potrero)
        {
            try
            {
                string mensajeVacuna = "";
                string mensajeVacunacion = "";
                Potrero potrero = _consultaPotreros.BuscarPotrero(id_potrero);
                Res res = potrero.buscar_res(nombre);
                byte contadorBacterianas = 0;
                byte contadorVivas = 0;

                if (vacuna == null) throw new ArgumentNullException(nameof(vacuna));
                if (res == null) throw new ArgumentNullException(nameof(res));

                if (res.L_vacunas_aplicadas.Any(v => v.Nombre == vacuna.Nombre || v.Lote == vacuna.Lote))
                    throw new Exception($"La vacuna '{vacuna.Nombre}' ya fue aplicada a la res '{res.Nombre}'.");

                foreach (Vacuna vac in res.L_vacunas_aplicadas)
                {
                    vac.AcumularContador(ref contadorBacterianas, ref contadorVivas);
                }

                if (res is not IReglasVacunacionRes reglas)
                {
                    throw new InvalidOperationException("La res no tiene reglas de vacunación definidas.");
                }

                vacuna.ValidarLimite(reglas, contadorBacterianas, contadorVivas, res.Nombre);

                bool vacunaVencida = _notificacionVacunacion.InformarVacunaVencida(
                    vacuna,
                    out mensajeVacuna);

                if (vacunaVencida)
                {
                    throw new Exception(mensajeVacuna);
                }
                else
                {
                    res.L_vacunas_aplicadas.Add(vacuna);
                    _inventarioVacunas.Retirar(vacuna);
                    vacuna.AcumularContador(ref contadorBacterianas, ref contadorVivas);

                    bool esquemaCompleto = _notificacionVacunacion.InformarVacunacionCompletada(
                        res,
                        contadorBacterianas,
                        contadorVivas,
                        out mensajeVacunacion);

                    return $"Vacuna aplicada correctamente a la res {res.Nombre}. {mensajeVacunacion}";
                }
            }
            catch (Exception err)
            {
                throw new Exception("Error inesperado en el metodo aplicar_vacuna: " + err.Message);
            }
        }
    }
}
