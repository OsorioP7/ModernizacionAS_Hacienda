using Bib_Hacienda.Clases;
using System;

namespace Bib_Hacienda.Interfaces
{
    public interface ICreacionVacunaViva
    {
        string crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Viva.enum_l_atenuaciones grado_atenuacion);
        string crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Viva.enum_l_atenuaciones grado_atenuacion, uint cantidad);
    }
}
