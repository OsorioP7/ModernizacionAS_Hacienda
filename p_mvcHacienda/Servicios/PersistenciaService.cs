using Bib_Hacienda.Aspectos;
using Bib_Hacienda.Clases;
using Bib_Hacienda.Clases.Validaciones;
using Castle.DynamicProxy;
using static Bib_Hacienda.Clases.Potrero;
using static Bib_Hacienda.Clases.Viva;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;

namespace p_mvcHacienda.Servicios
{
    public class PersistenciaService
    {
        // Atributos
        private readonly string _directorioArchivos;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private InterceptorValidarInformacion? _interceptorValidacion;

        // Validadores con proxy (interceptados) - Lazy initialization
        private ValidadorVacuna? _validadorVacunaProxy;
        private ValidadorPotrero? _validadorPotreroProxy;
        private ValidadorRes? _validadorResProxy;
        private ValidadorVenta? _validadorVentaProxy;

        // Constructor - NO recibe Hacienda ni crea proxies aquí
        public PersistenciaService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            // Usar la raíz de contenido de la aplicación para resolver la carpeta Datos
            _directorioArchivos = Path.Combine(env.ContentRootPath, "Datos");

            if (!Directory.Exists(_directorioArchivos))
            {
                Directory.CreateDirectory(_directorioArchivos);
            }

            _httpContextAccessor = httpContextAccessor;
            // NO inicializar interceptor aquí - se hará cuando sea necesario
        }

        // Crear proxies solo cuando se necesiten (lazy + thread-safe)
        private void InicializarProxies()
        {
            if (_validadorVacunaProxy == null)
            {
                // Crear el interceptor aquí, cuando hay HttpContext disponible
                if (_interceptorValidacion == null)
                {
                    _interceptorValidacion = new InterceptorValidarInformacion(_httpContextAccessor);
                }

                var proxyGenerator = new ProxyGenerator();
                _validadorVacunaProxy = proxyGenerator.CreateClassProxy<ValidadorVacuna>(_interceptorValidacion);
                _validadorPotreroProxy = proxyGenerator.CreateClassProxy<ValidadorPotrero>(_interceptorValidacion);
                _validadorResProxy = proxyGenerator.CreateClassProxy<ValidadorRes>(_interceptorValidacion);
                _validadorVentaProxy = proxyGenerator.CreateClassProxy<ValidadorVenta>(_interceptorValidacion);
            }
        }

        #region Guardar Datos

