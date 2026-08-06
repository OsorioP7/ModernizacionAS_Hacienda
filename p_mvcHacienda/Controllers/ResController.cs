using Bib_Hacienda.Clases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using p_mvcHacienda.Servicios;

namespace p_mvcHacienda.Controllers
{
    public class ResController : Controller
    {
        // Atributos
        private readonly ResService _resService;
        private readonly PotreroService _potreroService;
        private readonly Hacienda _hacienda;
        private readonly PersistenciaService _persistencia;

        //Constructor con inyección de dependencias
        public ResController(ResService resService, PotreroService potreroService, Hacienda hacienda, PersistenciaService persistencia)
        {
            _resService = resService;
            _potreroService = potreroService;
            _hacienda = hacienda;
            _persistencia = persistencia;
        }

        // GET: Res/Index - Listar todas las reses
        [HttpGet]
        public ActionResult Index()
        {
            var resesConPotrero = _resService.ObtenerTodasLasReses();
            var estadisticas = _resService.ObtenerEstadisticas();

            ViewBag.Estadisticas = estadisticas;

            return View(resesConPotrero);
        }

        // Ver vacunas aplicadas por res
        [HttpGet]
        public ActionResult DetalleVacunas(string potreroId, string nombreRes)
        {
            try
            {
                var potrero = _hacienda.buscar_potrero(potreroId);
                var res = potrero.buscar_res(nombreRes);
                if (res == null)
                {
                    TempData["Mensaje"] = "Res no encontrada";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.PotreroId = potreroId;
                ViewBag.NombreRes = nombreRes;
                return View(res.L_vacunas_aplicadas);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Res/Create - Mostrar formulario de creación
        public ActionResult Create()
        {
            ViewBag.Potreros = _potreroService.ObtenerTodosLosPotreros();
            return View();
        }

        // POST: Res/Create - Procesar creación de res
        [HttpPost]
        public ActionResult Create(string potreroId, string nombre, ushort edad, uint peso)
        {
            try
            {
                string mensaje;

                // Validar entrada
                if (string.IsNullOrWhiteSpace(potreroId) || string.IsNullOrWhiteSpace(nombre))
                {
                    ViewBag.Mensaje = "Todos los campos son requeridos";
                    ViewBag.TipoMensaje = "danger";
                    ViewBag.Potreros = _potreroService.ObtenerTodosLosPotreros();
                    return View();
                }

                // Usar PotreroService para agregar y guardar, y obtener mensaje compuesto
                mensaje = _potreroService.AgregarRes(potreroId, nombre, edad, peso);

                    TempData["Mensaje"] = mensaje;
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"{ex.Message}";
                ViewBag.TipoMensaje = "danger";
            }

            ViewBag.Potreros = _potreroService.ObtenerTodosLosPotreros();
            return View();
        }

        // POST: Res/Alimentar - Alimentar una res
        public ActionResult Alimentar(string potreroId, string nombreRes, uint cantidadAlimento)
        {
            try
            {
                string mensaje;

                // Validar cantidad de alimento
                mensaje = _hacienda.alimentar_res(potreroId, nombreRes, cantidadAlimento);

                // Guardar cambios en archivo
                _persistencia.GuardarReses(_hacienda.L_potreros);

                string mensajeAlimento = cantidadAlimento == 1 ? "vez" : "veces";
                TempData["Mensaje"] = mensaje;
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"{ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Res/Vender - Vender una res (validando overflow de monto)
        public ActionResult Vender(string potreroId, string nombreRes, string monto)
        {
            try
            {
                // Validar y convertir monto de forma segura
                if (string.IsNullOrWhiteSpace(monto))
                {
                    TempData["Mensaje"] = "El monto es requerido";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                // Intentar convertir a decimal primero
                if (!decimal.TryParse(monto, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var montoDec))
                {
                    TempData["Mensaje"] = "Monto inválido";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                // Validar límites de uint
                if (montoDec < 0 || montoDec > uint.MaxValue)
                {
                    TempData["Mensaje"] = $"El monto excede el máximo permitido ({uint.MaxValue})";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                var montoUint = (uint)montoDec;

                string mensaje;

                // Vende la res y envía el mensaje
                mensaje = _hacienda.vender_res(potreroId, nombreRes, montoUint);

                // Guardar cambios en archivos: ventas y reses
                _persistencia.GuardarVentas(_hacienda.L_ventas);
                _persistencia.GuardarReses(_hacienda.L_potreros);

                    TempData["Mensaje"] = mensaje;
                    TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"{ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
