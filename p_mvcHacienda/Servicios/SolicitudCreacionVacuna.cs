using static Bib_Hacienda.Clases.Viva;

namespace p_mvcHacienda.Servicios
{
    public class SolicitudCreacionVacuna
    {
        public string TipoVacuna { get; }
        public string Nombre { get; }
        public string Lote { get; }
        public string FechaVencimiento { get; }
        public string FechaAplicacion { get; }
        public uint? PeriodoAplicacion { get; }
        public enum_l_atenuaciones? Atenuacion { get; }

        public SolicitudCreacionVacuna(
            string tipoVacuna,
            string nombre,
            string lote,
            string fechaVencimiento,
            string fechaAplicacion,
            uint? periodoAplicacion,
            enum_l_atenuaciones? atenuacion)
        {
            TipoVacuna = tipoVacuna;
            Nombre = nombre;
            Lote = lote;
            FechaVencimiento = fechaVencimiento;
            FechaAplicacion = fechaAplicacion;
            PeriodoAplicacion = periodoAplicacion;
            Atenuacion = atenuacion;
        }
    }
}
