using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas;

namespace Bib_Hacienda.Clases
{

    //no cumple con el principio de responsabilidad única, ya que la clase Potrero tiene múltiples responsabilidades: gestionar las reses,añadir res, manejar eventos y validar la información de las reses. Sería recomendable separar estas responsabilidades en clases distintas para cumplir con este principio.
    public class Potrero
    {

        //Atributos
        public enum l_tipos_potreros {ternero, novillo, cebon};
        private string identificacion;
        private List<Res> l_reses = new List<Res>();
        private readonly IReglasTipoPotrero reglas_tipo_potrero;

        private readonly INotificacionIncorporacionRes notificacion_incorporacion;

        //EventHandler
        internal void EventHandler() { }

        //Constructor
        public Potrero(
            string identificacion,
            IReglasTipoPotrero reglas_tipo_potrero,
            INotificacionIncorporacionRes notificacion_incorporacion)
        {
            this.Identificacion = identificacion;
            this.reglas_tipo_potrero = reglas_tipo_potrero;
            this.notificacion_incorporacion = notificacion_incorporacion;

        }

        //Metodo para añadir las reces al potrero
        public string anadir_res(string nombre, ushort edad, uint peso) 
        {
            try
            {
                //Validar parámetros
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre de la res no puede estar vacío", nameof(nombre));
                }

                ushort cantidad_reses;

                if (l_reses.Count() == ReglaPotrero.max_reses_potrero)
                {
                    //Validacion de potrero lleno
                    throw new Exception($"La res no puede ser añadida al potrero {this.identificacion} porque este está lleno");
                }
                else
                {
                    reglas_tipo_potrero.ValidarEdad(edad, this.identificacion);
                    Res res = reglas_tipo_potrero.CrearRes(nombre, peso, edad);
                    l_reses.Add(res);

                    //Cuenta las reses actuales en el potrero
                    cantidad_reses = (ushort)L_reses.Count();

                        string mensajes_eventos = notificacion_incorporacion.ObtenerMensajes(
                            this,
                            res,
                            cantidad_reses);
                       
                        //Construir mensaje de retorno
                        string mensaje_final = $"La res {nombre} ha sido añadida al potrero {this.identificacion} con exito.";
                        if (!string.IsNullOrEmpty(mensajes_eventos))
                        {
                            mensaje_final += "\n" + mensajes_eventos.TrimEnd();
                        }

                    return mensaje_final;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inesperado en el metodo anadir_res: " + ex.Message);
            }

        }

        //Metodo para buscar res por el nombre
        public Res buscar_res(string nombre)
        {
            try
            {
                // Validar nombre
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre de búsqueda no puede estar vacío.");
                }

                // Buscar la res que contengan el texto (ignorando mayúsculas/minúsculas)
                var res_encontrada = l_reses
                    .Where(p => p.Nombre.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                // Si no hay resultados
                if (res_encontrada.Count == 0)
                {
                    throw new Exception($"No se encontró ningúna vaca con el nombre o coincidencia '{nombre}'.");
                }

                // Si hay más de un resultado, mostrar opciones
                if (res_encontrada.Count > 1)
                {
                    throw new Exception($" se encontró mas de una res con el nombre o coincidencia '{nombre}'.");
                }

                //  devolver potrero
                return res_encontrada.First();
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método buscar_potrero: " + er.Message);
            }
        }

        //Accesores
        public List<Res> L_reses { get => l_reses; set => l_reses = value; }
        public string Identificacion { get => identificacion; set => identificacion = value; }
        public string Tipo_potrero => reglas_tipo_potrero.TipoPotrero;

    }
}
