using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Clases
{
    public class InicializacionUsuarios : IInicializacionUsuarios
    {
        private readonly IEscrituraUsuarios escritura_usuarios;

        public InicializacionUsuarios(IEscrituraUsuarios escritura_usuarios)
        {
            this.escritura_usuarios = escritura_usuarios;
        }

        public void Inicializar()
        {
            escritura_usuarios.Agregar(new Usuario("admin", "admin123"));
            escritura_usuarios.Agregar(new Usuario("empleado", "emp456"));
            escritura_usuarios.Agregar(new Usuario("visitante", "visit789"));
        }
    }
}
