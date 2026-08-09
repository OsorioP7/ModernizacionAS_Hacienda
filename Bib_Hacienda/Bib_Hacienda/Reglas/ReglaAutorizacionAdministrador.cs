using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Reglas
{
    public class ReglaAutorizacionAdministrador : IReglasAutorizacionRol
    {
        public string Rol => "admin";

        public bool TienePermiso(string operacion)
        {
            return true;
        }
    }
}
