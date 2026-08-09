using System.Collections.Generic;

namespace Bib_Hacienda.Interfaces
{
    public interface IAlmacenamientoHacienda
    {
        void Guardar(string nombreDatos, IEnumerable<string> registros);
        IReadOnlyCollection<string> Cargar(string nombreDatos);
        bool Existe(string nombreDatos);
    }
}
