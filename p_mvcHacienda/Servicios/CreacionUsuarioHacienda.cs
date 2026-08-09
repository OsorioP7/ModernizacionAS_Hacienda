using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class CreacionUsuarioHacienda
    {
        private readonly UsuariosRegistradosHacienda _usuarios;
        private readonly IGuardadoUsuarios _guardadoUsuarios;

        public CreacionUsuarioHacienda(
            UsuariosRegistradosHacienda usuarios,
            IGuardadoUsuarios guardadoUsuarios)
        {
            _usuarios = usuarios;
            _guardadoUsuarios = guardadoUsuarios;
        }

        public string CrearUsuario(string nombre, string contrasena)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre del usuario no puede estar vacío");
                if (string.IsNullOrWhiteSpace(contrasena))
                    throw new ArgumentException("La contraseña no puede estar vacía");
                if (_usuarios.Usuarios.Any(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException($"Ya existe un usuario con el nombre '{nombre}'");

                var nuevoUsuario = new Usuario(nombre, contrasena);
                _usuarios.Usuarios.Add(nuevoUsuario);
                _guardadoUsuarios.GuardarUsuarios(_usuarios.Usuarios);
                return $"Usuario '{nombre}' creado exitosamente";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }
    }
}
