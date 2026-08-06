using Bib_Hacienda.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Interfaces
{
    public interface ICreacionVacuna
    {
        string crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion);
        string crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Viva.enum_l_atenuaciones grado_atenuacion);
        string crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion, uint cantidad);
        string crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Viva.enum_l_atenuaciones grado_atenuacion, uint cantidad);

    }
}
