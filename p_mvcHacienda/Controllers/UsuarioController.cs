using Microsoft.AspNetCore.Mvc;
using p_mvcHacienda.Servicios;

namespace p_mvcHacienda.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: Usuario/Index - Listar todos los usuarios
        [HttpGet]
        public ActionResult Index()
        {
            var usuarios = _usuarioService.ObtenerTodosLosUsuarios();
            var estadisticas = _usuarioService.ObtenerEstadisticas();

            ViewBag.Estadisticas = estadisticas;

            return View(usuarios);
        }

        // GET: Usuario/Create - Mostrar formulario de creación
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create - Procesar creación de usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string nombre, string contrasena)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(contrasena))
                {
                    ViewBag.Mensaje = "❌ Todos los campos son requeridos";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                var resultado = _usuarioService.CrearUsuario(nombre, contrasena);

                if (resultado.Contains("✅"))
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
                ViewBag.Mensaje = $"❌ Error: {ex.Message}";
                ViewBag.TipoMensaje = "danger";
                return View();
            }
        }
    }
}
