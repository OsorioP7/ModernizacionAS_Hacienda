using Bib_Hacienda.Eventos;
using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Bib_Hacienda.Clases.Potrero;

namespace Bib_Hacienda.Clases
{
    public class Hacienda : IVacunacion, IVentaRes, ICreacionVacuna
    {
        //Atributos
        private List<Potrero> l_potreros;
        private List<Venta> l_ventas;
        private List<Vacuna> l_vacunas;

        //Accesores públicos para los servicios (get público, set privado)
        public List<Potrero> L_potreros 
        { 
            get => l_potreros; 
            private set => l_potreros = value; 
        }

        public List<Venta> L_ventas 
        { 
            get => l_ventas; 
            private set => l_ventas = value; 
        }

        public List<Vacuna> L_vacunas 
        { 
            get => l_vacunas; 
            private set => l_vacunas = value; 
        }

        //Eventos
        private PublisherVacunacionCompletada publisher_vacunacion_completa = new PublisherVacunacionCompletada();
        private PublisherVacunaVencida publisher_vacuna_vencida = new PublisherVacunaVencida();
        private PublisherPesoMin publisher_peso_min = new PublisherPesoMin();
        private PublisherPesoVenta publisher_peso_ideal = new PublisherPesoVenta();


        //EventHandler
        internal void EventHandler() { }

        //Constructor vacío
        public Hacienda()
        {
            l_potreros = new List<Potrero>();
            l_ventas = new List<Venta>();
            l_vacunas = new List<Vacuna>();
        }

        //Metodo para crear potreros
        public string crear_potrero(string indentificacion, l_tipos_potreros tipo_potrero)
        {
            try
            {
                //Validar que el nombre no este vacio o nulo
                if (string.IsNullOrWhiteSpace(indentificacion))
                {
                    throw new ArgumentException("El nombre de la res no puede estar vacío", nameof(indentificacion));
                }
                if (l_potreros.Any(p => p.Identificacion.Equals(indentificacion, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"Ya existe un potrero con el nombre '{indentificacion}'.");
                }

                //Crear nuevo potrero

                Potrero nuevo_potrero = new Potrero(indentificacion, tipo_potrero);

                l_potreros.Add(nuevo_potrero);

                return ($"El potrero {indentificacion} se a añadido a la hacienda. ");

            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo crear_potrero: " + er.Message);
            }
        }

        //Metodo para buscar potreros por el nombre
        public Potrero buscar_potrero(string nombre)
        {
            try
            {
                // Validar nombre
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre de búsqueda no puede estar vacío.");
                }

                // Buscar potreros que contengan el texto (ignorando mayúsculas/minúsculas)
                var potreros_encontrados = l_potreros
                    .Where(p => p.Identificacion.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                // Si no hay resultados
                if (potreros_encontrados.Count == 0)
                {
                    throw new Exception($"No se encontró ningún potrero con el nombre o coincidencia '{nombre}'.");
                }

                // Si hay más de un resultado, mostrar opciones
                if (potreros_encontrados.Count > 1)
                {
                    throw new Exception($" se encontró mas de un potrero con el nombre o coincidencia '{nombre}'.");
                }

                //  devolver potrero
                return potreros_encontrados.First();
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método buscar_potrero: " + er.Message);
            }
        }
        
        //Metodo para  anadir res a un potrero 
        public string anadir_res_potrero (string id_potrero, string nombre, ushort edad, uint peso)
        {
            try
            {
                Potrero potrero = buscar_potrero(id_potrero);
                string resultado = potrero.anadir_res(nombre, edad, peso);  // ✅ Capturar el mensaje
                return resultado;  // ✅ Retornar el mensaje del potrero (incluye eventos)
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método anadir_res_potrero: " + er.Message);
            }
        }

        //Metodo para vender res
        public string vender_res(string id_potrero, string nombre, uint monto)
        {

            try
            {
                // Pedimos el potrero y la res
                Potrero potrero = buscar_potrero(id_potrero);
                Res res = potrero.buscar_res(nombre);
                //Validar parámetros
                if (potrero == null) throw new ArgumentNullException(nameof(potrero));
                if (res == null) throw new ArgumentNullException(nameof(res));

                //Crear la venta
                Venta venta = new Venta(potrero, DateTime.Now, res, monto);
                //Agregar la venta a la lista de ventas
                l_ventas.Add(venta);
                //Remover la res del potrero
                l_potreros.Where(p => p == potrero).FirstOrDefault().L_reses.Remove(res);
                return $"Venta de la res {res.Nombre} realizada con exito";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo vender_res: " + er.Message);
            }

        }

        //Metodo para alimentar una res
        public string alimentar_res(string id_potrero, string nombre)
        {
            try
            {
                Potrero potrero = buscar_potrero(id_potrero);
                Res res = potrero.buscar_res(nombre);
                string mensaje_final= "";

                //Validar parámetros
                if (potrero == null) throw new ArgumentNullException(nameof(potrero));
                if (res == null) throw new ArgumentNullException(nameof(res));

                //Alimentar la res (incrementa el peso)
                res.Peso ++;

                string mensaje_eventos = "";

                //Suscribirse a los eventos con lambdas para acumular mensajes
                publisher_peso_min.evt_peso_min += (mensaje) =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                    mensaje_eventos += mensaje + "\n";
                };

                publisher_peso_ideal.evt_peso_venta += (mensaje) =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                    mensaje_eventos += mensaje + "\n";
                };

                //Disparar los eventos con la res actualizada
                publisher_peso_min.Informar_Peso_Min(res);
                publisher_peso_ideal.Informar_Peso_Venta(res);

                //Construir mensaje de retorno
                mensaje_final = $"La res '{res.Nombre}' ha sido alimentada, ahora pesa {res.Peso} kg.";
                if (!string.IsNullOrEmpty(mensaje_eventos))
                {
                    mensaje_final += "\n" + mensaje_eventos.TrimEnd();
                }
                return mensaje_final;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo alimentar_res: " + er.Message);
            }
        }

