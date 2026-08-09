using System;
using System.Collections.Generic;
using System.Linq;
using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class ConsultaPotreros : IConsultaPotreros
    {
        private readonly IEstadoPotreros _estado;

        public ConsultaPotreros(IEstadoPotreros estado)
        {
            _estado = estado;
        }

        public List<Potrero> ObtenerTodosLosPotreros()
        {
            return _estado.Potreros.OrderBy(p => p.Identificacion).ToList();
        }

        IReadOnlyCollection<Potrero> IConsultaPotreros.ObtenerTodosLosPotreros()
        {
            return ObtenerTodosLosPotreros();
        }

        public Potrero? ObtenerPotreroPorIdentificacion(string identificacion)
        {
            try
            {
                return BuscarPotrero(identificacion);
            }
            catch
            {
                return null;
            }
        }

        public Potrero BuscarPotrero(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre de búsqueda no puede estar vacío.");

                var resultados = _estado.Potreros
                    .Where(p => p.Identificacion.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                if (!resultados.Any())
                    throw new Exception($"No se encontró ningún potrero con el nombre o coincidencia '{nombre}'.");

                if (resultados.Count > 1)
                    throw new Exception($" se encontró mas de un potrero con el nombre o coincidencia '{nombre}'.");

                return resultados.First();
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método buscar_potrero: " + er.Message);
            }
        }
    }
}
