using Bib_Hacienda.Clases;

namespace p_mvcHacienda.Servicios
{
    public class ResService
    {
        // Atributos
        private readonly Hacienda _hacienda;
        private readonly PersistenciaService _persistencia;

        // Constructor
        public ResService(Hacienda hacienda, PersistenciaService persistencia)
        {
            _hacienda = hacienda;
            _persistencia = persistencia;
        }

        // Obtener todas las reses de todos los potreros
        public List<(Potrero Potrero, Res Res)> ObtenerTodasLasReses()
        {
            // Lista para almacenar las reses junto con su potrero
            var resesConPotrero = new List<(Potrero, Res)>();

            // Recorrer cada potrero y sus reses
            foreach (var potrero in _hacienda.L_potreros)
            {
                // Agregar cada res junto con su potrero a la lista
                foreach (var res in potrero.L_reses)
                {
                    resesConPotrero.Add((potrero, res));
                }
            }

            return resesConPotrero;
        }

        // Buscar res en un potrero
        public Res? BuscarRes(string potreroId, string nombreRes) //signo de pregunta porque es nulleable o sea que
                                                                 //busca una res y si no la encuentra devuelve null
        {
            try
            {
                // Buscar el potrero por su identificación
                var potrero = _hacienda.buscar_potrero(potreroId);
                return potrero.buscar_res(nombreRes);
            }
            catch
            {
                return null;
            }
        }

        // Obtener estadísticas de reses
        public Dictionary<string, object> ObtenerEstadisticas()
        {
            // Obtener todas las reses
            var todasLasReses = ObtenerTodasLasReses();

            // Estadísticas con orden correcto:
            // Terneros (0-12 meses) = jóvenes
            // Cebones (13-48 meses) = medios 
            // Novillos (49+ meses) = viejos
            return new Dictionary<string, object>
            {
                { "TotalReses", todasLasReses.Count },
                { "Terneros", todasLasReses.Count(r => r.Res is Ternero) }, // Jóvenes (0-12 meses)
                { "Cebones", todasLasReses.Count(r => r.Res is Cebon) },   // Medios (13-48 meses)
                { "Novillos", todasLasReses.Count(r => r.Res is Novillo) }, // Viejos (49+ meses)
                { "PesoPromedio", todasLasReses.Any() ? todasLasReses.Average(r => r.Res.Peso) : 0 }
            };
        }
    }
}
