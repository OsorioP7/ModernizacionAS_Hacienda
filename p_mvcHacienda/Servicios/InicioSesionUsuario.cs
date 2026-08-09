using System.Security.Claims;

namespace p_mvcHacienda.Servicios
{
    public class InicioSesionUsuario
    {
        private readonly UsuariosRegistradosHacienda _usuarios;

        public InicioSesionUsuario(UsuariosRegistradosHacienda usuarios)
        {
            _usuarios = usuarios;
        }

        public async Task<(bool, IEnumerable<Claim>)> ValidateUserAsync(string username, string password)
        {
            var user = _usuarios.Usuarios.FirstOrDefault(u =>
                u.Nombre.Equals(username, StringComparison.OrdinalIgnoreCase)
                && u.Contrasena == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Nombre),
                };
                return (true, claims);
            }

            return (false, null);
        }
    }
}
