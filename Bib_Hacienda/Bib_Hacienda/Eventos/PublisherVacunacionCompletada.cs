using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;
using System;

namespace Bib_Hacienda.Eventos
{
    public class PublisherVacunacionCompletada
    {
        //Definicion del delegado y el evento (con mensaje)
        public delegate void dele_vacunacion_completada(string mensaje);
        public event dele_vacunacion_completada evt_vacunacion_completada;

        //Metodo para informar que una res ha completado su esquema de vacunacion
        public bool Informar_Vacunacion_Completada(Res res, ushort contador_bacterianas, ushort contador_vivas)
        {
            try
            {
                if (res == null)
                {
                    throw new ArgumentNullException(nameof(res), "La res no puede ser null");
                }

                //Verificar si la res ha completado su esquema de vacunacion
                bool esquema_completo = false;

                if (res is IReglasVacunacionRes reglas)
                {
                    esquema_completo = reglas.EsquemaCompleto(contador_bacterianas, contador_vivas);
                }

                // Disparar el evento con el mensaje apropiado
                if (evt_vacunacion_completada != null)
                {
                    string mensaje;
                    if (esquema_completo)
                    {
                        mensaje = $"[Evento] La res '{res.Nombre}' ha completado su esquema de vacunación.";
                    }
                    else
                    {
                        mensaje = $"[Evento] La res '{res.Nombre}' aún no ha completado su esquema de vacunación. Bacterianas: {contador_bacterianas}, Vivas: {contador_vivas}";
                    }
                    evt_vacunacion_completada(mensaje);
                }

                return esquema_completo;
            }
            catch (Exception er)
            {
                throw new Exception("[evento] Error inesperado en el metodo Informar_Vacunacion_Completada: " + er.Message);
            }
        }
    }
}
