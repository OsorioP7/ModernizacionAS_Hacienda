using Bib_Hacienda.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Interfaces
{
    public interface IVentaRes
    {
        //Metodo para vender res
        string vender_res(string id_potrero, string nombre, uint monto);
    }
}
