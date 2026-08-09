using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class VentaService : IVentaRes
    {
        // Atributos
        private readonly IEstadoVentas _estadoVentas;
        private readonly IRegistroVenta _registroVenta;
        private readonly IConsultaPotreros _consultaPotreros;
        private readonly IActualizacionVentas _actualizacionVentas;
        private readonly IActualizacionReses _actualizacionReses;

        public VentaService(
            IEstadoVentas estadoVentas,
            IRegistroVenta registroVenta,
            IConsultaPotreros consultaPotreros,
            IActualizacionVentas actualizacionVentas,
            IActualizacionReses actualizacionReses)
        {
            _estadoVentas = estadoVentas;
            _registroVenta = registroVenta;
            _consultaPotreros = consultaPotreros;
            _actualizacionVentas = actualizacionVentas;
            _actualizacionReses = actualizacionReses;
        }

        // Obtener todas las ventas
        public List<Venta> ObtenerTodasLasVentas()
        {
            // Ordenar las ventas por fecha descendente
            return _estadoVentas.Ventas.OrderByDescending(v => v.Fecha).ToList();
        }

        // Obtener ventas por potrero
        public List<Venta> ObtenerVentasPorPotrero(string potreroId)
        {
            // Filtrar ventas por el ID del potrero
            return _estadoVentas.Ventas
                .Where(v => v.Potrero.Identificacion == potreroId)
                .OrderByDescending(v => v.Fecha)
                .ToList();
        }

        // Obtener ventas por rango de fechas
        public List<Venta> ObtenerVentasPorFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            // Filtrar ventas dentro del rango de fechas
            return _estadoVentas.Ventas
                .Where(v => v.Fecha >= fechaInicio && v.Fecha <= fechaFin)
                .OrderByDescending(v => v.Fecha)
                .ToList();
        }

        // Obtener estadísticas de ventas
        public Dictionary<string, object> ObtenerEstadisticas()
        {
            // Calcular estadísticas básicas de ventas
            var ventas = _estadoVentas.Ventas;

            // Retornar las estadísticas
            return new Dictionary<string, object>
            {
                { "TotalVentas", ventas.Count },
                { "MontoTotal", ventas.Sum(v => v.Monto) },
                { "PromedioVenta", ventas.Any() ? ventas.Average(v => v.Monto) : 0 },
                { "VentasEsteMes", ventas.Count(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year) },
                { "MontoEsteMes", ventas.Where(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year).Sum(v => v.Monto) }
            };
        }

        // Método para vender res trasladado desde Hacienda.
        public string vender_res(string id_potrero, string nombre, uint monto)
        {
            try
            {
                // Pedimos el potrero y la res
                Potrero potrero = _consultaPotreros.BuscarPotrero(id_potrero);
                Res res = potrero.buscar_res(nombre);

                // Validar parámetros
                if (potrero == null) throw new ArgumentNullException(nameof(potrero));
                if (res == null) throw new ArgumentNullException(nameof(res));

                // Crear la venta
                Venta venta = new Venta(potrero, DateTime.Now, res, monto);
                // Agregar la venta a la lista de ventas
                _registroVenta.Agregar(venta);
                // Remover la res del potrero
                _consultaPotreros.ObtenerTodosLosPotreros()
                    .Where(p => p == potrero)
                    .FirstOrDefault()
                    .L_reses.Remove(res);
                _actualizacionVentas.ActualizarVentas(_estadoVentas.Ventas.ToList());
                _actualizacionReses.ActualizarReses(_consultaPotreros.ObtenerTodosLosPotreros().ToList());
                return $"Venta de la res {res.Nombre} realizada con exito";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo vender_res: " + er.Message);
            }
        }
    }
}
