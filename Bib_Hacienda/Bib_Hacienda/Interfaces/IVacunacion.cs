using Bib_Hacienda.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Interfaces
{
    public interface IVacunacion
    {
        //Metodo para aplicar vacuna
        string aplicar_vacuna(Vacuna vacuna, string nombre, string id_potrero);
    }
}
