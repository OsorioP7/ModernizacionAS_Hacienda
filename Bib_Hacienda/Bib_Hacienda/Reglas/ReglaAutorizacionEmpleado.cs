using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Reglas
{
    public class ReglaAutorizacionEmpleado : IReglasAutorizacionRol
    {
        public string Rol => "empleado";

        public bool TienePermiso(string operacion)
        {
            return !operacion.Contains("Eliminar");
        }
    }
}
