namespace Bib_Hacienda.Clases
{
    public class Lacteo : ProductoDerivado
    {
        public Lacteo(string codigo, string nombre, uint existencia, uint precioUnitario)
            : base(codigo, nombre, existencia, precioUnitario)
        {
        }

        public override string TipoProducto => "Lacteo";
        public override string UnidadMedida => "Litros";
    }
}