        //Metodo sobrecargado para alimentar una res con una cantidad de alimento especifica
        public string alimentar_res(string id_potrero, string nombre, uint cantidadAlimento)
        {
            try
            {
                Potrero potrero = buscar_potrero(id_potrero);
                Res res = potrero.buscar_res(nombre);

                //Validar parámetros
                if (potrero == null) throw new ArgumentNullException(nameof(potrero));
                if (res == null) throw new ArgumentNullException(nameof(res));

                res.Peso += cantidadAlimento;

                string mensaje_eventos = "";

                //Suscribirse a los eventos con lambdas para acumular mensajes
                publisher_peso_min.evt_peso_min += (mensaje) =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                        mensaje_eventos += mensaje + "\n";
                };

                publisher_peso_ideal.evt_peso_venta += (mensaje) =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                        mensaje_eventos += mensaje + "\n";
                };

                //Disparar los eventos con la res actualizada
                publisher_peso_min.Informar_Peso_Min(res);
                publisher_peso_ideal.Informar_Peso_Venta(res);

                //Construir mensaje de retorno
                string mensaje_final = $"La res '{res.Nombre}' ha sido alimentada, ahora pesa {res.Peso} kg.";
                if (!string.IsNullOrEmpty(mensaje_eventos))
                {
                    mensaje_final += "\n" + mensaje_eventos.TrimEnd();
                }

