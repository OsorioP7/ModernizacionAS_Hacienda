using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Eventos;
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
        private l_tipos_potreros tipo_potrero;

        //Eventos
        // no cumple
        private PublisherPotreroMitad publisher_potrero_mitad = new PublisherPotreroMitad();
        private PublisherPotreroLleno publisher_potrero_lleno = new PublisherPotreroLleno();
        private PublisherPesoVenta publisher_peso_venta = new PublisherPesoVenta();
        private PublisherPesoMin publisher_peso_min = new PublisherPesoMin();

        //EventHandler
        internal void EventHandler() { }

        //Constructor
        public Potrero(string identificacion, l_tipos_potreros tipo_potrero)
        {
            this.Identificacion = identificacion;
            this.tipo_potrero = tipo_potrero;

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

                //variables locales
                byte edad_min_potrero = 0;
                byte edad_max_potrero = 255;
                string tipo_vaca = "";
                ushort cantidad_reses;
                Res res = null;

                if (l_reses.Count() == ReglaPotrero.max_reses_potrero)
                {
                    //Validacion de potrero lleno
                    throw new Exception($"La res no puede ser añadida al potrero {this.identificacion} porque este está lleno");
                }
                else
                {
                    switch (tipo_potrero)
                    {

                        //Definir rangos de edad segun el tipo de potrero
                        case l_tipos_potreros.ternero:
                            edad_max_potrero = ReglaRes.edad_max_ternero; // 12
                            tipo_vaca = "ternero";
                            break;
                        
                        case l_tipos_potreros.cebon:
                            edad_min_potrero = ReglaRes.edad_max_ternero;
                            edad_min_potrero++; // 13
                            edad_max_potrero = ReglaRes.edad_max_cebon; // 48
                            tipo_vaca = "cebon";
                            break;
                        
                        case l_tipos_potreros.novillo:
                            edad_min_potrero = ReglaRes.edad_max_cebon;
                            edad_min_potrero++; // 49
                            tipo_vaca = "novillo";
                            break;
                    }

                    //Validar que la edad de la res esté dentro del rango permitido para el potrero
                    if (edad >= edad_min_potrero && edad <= edad_max_potrero)
                    {
                        switch (tipo_vaca)
                        {
                            case "ternero":
                                res = new Ternero(nombre, peso, edad);
                                l_reses.Add(res);
                                break;
                            case "cebon":
                                res = new Cebon(nombre, peso, edad);
                                l_reses.Add(res);
                                break;
                            case "novillo":
                                res = new Novillo(nombre, peso, edad);
                                l_reses.Add(res);
                                break;
                        }

                        //Cuenta las reses actuales en el potrero
                        cantidad_reses = (ushort)L_reses.Count();

                        string mensajes_eventos = "";

                        //Suscribirse a los eventos ANTES de dispararlos
                        publisher_peso_venta.evt_peso_venta += mensaje =>
                        {
                            if (!string.IsNullOrEmpty(mensaje))
                                mensajes_eventos += mensaje + "\n";
                        };

                        publisher_peso_min.evt_peso_min += mensaje =>
                        {
                            if (!string.IsNullOrEmpty(mensaje))
                                mensajes_eventos += mensaje + "\n";
                        };

                        publisher_potrero_mitad.evt_potrero_mitad += mensaje =>
                        {
                            if (!string.IsNullOrEmpty(mensaje))
                                mensajes_eventos += mensaje + "\n";
                        };

                        publisher_potrero_lleno.evt_potrero_lleno += mensaje =>
                        {
                            if (!string.IsNullOrEmpty(mensaje))
                            mensajes_eventos += mensaje + "\n";
                        };

                        //AHORA SÍ disparar los eventos (después de suscribnos)
                        publisher_potrero_mitad.Informar_Potrero_Mitad(cantidad_reses, this);
                        publisher_potrero_lleno.Informar_Potrero_Lleno(cantidad_reses, this);
                        publisher_peso_min.Informar_Peso_Min(res);
                        publisher_peso_venta.Informar_Peso_Venta(res);
                       
                        //Construir mensaje de retorno
                        string mensaje_final = $"La res {nombre} ha sido añadida al potrero {this.identificacion} con exito.";
                        if (!string.IsNullOrEmpty(mensajes_eventos))
                        {
                            mensaje_final += "\n" + mensajes_eventos.TrimEnd();
                        }

                        return mensaje_final;

                    }
                    else
                    {
                        throw new Exception($"La res no puede ser añadida al potrero {this.identificacion} porque su edad no corresponde al tipo de potrero");
                    }
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
        public l_tipos_potreros Tipo_potrero { get => tipo_potrero; set => tipo_potrero = value; }

    }
}
