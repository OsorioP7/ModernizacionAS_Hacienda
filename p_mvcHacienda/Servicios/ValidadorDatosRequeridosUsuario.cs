using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class ValidadorDatosRequeridosUsuario : IValidador<Usuario>
    {
        public bool Validar(Usuario usuario)
        {
            return !string.IsNullOrWhiteSpace(usuario.Nombre)
                && !string.IsNullOrWhiteSpace(usuario.Contrasena);
        }
    }
}
