using Bib_Hacienda.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace p_mvcHacienda.Controllers
{
    public class ProductoDerivadoController : Controller
    {
        private readonly IInventarioProductosDerivados inventario;
        private readonly IVentaProductoDerivado ventaProductoDerivado;

        public ProductoDerivadoController(
            IInventarioProductosDerivados inventario,
            IVentaProductoDerivado ventaProductoDerivado)
        {
            this.inventario = inventario;
            this.ventaProductoDerivado = ventaProductoDerivado;
        }

        public ActionResult Index()
        {
            return View(inventario.Productos);
        }

        [HttpPost]
        public ActionResult Vender(string codigo, uint cantidad)
        {
            try
            {
                TempData["Mensaje"] = ventaProductoDerivado.VenderProductoDerivado(codigo, cantidad);
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
