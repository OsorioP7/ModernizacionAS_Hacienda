using Bib_Hacienda.Clases;

namespace p_mvcHacienda.Servicios
{
    public class EstadisticasVacunas
    {
        private readonly InventarioVacunas _inventarioVacunas;
        private readonly ConsultaVacunas _consultaVacunas;

        public EstadisticasVacunas(
            InventarioVacunas inventarioVacunas,
            ConsultaVacunas consultaVacunas)
        {
            _inventarioVacunas = inventarioVacunas;
            _consultaVacunas = consultaVacunas;
        }

        public Dictionary<string, object> ObtenerEstadisticas()
        {
            if (_inventarioVacunas.Vacunas.Count == 0)
            {
                _consultaVacunas.ObtenerVacunasDisponibles();
            }

            var vacunas = _inventarioVacunas.Vacunas;
            var estadisticas = new Dictionary<string, object>
            {
                { "TotalVacunas", vacunas.Count },
                { "Bacterianas", 0 },
                { "Vivas", 0 },
                { "Vencidas", vacunas.Count(v => v.Fecha_vencimiento < DateTime.Now) },
                { "Vigentes", vacunas.Count(v => v.Fecha_vencimiento >= DateTime.Now) }
            };

            foreach (var grupo in vacunas.GroupBy(v => v.TipoVacuna))
            {
                estadisticas[grupo.Key] = grupo.Count();
            }

            return estadisticas;
        }
    }
}
