using Bib_Hacienda.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Reglas;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Eventos
{
    public class PublisherPotreroMitad : INotificadorPotreroMitad
    {
        //delegado y evento
        public delegate void delegado_potrero_mitad(string mensaje);
        public event delegado_potrero_mitad evt_potrero_mitad;

        //Metodo que salta el evento cuando el potrero alcanza la mitad de su capacidad
        public void Informar_Potrero_Mitad(ushort cantidad_reses, Potrero potrero)
        {
            try
            {
                //Capacidad a la mitad del potrero
                ushort capacidad_mitad = (ushort)(ReglaPotrero.max_reses_potrero / 2);

                //Validar si la cantidad de reses es igual a la capacidad a la mitad
                if (cantidad_reses == capacidad_mitad)
                {
                    string mensaje = $"[Evento] El potrero '{potrero.Identificacion}' ha alcanzado la mitad de su capacidad máxima de reses.";

                    if (evt_potrero_mitad != null)
                    {
                        evt_potrero_mitad(mensaje);
                    }
                    else
                    {
                        // Si no hay suscriptores, solo no hacer nada (el evento es opcional)
                    }
                }
            }
            catch (Exception er)
            {
                throw new Exception("[Evento] Error inesperado en el metodo Informar_Potrero_Mitad: " + er.Message);
            }
        }

        public string Notificar(ushort cantidad_reses, Potrero potrero)
        {
            string mensaje = string.Empty;
            delegado_potrero_mitad handler = m =>
            {
                if (!string.IsNullOrEmpty(m))
                    mensaje = m;
            };

            evt_potrero_mitad += handler;
            try
            {
                Informar_Potrero_Mitad(cantidad_reses, potrero);
            }
            finally
            {
                evt_potrero_mitad -= handler;
            }

            return mensaje;
        }
    }
}
