using Bib_Hacienda.Clases;
using Bib_Hacienda.Clases.Validaciones;
using System.Globalization;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class PersistenciaService : IActualizacionPotreros, IActualizacionReses, IActualizacionVentas, ICargaUsuarios, IGuardadoUsuarios, ICargaVacunas, IActualizacionInventarioVacunas, IActualizacionHistorialVacunacion
    {
        // Atributos
        private readonly IAlmacenamientoHacienda _almacenamiento;
        private readonly IReadOnlyDictionary<string, IReglasTipoPotrero> _reglasPorTipo;
        private readonly ICargaVacuna _cargaVacuna;
        private readonly INotificacionIncorporacionRes _notificacionIncorporacion;
        private readonly IValidadoresGuardado _validadores;
        private readonly IResultadoValidacion _resultadoValidacion;
        private readonly IValidador<Usuario> _validadorDatosRequeridosUsuario;

        // Constructor - NO recibe Hacienda ni crea proxies aqu�
public PersistenciaService(
            IAlmacenamientoHacienda almacenamiento,
            IReadOnlyDictionary<string, IReglasTipoPotrero> reglasPorTipo,
            ICargaVacuna cargaVacuna,
            INotificacionIncorporacionRes notificacionIncorporacion,
            IValidadoresGuardado validadores,
            IResultadoValidacion resultadoValidacion,
            IValidador<Usuario> validadorDatosRequeridosUsuario)
        {
            // Usar la ra�z de contenido de la aplicaci�n para resolver la carpeta Datos
            _almacenamiento = almacenamiento;
            _reglasPorTipo = reglasPorTipo;
            _cargaVacuna = cargaVacuna;
            _notificacionIncorporacion = notificacionIncorporacion;
            _validadores = validadores;
            _resultadoValidacion = resultadoValidacion;
            _validadorDatosRequeridosUsuario = validadorDatosRequeridosUsuario;
            // NO inicializar interceptor aqu� - se har� cuando sea necesario
        }

        #region Guardar Datos

        public string ActualizarPotreros(List<Potrero> potreros)
        {
            return GuardarPotreros(potreros);
        }

        public string ActualizarReses(List<Potrero> potreros)
        {
            return GuardarReses(potreros);
        }

        public string ActualizarVentas(List<Venta> ventas)
        {
            return GuardarVentas(ventas);
        }

        // Guardar potreros con validaci�n
        public string GuardarPotreros(List<Potrero> potreros)
        {
            try
            {
                bool esValido;

                // Validar usando el PROXY (esto activa el interceptor)
                foreach (var potrero in potreros)
                {
                    esValido = _validadores.ValidadorPotrero.Validar(potrero);

                    if (!esValido)
                    {
                        return _resultadoValidacion.Obtener("Error de validaci�n en potrero");
                    }
                }

                // Serializar y guardar
                var lineas = potreros.Select(p => $"{p.Identificacion}|{p.Tipo_potrero}");
                _almacenamiento.Guardar("Potreros", lineas);

                return _resultadoValidacion.Obtener("Guardado exitosamente");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar potreros: {ex.Message}", ex);
            }
        }

        // Guardar reses con validaci�n
        public string GuardarReses(List<Potrero> potreros)
        {
            try
            {
                var lineas = new List<string>();
                bool esValida;

                foreach (var potrero in potreros)
                {
                    foreach (var res in potrero.L_reses)
                    {
                        // Validar usando el PROXY
                        esValida = _validadores.ValidadorRes.Validar(res);

                        if (!esValida)
                        {
                        return _resultadoValidacion.Obtener("Error de validaci�n en res");
                        }

                        string tipoRes = res.GetType().Name;
                        lineas.Add($"{potrero.Identificacion}|{res.Nombre}|{res.Peso}|{res.Edad}|{tipoRes}");
                    }
                }

                _almacenamiento.Guardar("Reses", lineas);

                return _resultadoValidacion.Obtener("Guardado exitosamente");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar reses: {ex.Message}", ex);
            }
        }

        // Guardar ventas con validaci�n
        public string GuardarVentas(List<Venta> ventas)
        {
            try
            {
                bool esValida;
                string fecha;
                string tipoRes;

                // Validar usando el PROXY
                foreach (var venta in ventas)
                {
                    esValida = _validadores.ValidadorVenta.Validar(venta);

                    if (!esValida)
                    {
                        return _resultadoValidacion.Obtener("Error de validaci�n en venta");
                    }
                }

                // Serializar y guardar
                var lineas = new List<string>();
                foreach (var venta in ventas)
                {
                    fecha = venta.Fecha.ToString("yyyy-MM-dd");
                    tipoRes = venta.Res.GetType().Name;
                    lineas.Add($"{venta.Potrero.Identificacion}|{fecha}|{venta.Res.Nombre}|{venta.Res.Peso}|{venta.Res.Edad}|{tipoRes}|{venta.Monto}");
                }

                _almacenamiento.Guardar("Ventas", lineas);

                return _resultadoValidacion.Obtener("Guardado exitosamente");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar ventas: {ex.Message}", ex);
            }
        }

        // Guardar vacunas con validaci�n
        public string GuardarVacunas(List<Vacuna> vacunas)
        {
            try
            {
                bool esValida;

                // Validar usando el PROXY
                foreach (var vacuna in vacunas)
                {
                    esValida = _validadores.ValidadorVacuna.Validar(vacuna);

                    if (!esValida)
                    {
                        return _resultadoValidacion.Obtener("Error de validaci�n en vacuna");
                    }
                }

                // Serializar y guardar
                var lineas = new List<string>();
                foreach (var vacuna in vacunas)
                {
                    string fechaVenc = vacuna.Fecha_vencimiento.ToString("yyyy-MM-dd");
                    string fechaAplic = vacuna.Fecha_aplicacion.ToString("yyyy-MM-dd");
                    string tipo = vacuna.GetType().Name;
                    uint periodo = vacuna is Bacteriana bacteriana ? bacteriana.Periodo_aplicacion :0;

                    lineas.Add($"{vacuna.Nombre}|{vacuna.Lote}|{fechaVenc}|{fechaAplic}|{tipo}|{periodo}");
                }

                _almacenamiento.Guardar("Vacunas", lineas);

                return _resultadoValidacion.Obtener("Guardado exitosamente");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar vacunas: {ex.Message}", ex);
            }
        }

        // Guardar vacunas aplicadas con validaci�n
        public string GuardarVacunasAplicadas(List<Potrero> potreros)
        {
            try
            {
                var lineas = new List<string>();
                bool resValida;
                string fechaVenc;
                string fechaAplic;
                string tipo;
                uint periodo;

                foreach (var potrero in potreros)
                {
                    foreach (var res in potrero.L_reses)
                    {
                        // Validar res
                        resValida = _validadores.ValidadorRes.Validar(res);
                        if (!resValida)
                        {
                        return _resultadoValidacion.Obtener("Error de validaci�n en res");
                        }

                        foreach (var vacuna in res.L_vacunas_aplicadas)
                        {
                            // Validar vacuna
                            bool vacunaValida = _validadores.ValidadorVacuna.Validar(vacuna);
                            if (!vacunaValida)
                            {
                        return _resultadoValidacion.Obtener("Error de validaci�n en vacuna aplicada");
                            }

                            // Serializar vacuna aplicada
                            fechaVenc = vacuna.Fecha_vencimiento.ToString("yyyy-MM-dd");
                            fechaAplic = vacuna.Fecha_aplicacion.ToString("yyyy-MM-dd");
                            tipo = vacuna.GetType().Name;
                            periodo = vacuna is Bacteriana bacteriana ? bacteriana.Periodo_aplicacion :0;

                            lineas.Add($"{potrero.Identificacion}|{res.Nombre}|{vacuna.Nombre}|{vacuna.Lote}|{fechaVenc}|{fechaAplic}|{tipo}|{periodo}");
                        }
                    }
                }

                _almacenamiento.Guardar("VacunasAplicadas", lineas);

                return _resultadoValidacion.Obtener("Guardado exitosamente");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar vacunas aplicadas: {ex.Message}", ex);
            }
        }

        // Guardar usuarios (validaci�n simple, sin proxies)
        public string GuardarUsuarios(List<Usuario> usuarios)
        {
            try
            {
                // Validaci�n simple para usuarios (sin proxy por ahora)
                foreach (var usuario in usuarios)
                {
                    if (!_validadorDatosRequeridosUsuario.Validar(usuario))
                    {
                        return "Error: Usuario debe tener nombre y contrase�a";
                    }
                }

                var lineas = usuarios.Select(u => $"{u.Nombre}|{u.Contrasena}");
                _almacenamiento.Guardar("Usuarios", lineas);

                return "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar usuarios: {ex.Message}", ex);
            }
        }

        #endregion

        #region Cargar Datos

        // Cargar potreros desde archivo (normaliza identificaciones y evita duplicados)
        public List<Potrero> CargarPotreros()
        {
            try
            {
                string identificacion;

                if (!_almacenamiento.Existe("Potreros"))
                {
                    return new List<Potrero>();
                }

                var potreros = new List<Potrero>();
                var lineas = _almacenamiento.Cargar("Potreros");

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >=2)
                    {
                        identificacion = partes[0].Trim(); // normalizar
                        string tipo = partes[1].Trim();
                        // evitar duplicados por identificacion (case-insensitive)
                        if (!potreros.Any(p => string.Equals(p.Identificacion, identificacion, StringComparison.OrdinalIgnoreCase)))
                        {
                            potreros.Add(new Potrero(identificacion, _reglasPorTipo[tipo], _notificacionIncorporacion));
                        }
                    }
                }

                return potreros;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar potreros: {ex.Message}");
            }
        }

        // Cargar reses y asociarlas a los potreros
        public void CargarReses(List<Potrero> potreros)
        {
            try
            {
                string nombreRes;
                string nombrePotrero;
                uint peso;
                ushort edad;

                if (!_almacenamiento.Existe("Reses"))
                {
                    return;
                }

                var lineas = _almacenamiento.Cargar("Reses");

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >=5)
                    {
                        nombrePotrero = partes[0].Trim();
                        nombreRes = partes[1];
                        peso = uint.Parse(partes[2]);
                        edad = ushort.Parse(partes[3]);

                        var potrero = potreros.FirstOrDefault(p => string.Equals(p.Identificacion, nombrePotrero, StringComparison.OrdinalIgnoreCase));
                        if (potrero != null)
                        {
                            var regla = _reglasPorTipo[potrero.Tipo_potrero];
                            regla.ValidarEdad(edad, potrero.Identificacion);
                            potrero.L_reses.Add(regla.CrearRes(nombreRes, peso, edad));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar reses: {ex.Message}");
            }
        }

        public List<Venta> CargarVentas(List<Potrero> potreros)
        {
            try
            {
                string potreroId;
                DateTime fecha;
                string resNombre;
                uint resPeso;
                ushort resEdad;
                string resTipo;
                uint monto;

                if (!_almacenamiento.Existe("Ventas"))
                {
                    return new List<Venta>();
                }

                var ventas = new List<Venta>();
                var lineas = _almacenamiento.Cargar("Ventas");

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >=7)
                    {
                        potreroId = partes[0].Trim();
                        if (!DateTime.TryParseExact(partes[1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            continue;
                        }
                        resNombre = partes[2];
                        resPeso = uint.Parse(partes[3]);
                        resEdad = ushort.Parse(partes[4]);
                        resTipo = partes[5];
                        monto = uint.Parse(partes[6]);

                        var potrero = potreros.FirstOrDefault(p => string.Equals(p.Identificacion, potreroId, StringComparison.OrdinalIgnoreCase));
                        if (potrero == null)
                        {
                            potrero = new Potrero(potreroId, _reglasPorTipo["ternero"], _notificacionIncorporacion);
                        }

                        IReglasTipoPotrero reglaTipoRes = resTipo switch
                        {
                            "Ternero" => _reglasPorTipo["ternero"],
                            "Novillo" => _reglasPorTipo["novillo"],
                            "Cebon" => _reglasPorTipo["cebon"],
                            _ => _reglasPorTipo["ternero"]
                        };

                        Res res = reglaTipoRes.CrearRes(resNombre, resPeso, resEdad);

                        ventas.Add(new Venta(potrero, fecha, res, monto));
                    }
                }

                return ventas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar ventas: {ex.Message}");
            }
        }

        // Cargar vacunas disponibles
        public List<Vacuna> CargarVacunas()
        {
            try
            {
                if (!_almacenamiento.Existe("Vacunas"))
                {
                    return new List<Vacuna>();
                }

                var vacunas = new List<Vacuna>();
                var lineas = _almacenamiento.Cargar("Vacunas");

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >=6)
                    {
                        string[] camposVacuna =
                        {
                            partes[0], partes[1], partes[2],
                            partes[3], partes[4], partes[5]
                        };
                        Vacuna vacuna = _cargaVacuna.Cargar(camposVacuna);
                        if (vacuna != null)
                        {
                            vacunas.Add(vacuna);
                        }
                    }
                }

                return vacunas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar vacunas: {ex.Message}");
            }
        }

        // Cargar vacunas aplicadas por res
        public void CargarVacunasAplicadas(List<Potrero> potreros)
        {
            try
            {
                string nombrePotrero;
                string nombreRes;
                DateTime fechaVenc;
                DateTime fechaAplic;

                if (!_almacenamiento.Existe("VacunasAplicadas"))
                {
                    return;
                }

                var lineas = _almacenamiento.Cargar("VacunasAplicadas");

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >=8)
                    {
                        nombrePotrero = partes[0].Trim();
                        nombreRes = partes[1];
                        if (!DateTime.TryParseExact(partes[4].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaVenc))
                        {
                            continue;
                        }
                        if (!DateTime.TryParseExact(partes[5].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaAplic))
                        {
                            continue;
                        }
                        var potrero = potreros.FirstOrDefault(p => string.Equals(p.Identificacion, nombrePotrero, StringComparison.OrdinalIgnoreCase));
                        if (potrero != null)
                        {
                            var res = potrero.buscar_res(nombreRes);
                            if (res != null)
                            {
                                string[] camposVacuna = new string[8];
                                Array.Copy(partes, camposVacuna, 8);
                                Vacuna vacuna = _cargaVacuna.Cargar(camposVacuna);
                                res.L_vacunas_aplicadas.Add(vacuna);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar vacunas aplicadas: {ex.Message}");
            }
        }

        // Cargar usuarios desde archivo
        public List<Usuario> CargarUsuarios()
        {
            try
            {
                string nombre;
                string contrasena;

                if (!_almacenamiento.Existe("Usuarios"))
                {
                    return new List<Usuario>();
                }

                var usuarios = new List<Usuario>();
                var lineas = _almacenamiento.Cargar("Usuarios");

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >=2)
                    {
                        nombre = partes[0];
                        contrasena = partes[1];
                        usuarios.Add(new Usuario(nombre, contrasena));
                    }
                }

                return usuarios;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar usuarios: {ex.Message}");
                return new List<Usuario>();
            }
        }

        #endregion
    }
}

