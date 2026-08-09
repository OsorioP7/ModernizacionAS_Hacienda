using System.Collections.Generic;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class TiposPotreroDisponibles : ITiposPotreroDisponibles
    {
        private readonly IReadOnlyDictionary<string, IReglasTipoPotrero> _reglasPorTipo;

        public TiposPotreroDisponibles(IReadOnlyDictionary<string, IReglasTipoPotrero> reglasPorTipo)
        {
            _reglasPorTipo = reglasPorTipo;
        }

        public IReglasTipoPotrero Obtener(string tipo)
        {
            return _reglasPorTipo[tipo];
        }
    }
}
