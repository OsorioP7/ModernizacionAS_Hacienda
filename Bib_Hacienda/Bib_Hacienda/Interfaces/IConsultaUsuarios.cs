using Bib_Hacienda.Clases;
using System.Collections.Generic;

namespace Bib_Hacienda.Interfaces
{
    public interface IConsultaUsuarios
    {
        List<Usuario> listar_usuarios();
        Usuario buscar_usuario(string nombre);
        bool ExisteUsuario(string nombre);
    }
}
