using System.Collections.Generic;
using Bib_Hacienda.Clases;

namespace p_mvcHacienda.Servicios
{
    public class UsuariosRegistradosHacienda
    {
        private readonly List<Usuario> _usuarios;

        public List<Usuario> Usuarios
        {
            get { return _usuarios; }
        }

        public UsuariosRegistradosHacienda(List<Usuario> usuarios)
        {
            _usuarios = usuarios;
        }
    }
}
