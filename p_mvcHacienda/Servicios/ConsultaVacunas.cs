using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class ConsultaVacunas
    {
        private readonly ICargaVacunas _cargaVacunas;
        private readonly IConsultaPotreros _consultaPotreros;
        private readonly InventarioVacunas _inventarioVacunas;

        public ConsultaVacunas(
            ICargaVacunas cargaVacunas,
            IConsultaPotreros consultaPotreros,
            InventarioVacunas inventarioVacunas)
        {
            _cargaVacunas = cargaVacunas;
            _consultaPotreros = consultaPotreros;
            _inventarioVacunas = inventarioVacunas;
        }

        public List<Vacuna> ObtenerVacunasDisponibles()
        {
            if (_inventarioVacunas.Vacunas.Count == 0)
            {
                _inventarioVacunas.AgregarCargadas(_cargaVacunas.CargarVacunas());
            }

            return _inventarioVacunas.Vacunas.OrderBy(v => v.Nombre).ToList();
        }

        public List<Vacuna> ObtenerVacunasAplicadas(string potreroId, string nombreRes)
        {
            try
            {
                var potrero = _consultaPotreros.BuscarPotrero(potreroId);
                var res = potrero.buscar_res(nombreRes);
                return res.L_vacunas_aplicadas;
            }
            catch
            {
                return new List<Vacuna>();
            }
        }
    }
}
