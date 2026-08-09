using Bib_Hacienda.Clases;

namespace p_mvcHacienda.Servicios
{
    public class ConsultaUsuariosHacienda
    {
        private readonly UsuariosRegistradosHacienda _usuarios;

        public ConsultaUsuariosHacienda(UsuariosRegistradosHacienda usuarios)
        {
            _usuarios = usuarios;
        }

        public List<Usuario> ObtenerTodosLosUsuarios()
        {
            return _usuarios.Usuarios.OrderBy(u => u.Nombre).ToList();
        }

        public Usuario? BuscarUsuario(string nombre)
        {
            return _usuarios.Usuarios.FirstOrDefault(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }
    }
}
