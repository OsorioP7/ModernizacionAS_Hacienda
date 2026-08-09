namespace Bib_Hacienda.Clases
{
    public class Piel : ProductoDerivado
    {
        public Piel(string codigo, string nombre, uint existencia, uint precioUnitario)
            : base(codigo, nombre, existencia, precioUnitario)
        {
        }

        public override string TipoProducto => "Piel";
        public override string UnidadMedida => "Unidades";
    }
}
