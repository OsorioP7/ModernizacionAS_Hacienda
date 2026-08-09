using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using p_mvcHacienda.Servicios;
using Bib_Hacienda.Interfaces;
using static Bib_Hacienda.Clases.Viva;

namespace p_mvcHacienda.Controllers
{
    public class VacunaController : Controller
    {
        // Atributos
        private readonly AplicacionVacuna _aplicacionVacuna;
        private readonly ConsultaVacunas _consultaVacunas;
        private readonly EstadisticasVacunas _estadisticasVacunas;
        private readonly IReadOnlyDictionary<string, ICreacionPorTipoVacuna> _formulariosPorTipo;
        private readonly ICreacionPorTipoVacuna _formularioPredeterminado;
        private readonly ResService _resService;
        private readonly IConsultaPotreros _consultaPotreros;

        //Constructor con inyección de dependencias
        public VacunaController(
            AplicacionVacuna aplicacionVacuna,
            ConsultaVacunas consultaVacunas,
            EstadisticasVacunas estadisticasVacunas,
            IReadOnlyDictionary<string, ICreacionPorTipoVacuna> formulariosPorTipo,
            ICreacionPorTipoVacuna formularioPredeterminado,
            ResService resService,
            IConsultaPotreros consultaPotreros)
        {
            _aplicacionVacuna = aplicacionVacuna;
            _consultaVacunas = consultaVacunas;
            _estadisticasVacunas = estadisticasVacunas;
            _formulariosPorTipo = formulariosPorTipo;
            _formularioPredeterminado = formularioPredeterminado;
            _resService = resService;
            _consultaPotreros = consultaPotreros;
        }

        // GET: Vacuna/Index - Listar todas las vacunas
        [HttpGet]
        public ActionResult Index()
        {
            var vacunas = _consultaVacunas.ObtenerVacunasDisponibles();
            var estadisticas = _estadisticasVacunas.ObtenerEstadisticas();

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
            ViewBag.Potreros = _consultaPotreros.ObtenerTodosLosPotreros();
            ViewBag.Reses = _resService.ObtenerTodasLasReses();
            ViewBag.Vacunas = _consultaVacunas.ObtenerVacunasDisponibles();
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
                var solicitud = new SolicitudCreacionVacuna(
                    tipoVacuna,
                    nombre,
                    lote,
                    fechaVencimiento,
                    fechaAplicacion,
                    periodoAplicacion,
                    atenuacion);

                var formulario = _formulariosPorTipo.TryGetValue(
                    tipoVacuna ?? string.Empty,
                    out var encontrado)
                    ? encontrado
                    : _formularioPredeterminado;

                string resultado = formulario.Crear(solicitud);

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
                    ViewBag.Potreros = _consultaPotreros.ObtenerTodosLosPotreros();
                    ViewBag.Reses = _resService.ObtenerTodasLasReses();
                    ViewBag.Vacunas = _consultaVacunas.ObtenerVacunasDisponibles();
                    return View();
                }

                var resultado = _aplicacionVacuna.AplicarVacuna(potreroId, nombreRes, loteVacuna);

                TempData["Mensaje"] = resultado;
                TempData["TipoMensaje"] = resultado.Contains("x") ? "success" : "danger";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $" Error: {ex.Message}";
                ViewBag.TipoMensaje = "danger";
                ViewBag.Potreros = _consultaPotreros.ObtenerTodosLosPotreros();
                ViewBag.Reses = _resService.ObtenerTodasLasReses();
                ViewBag.Vacunas = _consultaVacunas.ObtenerVacunasDisponibles();
                return View();
            }
        }
    }
}
