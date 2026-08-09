namespace Bib_Hacienda.Clases
{
    public class Carne : ProductoDerivado
    {
        public Carne(string codigo, string nombre, uint existencia, uint precioUnitario)
            : base(codigo, nombre, existencia, precioUnitario)
        {
        }

        public override string TipoProducto => "Carne";
        public override string UnidadMedida => "Kilogramos";
    }
}
