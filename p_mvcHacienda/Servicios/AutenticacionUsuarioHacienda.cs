using Bib_Hacienda.Clases;

namespace p_mvcHacienda.Servicios
{
    public class AutenticacionUsuarioHacienda
    {
        private readonly UsuariosRegistradosHacienda _usuarios;

        public AutenticacionUsuarioHacienda(UsuariosRegistradosHacienda usuarios)
        {
            _usuarios = usuarios;
        }

        public bool AutenticarUsuario(string nombre, string contrasena)
        {
            return _usuarios.Usuarios.Any(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)
                && u.Contrasena == contrasena);
        }
    }
}
