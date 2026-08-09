using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class InventarioVacunas
    {
        private readonly IInventarioVacunas _inventarioVacunas;

        public InventarioVacunas(IInventarioVacunas inventarioVacunas)
        {
            _inventarioVacunas = inventarioVacunas;
        }

        public List<Vacuna> Vacunas => _inventarioVacunas.Vacunas;

        public Vacuna? BuscarPorLote(string lote)
        {
            return _inventarioVacunas.Vacunas.FirstOrDefault(v => v.Lote == lote);
        }

        public void Retirar(Vacuna vacuna)
        {
            _inventarioVacunas.Vacunas.Remove(vacuna);
        }

        public void AgregarCargadas(IEnumerable<Vacuna> vacunas)
        {
            foreach (var vacuna in vacunas)
            {
                _inventarioVacunas.Vacunas.Add(vacuna);
            }
        }
    }
}
