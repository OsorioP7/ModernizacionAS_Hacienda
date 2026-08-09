using System;
using System.Collections.Generic;
using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Reglas.ReglasCargaVacuna
{
    public class CargaVacuna : ICargaVacuna
    {
        private readonly IReadOnlyDictionary<string, IReglaCargaTipoVacuna> reglasPorTipo;
        private readonly IReglaCargaTipoVacuna reglaPorDefecto;

        public CargaVacuna(
            IReadOnlyDictionary<string, IReglaCargaTipoVacuna> reglasPorTipo,
            IReglaCargaTipoVacuna reglaPorDefecto)
        {
            this.reglasPorTipo = reglasPorTipo;
            this.reglaPorDefecto = reglaPorDefecto;
        }

        public Vacuna Cargar(string[] partes)
        {
            bool esFormatoAplicado = partes.Length >= 8;
            string tipo = esFormatoAplicado ? partes[6] : partes[4].Trim();
            StringComparer comparador = esFormatoAplicado
                ? StringComparer.Ordinal
                : StringComparer.OrdinalIgnoreCase;

            return ObtenerRegla(tipo, comparador).Cargar(partes);
        }

        private IReglaCargaTipoVacuna ObtenerRegla(string tipo, StringComparer comparador)
        {
            foreach (var entrada in reglasPorTipo)
            {
                if (comparador.Equals(entrada.Key, tipo))
                {
                    return entrada.Value;
                }
            }

            return reglaPorDefecto;
        }
    }
}