        // Guardar potreros con validación
        public string GuardarPotreros(List<Potrero> potreros)
        {
            try
            {
                InicializarProxies(); // Crear proxies solo cuando se guardan datos
                bool esValido;

                // Validar usando el PROXY (esto activa el interceptor)
                foreach (var potrero in potreros)
                {
                    esValido = _validadorPotreroProxy!.ValidarPotrero(potrero);

                    if (!esValido)
                    {
                        var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                        return mensaje ?? "Error de validación en potrero";
                    }
                }

                // Serializar y guardar
                var lineas = potreros.Select(p => $"{p.Identificacion}|{p.Tipo_potrero}");
                File.WriteAllLines(Path.Combine(_directorioArchivos, "Potreros.txt"), lineas);

                return _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString()
                    ?? "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar potreros: {ex.Message}", ex);
            }
        }

        // Guardar reses con validación
        public string GuardarReses(List<Potrero> potreros)
        {
            try
            {
                InicializarProxies();
                var lineas = new List<string>();
                bool esValida;

                foreach (var potrero in potreros)
                {
                    foreach (var res in potrero.L_reses)
                    {
                        // Validar usando el PROXY
                        esValida = _validadorResProxy!.ValidarRes(res);

                        if (!esValida)
                        {
                            var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                            return mensaje ?? "Error de validación en res";
                        }

                        string tipoRes = res.GetType().Name;
                        lineas.Add($"{potrero.Identificacion}|{res.Nombre}|{res.Peso}|{res.Edad}|{tipoRes}");
                    }
                }

                File.WriteAllLines(Path.Combine(_directorioArchivos, "Reses.txt"), lineas);

                return _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString()
                    ?? "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar reses: {ex.Message}", ex);
            }
        }

        // Guardar ventas con validación
        public string GuardarVentas(List<Venta> ventas)
        {
            try
            {
                InicializarProxies();
                bool esValida;
                string fecha;
                string tipoRes;

                // Validar usando el PROXY
                foreach (var venta in ventas)
                {
                    esValida = _validadorVentaProxy!.ValidarVenta(venta);

                    if (!esValida)
                    {
                        var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                        return mensaje ?? "Error de validación en venta";
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

                File.WriteAllLines(Path.Combine(_directorioArchivos, "Ventas.txt"), lineas);

                return _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString()
                    ?? "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar ventas: {ex.Message}", ex);
            }
        }

        // Guardar vacunas con validación
        public string GuardarVacunas(List<Vacuna> vacunas)
        {
            try
            {
                InicializarProxies();
                bool esValida;

                // Validar usando el PROXY
                foreach (var vacuna in vacunas)
                {
                    esValida = _validadorVacunaProxy!.ValidarVacuna(vacuna);

                    if (!esValida)
                    {
                        var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                        return mensaje ?? "Error de validación en vacuna";
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

                File.WriteAllLines(Path.Combine(_directorioArchivos, "Vacunas.txt"), lineas);

                return _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString()
                    ?? "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar vacunas: {ex.Message}", ex);
            }
        }

        // Guardar vacunas aplicadas con validación
        public string GuardarVacunasAplicadas(List<Potrero> potreros)
        {
            try
            {
                InicializarProxies();
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
                        resValida = _validadorResProxy!.ValidarRes(res);
                        if (!resValida)
                        {
                            var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                            return mensaje ?? "Error de validación en res";
                        }

                        foreach (var vacuna in res.L_vacunas_aplicadas)
                        {
                            // Validar vacuna
                            bool vacunaValida = _validadorVacunaProxy!.ValidarVacuna(vacuna);
                            if (!vacunaValida)
                            {
                                var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                                return mensaje ?? "Error de validación en vacuna aplicada";
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

                File.WriteAllLines(Path.Combine(_directorioArchivos, "VacunasAplicadas.txt"), lineas);

                return _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString()
                    ?? "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar vacunas aplicadas: {ex.Message}", ex);
            }
        }

        // Guardar usuarios (validación simple, sin proxies)
        public string GuardarUsuarios(List<Usuario> usuarios)
        {
            try
            {
                // Validación simple para usuarios (sin proxy por ahora)
                foreach (var usuario in usuarios)
                {
                    if (string.IsNullOrWhiteSpace(usuario.Nombre) || string.IsNullOrWhiteSpace(usuario.Contrasena))
                    {
                        return "Error: Usuario debe tener nombre y contraseña";
                    }
                }

                var lineas = usuarios.Select(u => $"{u.Nombre}|{u.Contrasena}");
                File.WriteAllLines(Path.Combine(_directorioArchivos, "Usuarios.txt"), lineas);

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
                string rutaArchivo = Path.Combine(_directorioArchivos, "Potreros.txt");
                string identificacion;

                if (!File.Exists(rutaArchivo))
                {
                    return new List<Potrero>();
                }

                var potreros = new List<Potrero>();
                var lineas = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >=2)
                    {
                        identificacion = partes[0].Trim(); // normalizar
                        l_tipos_potreros tipo = Enum.Parse<l_tipos_potreros>(partes[1]);

                        // evitar duplicados por identificacion (case-insensitive)
                        if (!potreros.Any(p => string.Equals(p.Identificacion, identificacion, StringComparison.OrdinalIgnoreCase)))
                        {
                            potreros.Add(new Potrero(identificacion, tipo));
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
                string rutaArchivo = Path.Combine(_directorioArchivos, "Reses.txt");
                string nombreRes;
                string nombrePotrero;
                uint peso;
                ushort edad;

                if (!File.Exists(rutaArchivo))
                {
                    return;
                }

                var lineas = File.ReadAllLines(rutaArchivo);

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
                            potrero.anadir_res(nombreRes, edad, peso);
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
                string rutaArchivo = Path.Combine(_directorioArchivos, "Ventas.txt");
                string potreroId;
                DateTime fecha;
                string resNombre;
                uint resPeso;
                ushort resEdad;
                string resTipo;
                uint monto;

                if (!File.Exists(rutaArchivo))
                {
                    return new List<Venta>();
                }

                var ventas = new List<Venta>();
                var lineas = File.ReadAllLines(rutaArchivo);

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
                            potrero = new Potrero(potreroId, l_tipos_potreros.ternero);
                        }

                        Res res = resTipo switch
                        {
                            "Ternero" => new Ternero(resNombre, resPeso, resEdad),
                            "Novillo" => new Novillo(resNombre, resPeso, resEdad),
                            "Cebon" => new Cebon(resNombre, resPeso, resEdad),
                            _ => new Ternero(resNombre, resPeso, resEdad)
                        };

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
                string rutaArchivo = Path.Combine(_directorioArchivos, "Vacunas.txt");
                string nombre;
                string lote;
                DateTime fechaVenc;
                DateTime fechaAplic;
                string tipo;
                uint periodo;

                if (!File.Exists(rutaArchivo))
                {
                    return new List<Vacuna>();
                }

                var vacunas = new List<Vacuna>();
                var lineas = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >=6)
                    {
                        nombre = partes[0];
                        lote = partes[1];
                        if (!DateTime.TryParseExact(partes[2].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaVenc))
                        {
                            continue;
                        }
                        if (!DateTime.TryParseExact(partes[3].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaAplic))
                        {
                            continue;
                        }
                        tipo = partes[4].Trim();
                        periodo = uint.TryParse(partes[5].Trim(), out var per) ? per :0u;

                        Vacuna vacuna;
                        if (tipo.Equals("Bacteriana", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!uint.TryParse(partes[5].Trim(), out periodo) || periodo < 2 || periodo > 4)
                            {
                                // TODO: loggear línea inválida
                                continue; // omitir y seguir
                            }
                            try
                            {
                                vacuna = new Bacteriana(nombre, lote, fechaVenc, fechaAplic, periodo);
                            }
                            catch
                            {
                                continue; // por si el constructor valida otras reglas
                            }
                        }
                        else
                        {
                            vacuna = new Viva(nombre, lote, fechaVenc, fechaAplic, enum_l_atenuaciones.Atenuacion10);
                        }

                        vacunas.Add(vacuna);
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
                string rutaArchivo = Path.Combine(_directorioArchivos, "VacunasAplicadas.txt");
                string nombrePotrero;
                string nombreRes;
                string nombreVacuna;
                string lote;
                DateTime fechaVenc;
                DateTime fechaAplic;
                string tipo;
                uint periodo;

                if (!File.Exists(rutaArchivo))
                {
                    return;
                }

                var lineas = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >=8)
                    {
                        nombrePotrero = partes[0].Trim();
                        nombreRes = partes[1];
                        nombreVacuna = partes[2];
                        lote = partes[3];
                        if (!DateTime.TryParseExact(partes[4].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaVenc))
                        {
                            continue;
                        }
                        if (!DateTime.TryParseExact(partes[5].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaAplic))
                        {
                            continue;
                        }
                        tipo = partes[6];
                        periodo = uint.TryParse(partes[7].Trim(), out var per) ? per :0u;

                        var potrero = potreros.FirstOrDefault(p => string.Equals(p.Identificacion, nombrePotrero, StringComparison.OrdinalIgnoreCase));
                        if (potrero != null)
                        {
                            var res = potrero.buscar_res(nombreRes);
                            if (res != null)
                            {
                                Vacuna vacuna;
                                if (tipo == "Bacteriana")
                                {
                                    vacuna = new Bacteriana(nombreVacuna, lote, fechaVenc, fechaAplic, periodo);
                                }
                                else
                                {
                                    vacuna = new Viva(nombreVacuna, lote, fechaVenc, fechaAplic, enum_l_atenuaciones.Atenuacion10);
                                }

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
                string rutaArchivo = Path.Combine(_directorioArchivos, "Usuarios.txt");
                string nombre;
                string contrasena;

                if (!File.Exists(rutaArchivo))
                {
                    return new List<Usuario>();
                }

                var usuarios = new List<Usuario>();
                var lineas = File.ReadAllLines(rutaArchivo);

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

