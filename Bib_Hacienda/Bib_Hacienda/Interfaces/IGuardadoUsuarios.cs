using System.Collections.Generic;
using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface IGuardadoUsuarios
    {
        string GuardarUsuarios(List<Usuario> usuarios);
    }
}
