using System.Collections.Generic;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Clases
{
    public class Hacienda : IEstadoPotreros, IRegistroPotrero, IInventarioVacunas, IEstadoVentas, IRegistroVenta
    {
        // Estado propiedad de la hacienda.
        private List<Potrero> l_potreros;
        private List<Venta> l_ventas;
        private List<Vacuna> l_vacunas;

        // Accesores públicos para los servicios (get público, set privado).
        public List<Potrero> L_potreros
        {
            get => l_potreros;
            private set => l_potreros = value;
        }

        public IReadOnlyCollection<Potrero> Potreros => L_potreros;

        public void Agregar(Potrero potrero)
        {
            L_potreros.Add(potrero);
        }

        public List<Venta> L_ventas
        {
            get => l_ventas;
            private set => l_ventas = value;
        }

        public IReadOnlyCollection<Venta> Ventas => L_ventas;

        public void Agregar(Venta venta)
        {
            L_ventas.Add(venta);
        }

        public List<Vacuna> L_vacunas
        {
            get => l_vacunas;
            private set => l_vacunas = value;
        }

        public List<Vacuna> Vacunas
        {
            get { return L_vacunas; }
        }

        // EventHandler conservado fuera del alcance de H-01.
        internal void EventHandler() { }

        // Constructor vacío.
        public Hacienda()
        {
            l_potreros = new List<Potrero>();
            l_ventas = new List<Venta>();
            l_vacunas = new List<Vacuna>();
        }
    }
}
