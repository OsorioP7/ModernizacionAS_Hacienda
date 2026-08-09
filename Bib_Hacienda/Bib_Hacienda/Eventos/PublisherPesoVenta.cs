using Bib_Hacienda.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Eventos
{
    public class PublisherPesoVenta : INotificadorPesoVenta
    {
        //Definicion del delegado y el evento
        public delegate void dele_peso_venta(string peso_venta);
        public event dele_peso_venta evt_peso_venta;

        //Metodo para informar si la res está apta para la venta
        public void Informar_Peso_Venta(Res res)
        {
            try
            {
                    

                    //Determinar el peso recomendado para la venta segun el tipo de res
                    ushort peso_apto = res.ReglaPeso.PesoVenta;

                    //Informar si la res está apta para la venta
                    if (res.Peso >= peso_apto)
                    {
                        string mensaje = $"[Evento] La res '{res.Nombre}' tiene un peso {res.Peso}, apta para venta.";

                        if (evt_peso_venta != null)
                        {
                            evt_peso_venta(mensaje);
                        }
                        else
                        {
                            // Si no hay suscriptores, solo no hacer nada (el evento es opcional)
                        }
                    }
                
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo Informar_Peso_Venta: " + er.Message);
            }
        }

        public string Notificar(Res res)
        {
            string mensaje = string.Empty;
            dele_peso_venta handler = m =>
            {
                if (!string.IsNullOrEmpty(m))
                    mensaje = m;
            };

            evt_peso_venta += handler;
            try
            {
                Informar_Peso_Venta(res);
            }
            finally
            {
                evt_peso_venta -= handler;
            }

            return mensaje;
        }
    }
}
