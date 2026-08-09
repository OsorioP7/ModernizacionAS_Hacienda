using System.Globalization;
using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class PersistenciaProductosDerivados : IActualizacionProductosDerivados
    {
        private readonly IAlmacenamientoHacienda almacenamiento;
        private readonly IReadOnlyDictionary<string, Func<string, string, uint, uint, ProductoDerivado>> creadoresPorTipo;

        public PersistenciaProductosDerivados(
            IAlmacenamientoHacienda almacenamiento,
            IReadOnlyDictionary<string, Func<string, string, uint, uint, ProductoDerivado>> creadoresPorTipo)
        {
            this.almacenamiento = almacenamiento;
            this.creadoresPorTipo = creadoresPorTipo;
        }

        public List<ProductoDerivado> CargarProductosDerivados()
        {
            var productos = new List<ProductoDerivado>();
            if (!almacenamiento.Existe("ProductosDerivados"))
            {
                return productos;
            }

            foreach (var linea in almacenamiento.Cargar("ProductosDerivados"))
            {
                if (string.IsNullOrWhiteSpace(linea))
                {
                    continue;
                }

                var partes = linea.Split('|');
                if (partes.Length < 5 ||
                    !uint.TryParse(partes[3], NumberStyles.None, CultureInfo.InvariantCulture, out var existencia) ||
                    !uint.TryParse(partes[4], NumberStyles.None, CultureInfo.InvariantCulture, out var precioUnitario) ||
                    !creadoresPorTipo.TryGetValue(partes[0].Trim(), out var creador))
                {
                    continue;
                }

                productos.Add(creador(
                    partes[1].Trim(),
                    partes[2],
                    existencia,
                    precioUnitario));
            }

            return productos;
        }

        public string ActualizarProductosDerivados(IReadOnlyCollection<ProductoDerivado> productos)
        {
            var lineas = productos.Select(producto =>
                string.Join("|", new[]
                {
                    producto.TipoProducto,
                    producto.Codigo,
                    producto.Nombre,
                    producto.Existencia.ToString(CultureInfo.InvariantCulture),
                    producto.PrecioUnitario.ToString(CultureInfo.InvariantCulture)
                }));

            almacenamiento.Guardar("ProductosDerivados", lineas);
            return "Guardado exitosamente";
        }
    }
}
