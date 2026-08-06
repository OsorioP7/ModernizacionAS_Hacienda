using Bib_Hacienda.Clases;

namespace p_mvcHacienda.Servicios
{
    public class VentaService
    {
        // Atributos
        private readonly Hacienda _hacienda;
        private readonly PersistenciaService _persistencia;

        public VentaService(Hacienda hacienda, PersistenciaService persistencia)
        {
            _hacienda = hacienda;
            _persistencia = persistencia;
        }

        // Obtener todas las ventas
        public List<Venta> ObtenerTodasLasVentas()
        {
            // Ordenar las ventas por fecha descendente
            return _hacienda.L_ventas.OrderByDescending(v => v.Fecha).ToList();
        }

        // Obtener ventas por potrero
        public List<Venta> ObtenerVentasPorPotrero(string potreroId)
        {
            // Filtrar ventas por el ID del potrero
            return _hacienda.L_ventas
                .Where(v => v.Potrero.Identificacion == potreroId)
                .OrderByDescending(v => v.Fecha)
                .ToList();
        }

        // Obtener ventas por rango de fechas
        public List<Venta> ObtenerVentasPorFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            // Filtrar ventas dentro del rango de fechas
            return _hacienda.L_ventas
                .Where(v => v.Fecha >= fechaInicio && v.Fecha <= fechaFin)
                .OrderByDescending(v => v.Fecha)
                .ToList();
        }

        // Obtener estadísticas de ventas
        public Dictionary<string, object> ObtenerEstadisticas()
        {
            // Calcular estadísticas básicas de ventas
            var ventas = _hacienda.L_ventas;

            // Retornar un diccionario con las estadísticas
            return new Dictionary<string, object>
            {
                { "TotalVentas", ventas.Count },
                { "MontoTotal", ventas.Sum(v => v.Monto) },
                { "PromedioVenta", ventas.Any() ? ventas.Average(v => v.Monto) : 0 },
                { "VentasEsteMes", ventas.Count(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year) },
                { "MontoEsteMes", ventas.Where(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year).Sum(v => v.Monto) }
            };
        }
    }
}
