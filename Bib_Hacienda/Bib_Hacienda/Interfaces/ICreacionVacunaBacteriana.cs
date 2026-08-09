using Bib_Hacienda.Clases;
using System;

namespace Bib_Hacienda.Interfaces
{
    public interface ICreacionVacunaBacteriana
    {
        string crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion);
        string crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion, uint cantidad);
    }
}
