using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Clases;
using Bib_Hacienda.Reglas;

namespace Bib_Hacienda.Eventos
{
    public class PublisherPesoMin
    {
        //delegado y evento
        public delegate void dele_peso_min(string peso_min);
        public event dele_peso_min evt_peso_min;

        //Metodo para informar si la res está por debajo del peso mínimo
        public void Informar_Peso_Min(Res res)
        {
            try
            {
                //Validar si la res está por debajo del peso mínimo según su tipo
                ushort peso_minimo = 0;

                if (res is Ternero) { peso_minimo = ReglaRes.peso_min_ternero; }
                else if (res is Cebon) { peso_minimo = ReglaRes.peso_min_cebon; }
                else if (res is Novillo) { peso_minimo = ReglaRes.peso_min_novillo; }

                //Informar si la res está en desnutrición
                if (res.Peso < peso_minimo)
                {
                    string mensaje = $"[Evento] La res '{res.Nombre}' tiene un peso {res.Peso}, está en desnutrición.";

                    if (evt_peso_min != null)
                    {
                        evt_peso_min(mensaje);
                    }
                    else
                    {
                        // Si no hay suscriptores, solo no hacer nada (el evento es opcional)
                    }
                }
            }
            catch (Exception er)
            {
                throw new Exception("[Evento] Error inesperado en el metodo Informar_Peso_Min: " + er.Message);
            }
        }

        public static implicit operator PublisherPesoMin(PublisherPesoVenta v)
        {
            throw new NotImplementedException();
        }
    }
}