                return mensaje_final;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo alimentar_res: " + er.Message);
            }
        }

        //Metodo para crear y añadir vacuna al inventario
        public string crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion)
        {
            try
            {
                //Validar parámetros
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre de la vacuna no puede estar vacío", nameof(nombre));

                if (string.IsNullOrWhiteSpace(lote))
                    throw new ArgumentException("El lote de la vacuna no puede estar vacío", nameof(lote));

                //Validar que la fecha de vencimiento sea posterior a la fecha de aplicación
                if (fecha_vencimiento <= fecha_aplicacion)
                    throw new Exception("La fecha de vencimiento debe ser posterior a la fecha de aplicación");

                //Verificar si ya existe una vacuna con el mismo lote
                if (l_vacunas.Any(v => v.Lote.Equals(lote, StringComparison.OrdinalIgnoreCase)))
                    throw new Exception($"Ya existe una vacuna con el lote '{lote}' en el inventario");

                //Crear vacuna bacteriana
                Bacteriana nueva_vacuna = new Bacteriana(nombre, lote, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion);

                //Agregar al inventario
                l_vacunas.Add(nueva_vacuna);

                return $"Vacuna bacteriana '{nombre}' del lote '{lote}' agregada al inventario con éxito. Período de aplicación: {periodo_aplicacion} semanas.";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (bacteriana): " + er.Message);
            }
        }

        //Metodo para crear vacuna viva individual
        public string crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Viva.enum_l_atenuaciones grado_atenuacion)
        {
            try
            {
                //Validar parámetros
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre de la vacuna no puede estar vacío", nameof(nombre));

                if (string.IsNullOrWhiteSpace(lote))
                    throw new ArgumentException("El lote de la vacuna no puede estar vacío", nameof(lote));

                //Validar que la fecha de vencimiento sea posterior a la fecha de aplicación
                if (fecha_vencimiento <= fecha_aplicacion)
                    throw new Exception("La fecha de vencimiento debe ser posterior a la fecha de aplicación");

                //Verificar si ya existe una vacuna con el mismo lote
                if (l_vacunas.Any(v => v.Lote.Equals(lote, StringComparison.OrdinalIgnoreCase)))
                    throw new Exception($"Ya existe una vacuna con el lote '{lote}' en el inventario");

                //Crear vacuna viva
                Viva nueva_vacuna = new Viva(nombre, lote, fecha_vencimiento, fecha_aplicacion, grado_atenuacion);

                //Agregar al inventario
                l_vacunas.Add(nueva_vacuna);

                return $"Vacuna viva '{nombre}' del lote '{lote}' agregada al inventario con éxito. Grado de atenuación: {(int)grado_atenuacion}.";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (viva): " + er.Message);
            }
        }

        //Metodo para crear lote de vacunas bacterianas
        public string crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion, uint cantidad)
        {
            try
            {
                //Validar cantidad
                if (cantidad <= 0)
                    throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(cantidad));

                if (cantidad > 100)
                    throw new ArgumentException("No se pueden crear más de 100 vacunas en un solo lote", nameof(cantidad));

                //Validar parámetros
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre de la vacuna no puede estar vacío", nameof(nombre));

                if (string.IsNullOrWhiteSpace(lote_base))
                    throw new ArgumentException("El lote base no puede estar vacío", nameof(lote_base));

                //Validar fechas
                if (fecha_vencimiento <= fecha_aplicacion)
                    throw new Exception("La fecha de vencimiento debe ser posterior a la fecha de aplicación");

                int vacunas_creadas = 0;

                //Crear múltiples vacunas con lotes numerados
                for (int i = 1; i <= cantidad; i++)
                {
                    string lote_numerado = $"{lote_base}-{i:D3}";

                    //Verificar si ya existe
                    if (l_vacunas.Any(v => v.Lote.Equals(lote_numerado, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    //Crear vacuna bacteriana
                    Bacteriana nueva_vacuna = new Bacteriana(nombre, lote_numerado, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion);
                    l_vacunas.Add(nueva_vacuna);
                    vacunas_creadas++;
                }

                if (vacunas_creadas == 0)
                    throw new Exception($"No se pudo crear ninguna vacuna. Todos los lotes ya existen en el inventario");

                return $"Lote de vacunas bacterianas creado con éxito:\n" +
                "- Nombre: {nombre}\n" +
                $"- Cantidad creada: {vacunas_creadas} de {cantidad}\n" +
                $"- Lotes: {lote_base}-001 a {lote_base}-{vacunas_creadas:D3}\n" +
                $"- Período de aplicación: {periodo_aplicacion} semanas";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (lote bacteriano): " + er.Message);
            }
        }

        //Metodo para crear lote de vacunas vivas
        public string crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Viva.enum_l_atenuaciones grado_atenuacion, uint cantidad)
        {
            try
            {
                //Validar cantidad
                if (cantidad <= 0)
                    throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(cantidad));

                if (cantidad > 100)
                    throw new ArgumentException("No se pueden crear más de 100 vacunas en un solo lote", nameof(cantidad));

                //Validar parámetros
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre de la vacuna no puede estar vacío", nameof(nombre));

                if (string.IsNullOrWhiteSpace(lote_base))
                    throw new ArgumentException("El lote base no puede estar vacío", nameof(lote_base));

                //Validar fechas
                if (fecha_vencimiento <= fecha_aplicacion)
                    throw new Exception("La fecha de vencimiento debe ser posterior a la fecha de aplicación");

                int vacunas_creadas = 0;

                //Crear múltiples vacunas con lotes numerados
                for (int i = 1; i <= cantidad; i++)
                {
                    string lote_numerado = $"{lote_base}-{i:D3}";

                    //Verificar si ya existe
                    if (l_vacunas.Any(v => v.Lote.Equals(lote_numerado, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    //Crear vacuna viva
                    Viva nueva_vacuna = new Viva(nombre, lote_numerado, fecha_vencimiento, fecha_aplicacion, grado_atenuacion);
                    l_vacunas.Add(nueva_vacuna);
                    vacunas_creadas++;
                }

                if (vacunas_creadas == 0)
                    throw new Exception($"No se pudo crear ninguna vacuna. Todos los lotes ya existen en el inventario");

                return $"Lote de vacunas vivas creado con éxito:\n" +
                $"- Nombre: {nombre}\n" +
                $"- Cantidad creada: {vacunas_creadas} de {cantidad}\n" +
                $"- Lotes: {lote_base}-001 a {lote_base}-{vacunas_creadas:D3}\n" +
                $"- Grado de atenuación: {(int)grado_atenuacion}";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (lote vivo): " + er.Message);
            }
        }


        //Metodo para aplicar vacuna
        public string aplicar_vacuna(Vacuna vacuna, string nombre,string id_potrero)
        {
            try
            {
                string mensaje_vacuna = "";
                string mensaje_vacunacion = "";
                Potrero potrero = buscar_potrero(id_potrero);
                Res res = potrero.buscar_res( nombre);
                //Contadores de vacunas aplicadas
                byte contador_bacterianas = 0;
                byte contador_vivas = 0;   
                byte max_bac = 0;
                byte max_viv = 0;

                //Validar parámetros
                if (vacuna == null) throw new ArgumentNullException(nameof(vacuna));
                if (res == null) throw new ArgumentNullException(nameof(res));

                // Validar si la vacuna ya fue aplicada (por nombre o lote)
                if (res.L_vacunas_aplicadas.Any(v => v.Nombre == vacuna.Nombre || v.Lote == vacuna.Lote))
                    throw new Exception($"La vacuna '{vacuna.Nombre}' ya fue aplicada a la res '{res.Nombre}'.");

                //Contar las vacunas ya aplicadas a la res
                foreach (Vacuna vac in res.L_vacunas_aplicadas)
                {
                    if (vac is Bacteriana)
                    {
                        contador_bacterianas++;
                    }
                    else if (vac is Viva)
                    {
                        contador_vivas++;
                    }
                }

                //Determinar el maximo segun el tipo de res 
                if (res is Ternero)
                {
                    max_bac = ReglaVacuna.max_bac_ternero; // 3
                    max_viv = ReglaVacuna.max_viv_ternero; // 1
                }
                else if (res is Novillo)
                {
                    max_bac = ReglaVacuna.max_bac_novillo; // 2
                    max_viv = ReglaVacuna.max_viv_novillo; // 2
                }
                else if (res is Cebon)
                {
                    max_bac = ReglaVacuna.max_bac_cebon; // 1
                    max_viv = ReglaVacuna.max_viv_cebon; // 4
                }

                // Validar límites antes de aplicar
                if (vacuna is Bacteriana && contador_bacterianas >= max_bac)
                    throw new Exception($"No se puede aplicar más vacunas bacterianas a la res '{res.Nombre}'. Ya tiene las {max_bac} permitidas.");

                if (vacuna is Viva && contador_vivas >= max_viv)
                    throw new Exception($"No se puede aplicar más vacunas vivas a la res '{res.Nombre}'. Ya tiene las {max_viv} permitidas.");


                //Suscribirse al evento con una lambda para capturar el mensaje
                
                publisher_vacuna_vencida.evt_vacuna_vencida += (mensaje) =>
                {
                    mensaje_vacuna = mensaje;
                };

                //Validar fecha de vencimiento de la vacuna
                bool vacuna_vencida = publisher_vacuna_vencida.Informar_Vacuna_Vencida(vacuna);

                if (vacuna_vencida)
                {
                    throw new Exception(mensaje_vacuna);
                }
                else
                {
                    res.L_vacunas_aplicadas.Add(vacuna);
                    l_vacunas.Remove(vacuna);

                    //Actualizar contadores
                    if (vacuna is Bacteriana)
                    {
                        contador_bacterianas++;
                    }
                    else if (vacuna is Viva)
                    {
                        contador_vivas++;
                    }

                    //Suscribirse al evento de vacunación completada
                    publisher_vacunacion_completa.evt_vacunacion_completada += (mensaje) =>
                    {
                        mensaje_vacunacion = mensaje;
                    };

                    //Disparar evento de vacunacion completa
                    bool esquema_completo = publisher_vacunacion_completa.Informar_Vacunacion_Completada(res, contador_bacterianas, contador_vivas);

                    return $"Vacuna aplicada correctamente a la res {res.Nombre}. {mensaje_vacunacion}";
                }

            }
            catch (Exception err)
            {
                throw new Exception("Error inesperado en el metodo aplicar_vacuna: " + err.Message);
            }
        }
    }
}