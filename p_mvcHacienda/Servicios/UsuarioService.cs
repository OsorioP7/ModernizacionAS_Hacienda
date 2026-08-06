using Bib_Hacienda.Clases;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace p_mvcHacienda.Servicios
{
    public class UsuarioService
    {   // Atributos
        private static List<Usuario> _usuarios = new List<Usuario>();
        private readonly PersistenciaService _persistencia;

        // Constructor
        public UsuarioService(PersistenciaService persistencia)
        {
            _persistencia = persistencia;
        }

        // Cargar usuarios desde persistencia
        public void CargarUsuarios()
        {
            _usuarios = _persistencia.CargarUsuarios();
        }

        // Crear un nuevo usuario
        public string CrearUsuario(string nombre, string contrasena)
        {
            try
            {      // Validaciones básicas
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre del usuario no puede estar vacío");
                }

                if (string.IsNullOrWhiteSpace(contrasena))
                {
                    // Validar que la contraseña no esté vacía
                    throw new ArgumentException("La contraseña no puede estar vacía");
                }

                if (_usuarios.Any(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                {
                    // Verificar si ya existe un usuario con el mismo nombre
                    throw new InvalidOperationException($"Ya existe un usuario con el nombre '{nombre}'");
                }
                
                // Crear y agregar el nuevo usuario
                var nuevoUsuario = new Usuario(nombre, contrasena);
                _usuarios.Add(nuevoUsuario);
                _persistencia.GuardarUsuarios(_usuarios);

                return $"Usuario '{nombre}' creado exitosamente";

            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        // Autenticar usuario
        public bool AutenticarUsuario(string nombre, string contrasena)
        {
            // Verificar si existe un usuario con el nombre y contraseña proporcionados
            return _usuarios.Any(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
            u.Contrasena == contrasena);
        }

        // Obtener todos los usuarios
        public List<Usuario> ObtenerTodosLosUsuarios()
        { 
            // Retornar la lista de usuarios ordenada por nombre
            return _usuarios.OrderBy(u => u.Nombre).ToList();
        }

        // Buscar usuario por nombre
        public Usuario? BuscarUsuario(string nombre)
        {   
            // Retornar el usuario que coincida con el nombre proporcionado
            return _usuarios.FirstOrDefault(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }

        // Obtener estadísticas
        public Dictionary<string, object> ObtenerEstadisticas()
        {
            // Retornar un diccionario con el total de usuarios
            return new Dictionary<string, object>
            {
                {"TotalUsuarios", _usuarios.Count}
            };
        }

        public async Task<(bool, IEnumerable<Claim>)> ValidateUserAsync(string username, string password)
        {
            var user = _usuarios.FirstOrDefault(u => u.Nombre.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Contrasena == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Nombre),
                    // Puedes agregar más claims si tienes roles u otra información
                };
                return (true, claims);
            }

            return (false, null);
        }
    }
}
