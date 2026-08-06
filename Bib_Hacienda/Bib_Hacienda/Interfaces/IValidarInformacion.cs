using Bib_Hacienda.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Interfaces
{
    // SEGREGACION DE INTERFACES
    //Interfaz para validar información antes de guardar en base de datos
    public interface IValidarInformacion
    {
        //Valida si una res cumple con los requisitos para ser guardada en BD
        bool ValidarRes(Res res);

        //Valida si un potrero cumple con los requisitos para ser guardado en BD
        bool ValidarPotrero(Potrero potrero);

        //Valida si una vacuna cumple con los requisitos para ser guardada en BD
        bool ValidarVacuna(Vacuna vacuna);

        //Valida si una venta cumple con los requisitos para ser guardada en BD
        bool ValidarVenta(Venta venta);
    }
}
