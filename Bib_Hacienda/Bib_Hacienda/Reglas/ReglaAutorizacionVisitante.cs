using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Reglas
{
    public class ReglaAutorizacionVisitante : IReglasAutorizacionRol
    {
        public string Rol => "visitante";

        public bool TienePermiso(string operacion)
        {
            return operacion.Contains("Consultar") || operacion.Contains("Listar");
        }
    }
}
