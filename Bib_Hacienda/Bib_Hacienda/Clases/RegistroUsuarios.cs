using Bib_Hacienda.Interfaces;
using System.Collections.Generic;

namespace Bib_Hacienda.Clases
{
    public class RegistroUsuarios : ILecturaUsuarios, IEscrituraUsuarios
    {
        private readonly List<Usuario> usuarios_registrados = new List<Usuario>();

        public IReadOnlyCollection<Usuario> ObtenerTodos()
        {
            return usuarios_registrados.AsReadOnly();
        }

        public void Agregar(Usuario usuario)
        {
            usuarios_registrados.Add(usuario);
        }
    }
}
