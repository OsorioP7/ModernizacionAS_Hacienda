using Microsoft.AspNetCore.Mvc;
using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;
using p_mvcHacienda.Servicios;
using static Bib_Hacienda.Clases.Potrero;

namespace p_mvcHacienda.Controllers
{
    public class PotreroController : Controller
    {
        //Atributos
        private readonly PotreroService _potreroService;
        private readonly IConsultaPotreros _consultaPotreros;
        private readonly EstadisticasPotreros _estadisticasPotreros;

        //Inyección de dependencias del servicio
        public PotreroController(PotreroService potreroService, IConsultaPotreros consultaPotreros, EstadisticasPotreros estadisticasPotreros)
        {
            _potreroService = potreroService;
            _consultaPotreros = consultaPotreros;
            _estadisticasPotreros = estadisticasPotreros;
        }

        // GET
        [HttpGet]

        //Mostrar la lista de potreros y estadisticas
        public ActionResult Index()
        {
            var potreros = _consultaPotreros.ObtenerTodosLosPotreros();
            var estadisticas = _estadisticasPotreros.ObtenerEstadisticas();
      
            ViewBag.Estadisticas = estadisticas;

            return View(potreros);
        }

        
        // GET: Potrero/Create - Mostrar formulario de creación
        public ActionResult Create()
        {
            return View();
        }

        //Detalles de un potrero
        public ActionResult Details(string id)
        {
            var potrero = _consultaPotreros.ObtenerPotreroPorIdentificacion(id);

            if (potrero == null)
            {
                TempData["Mensaje"] = "Potrero no encontrado";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            return View(potrero);
        }

        // POST:
        [HttpPost]

        // Procesar creación de potrero
        public ActionResult Create(string identificacion, l_tipos_potreros tipo)
        {
            try
            { 
                // Validar entrada
                if (string.IsNullOrWhiteSpace(identificacion))
                {
                    ViewBag.Mensaje = "La identificación no puede estar vacía";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                // Llamar al servicio para crear potrero (persiste internamente)
                string exitoso = _potreroService.CrearPotrero(identificacion, tipo.ToString());
                

                // Si es exitoso, redirigir con mensaje de éxito
                TempData["Mensaje"] = exitoso;
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"{ex.Message}";
                ViewBag.TipoMensaje = "danger";
            }
  
            return View();
        }
    }
}
