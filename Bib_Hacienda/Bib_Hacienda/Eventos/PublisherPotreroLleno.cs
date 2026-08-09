using Bib_Hacienda.Clases;
using Bib_Hacienda.Reglas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Eventos
{
    public class PublisherPotreroLleno : INotificadorPotreroLleno
    {
        //Definicion del delegado y el evento
        public delegate void delegado_potrero_lleno(string mensaje);
        public event delegado_potrero_lleno evt_potrero_lleno;

        //Metodo para informar que el potrero está lleno
        public void Informar_Potrero_Lleno(ushort cantidad_reses, Potrero potrero)
        {
            try
            {
                //Notificar que el potrero ha alcanzado su capacidad máxima
                if (cantidad_reses == ReglaPotrero.max_reses_potrero)
                {
                    string mensaje = $"[Evento] El potrero '{potrero.Identificacion}' ha alcanzado su capacidad máxima de reses ({ReglaPotrero.max_reses_potrero}). No se pueden agregar más reses.";

                    if (evt_potrero_lleno != null)
                    {
                        evt_potrero_lleno(mensaje);
                    }
                    else
                    {
                        // Si no hay suscriptores, solo no hacer nada (el evento es opcional)
                        // No lanzar excepción porque es válido que no haya suscriptores
                    }
                }
            }
            catch (Exception er)
            {
                throw new Exception("[Evento] Error inesperado en el metodo Informar_Potrero_Lleno: " + er.Message);
            }
        }

        public string Notificar(ushort cantidad_reses, Potrero potrero)
        {
            string mensaje = string.Empty;
            delegado_potrero_lleno handler = m =>
            {
                if (!string.IsNullOrEmpty(m))
                    mensaje = m;
            };

            evt_potrero_lleno += handler;
            try
            {
                Informar_Potrero_Lleno(cantidad_reses, potrero);
            }
            finally
            {
                evt_potrero_lleno -= handler;
            }

            return mensaje;
        }
    }
}
