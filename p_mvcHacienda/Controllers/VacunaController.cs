using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using p_mvcHacienda.Servicios;
using static Bib_Hacienda.Clases.Viva;
using System.Globalization;

namespace p_mvcHacienda.Controllers
{
    public class VacunaController : Controller
    {
        // Atributos
        private readonly VacunaService _vacunaService;
        private readonly ResService _resService;
        private readonly PotreroService _potreroService;

        //Constructor con inyección de dependencias
        public VacunaController(VacunaService vacunaService, ResService resService, PotreroService potreroService)
        {
            _vacunaService = vacunaService;
            _resService = resService;
            _potreroService = potreroService;
        }

        // GET: Vacuna/Index - Listar todas las vacunas
        [HttpGet]
        public ActionResult Index()
        {
            var vacunas = _vacunaService.ObtenerVacunasDisponibles();
            var estadisticas = _vacunaService.ObtenerEstadisticas();

            ViewBag.Estadisticas = estadisticas;

            return View(vacunas);
        }

        // GET: Vacuna/Create - Mostrar formulario de creación
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // GET: Vacuna/Aplicar - Mostrar formulario de aplicación
        [HttpGet]
        public ActionResult Aplicar()
        {
            ViewBag.Potreros = _potreroService.ObtenerTodosLosPotreros();
            ViewBag.Reses = _resService.ObtenerTodasLasReses();
            ViewBag.Vacunas = _vacunaService.ObtenerVacunasDisponibles();
            return View();
        }

        // POST: Vacuna/Create - Procesar creación de vacuna
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string tipoVacuna, string nombre, string lote,
            string fechaVencimiento, string fechaAplicacion,    
            uint? periodoAplicacion, enum_l_atenuaciones? atenuacion)
        {
            try
            {
                string resultado;

                // Validar campos requeridos básicos
                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(lote))
                {
                    ViewBag.Mensaje = "El nombre y lote son requeridos";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                // Parsear fechas desde inputs HTML date (yyyy-MM-dd)
                if (!DateTime.TryParseExact(fechaVencimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaVenc))
                {
                    ViewBag.Mensaje = "Fecha de vencimiento inválida";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }
                if (!DateTime.TryParseExact(fechaAplicacion, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaAplic))
                {
                    ViewBag.Mensaje = "Fecha de aplicación inválida";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                // Reglas simples de fecha
                if (fechaAplic > fechaVenc)
                {
                    ViewBag.Mensaje = "La fecha de aplicación no puede ser posterior a la fecha de vencimiento";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                if (tipoVacuna == "Bacteriana")
                {
                    //HasValue para validar que no sea nulo la entrada del formulario en la vista
                    if (!periodoAplicacion.HasValue)
                    {
                        ViewBag.Mensaje = "El período de aplicación es requerido para vacunas bacterianas";
                        ViewBag.TipoMensaje = "danger";
                        return View();
                    }
                    // Pasar null para atenuación en bacterianas
                    resultado = _vacunaService.CrearVacuna(nombre, lote, fechaVenc, fechaAplic, periodoAplicacion.Value, null);
                }
                else // Viva
                {
                    if (!atenuacion.HasValue)
                    {
                        ViewBag.Mensaje = "La atenuación es requerida para vacunas vivas";
                        ViewBag.TipoMensaje = "danger";
                        return View();
                    }
                    // Pasar null para período en vivas
                    resultado = _vacunaService.CrearVacuna(nombre, lote, fechaVenc, fechaAplic, null, atenuacion.Value);
                }

                if (resultado.Contains("x"))
                {
                    TempData["Mensaje"] = resultado;
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Mensaje = resultado;
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $" Error: {ex.Message}";
                ViewBag.TipoMensaje = "danger";
                return View();
            }
        }

        // POST: Vacuna/Aplicar - Procesar aplicación de vacuna
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aplicar(string potreroId, string nombreRes, string loteVacuna)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(potreroId) || string.IsNullOrWhiteSpace(nombreRes) || string.IsNullOrWhiteSpace(loteVacuna))
                {
                    ViewBag.Mensaje = " Todos los campos son requeridos";
                    ViewBag.TipoMensaje = "danger";
                    ViewBag.Potreros = _potreroService.ObtenerTodosLosPotreros();
                    ViewBag.Reses = _resService.ObtenerTodasLasReses();
                    ViewBag.Vacunas = _vacunaService.ObtenerVacunasDisponibles();
                    return View();
                }

                var resultado = _vacunaService.AplicarVacuna(potreroId, nombreRes, loteVacuna);

                TempData["Mensaje"] = resultado;
                TempData["TipoMensaje"] = resultado.Contains("x") ? "success" : "danger";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $" Error: {ex.Message}";
                ViewBag.TipoMensaje = "danger";
                ViewBag.Potreros = _potreroService.ObtenerTodosLosPotreros();
                ViewBag.Reses = _resService.ObtenerTodasLasReses();
                ViewBag.Vacunas = _vacunaService.ObtenerVacunasDisponibles();
                return View();
            }
        }
    }
}
