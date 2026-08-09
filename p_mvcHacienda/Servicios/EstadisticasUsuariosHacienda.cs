namespace p_mvcHacienda.Servicios
{
    public class EstadisticasUsuariosHacienda
    {
        private readonly UsuariosRegistradosHacienda _usuarios;

        public EstadisticasUsuariosHacienda(UsuariosRegistradosHacienda usuarios)
        {
            _usuarios = usuarios;
        }

        public Dictionary<string, object> ObtenerEstadisticas()
        {
            return new Dictionary<string, object>
            {
                {"TotalUsuarios", _usuarios.Usuarios.Count}
            };
        }
    }
}
