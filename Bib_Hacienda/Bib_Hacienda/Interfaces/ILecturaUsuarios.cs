using Bib_Hacienda.Clases;
using System.Collections.Generic;

namespace Bib_Hacienda.Interfaces
{
    public interface ILecturaUsuarios
    {
        IReadOnlyCollection<Usuario> ObtenerTodos();
    }
}
