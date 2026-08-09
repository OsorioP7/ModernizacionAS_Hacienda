using Bib_Hacienda.Interfaces;
using System.Globalization;

namespace p_mvcHacienda.Servicios
{
    public class FormularioBacteriana : ICreacionPorTipoVacuna
    {
        private readonly ICreacionVacunaBacteriana creacionVacunaBacteriana;

        public FormularioBacteriana(ICreacionVacunaBacteriana creacionVacunaBacteriana)
        {
            this.creacionVacunaBacteriana = creacionVacunaBacteriana;
        }

        public string Crear(SolicitudCreacionVacuna solicitud)
        {
            if (string.IsNullOrWhiteSpace(solicitud.Nombre) || string.IsNullOrWhiteSpace(solicitud.Lote))
                return "El nombre y lote son requeridos";

            if (!DateTime.TryParseExact(solicitud.FechaVencimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaVenc))
                return "Fecha de vencimiento inválida";

            if (!DateTime.TryParseExact(solicitud.FechaAplicacion, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaAplic))
                return "Fecha de aplicación inválida";

            if (fechaAplic > fechaVenc)
                return "La fecha de aplicación no puede ser posterior a la fecha de vencimiento";

            if (!solicitud.PeriodoAplicacion.HasValue)
                return "El período de aplicación es requerido para vacunas bacterianas";

            return creacionVacunaBacteriana.crear_vacuna(
                solicitud.Nombre,
                solicitud.Lote,
                fechaVenc,
                fechaAplic,
                solicitud.PeriodoAplicacion.Value);
        }
    }
}
