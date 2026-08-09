using System.Collections.Generic;
using System.Linq;
using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class EstadisticasPotreros
    {
        private readonly IConsultaPotreros _consulta;

        public EstadisticasPotreros(IConsultaPotreros consulta)
        {
            _consulta = consulta;
        }

        public Dictionary<string, object> ObtenerEstadisticas()
        {
            var potreros = _consulta.ObtenerTodosLosPotreros().ToList();

            return new Dictionary<string, object>
            {
                { "TotalPotreros", potreros.Count },
                { "TotalReses", potreros.Sum(p => p.L_reses.Count) },
                { "PotrerosVacios", potreros.Count(p => p.L_reses.Count == 0) },
                { "PotrerosConReses", potreros.Count(p => p.L_reses.Count > 0) }
            };
        }
    }
}
