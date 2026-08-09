using System;
using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;
using System.Globalization;

namespace Bib_Hacienda.Reglas.ReglasCargaVacuna
{
    public class ReglaCargaBacteriana : IReglaCargaTipoVacuna
    {
        public string TipoVacuna => "Bacteriana";

        public Vacuna Cargar(string[] partes)
        {
            if (partes.Length >= 8)
            {
                string nombreVacuna = partes[2];
                string loteAplicado = partes[3];
                DateTime fechaVencAplicada;
                DateTime fechaAplicAplicada;
                uint periodoAplicado;

                if (!DateTime.TryParseExact(partes[4].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaVencAplicada))
                {
                    return null;
                }
                if (!DateTime.TryParseExact(partes[5].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaAplicAplicada))
                {
                    return null;
                }
                periodoAplicado = uint.TryParse(partes[7].Trim(), out var perAplicado) ? perAplicado : 0u;

                return new Bacteriana(nombreVacuna, loteAplicado, fechaVencAplicada, fechaAplicAplicada, periodoAplicado);
            }

            string nombre = partes[0];
            string lote = partes[1];
            DateTime fechaVenc;
            DateTime fechaAplic;
            uint periodo;

            if (!DateTime.TryParseExact(partes[2].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaVenc))
            {
                return null;
            }
            if (!DateTime.TryParseExact(partes[3].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaAplic))
            {
                return null;
            }
            periodo = uint.TryParse(partes[5].Trim(), out var per) ? per : 0u;

            if (!uint.TryParse(partes[5].Trim(), out periodo) || periodo < 2 || periodo > 4)
            {
                return null;
            }
            try
            {
                return new Bacteriana(nombre, lote, fechaVenc, fechaAplic, periodo);
            }
            catch
            {
                return null;
            }
        }

    }
}
