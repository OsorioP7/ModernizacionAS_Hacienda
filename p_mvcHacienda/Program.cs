using Bib_Hacienda.Aspectos;
using Bib_Hacienda.Clases;
using Bib_Hacienda.Eventos;
using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas;
using Bib_Hacienda.Reglas.ReglasEdadRes;
using Bib_Hacienda.Reglas.ReglasCargaVacuna;
using Bib_Hacienda.Reglas.ReglasPesoRes;
using Bib_Hacienda.Reglas.ReglasTipoPotrero;
using p_mvcHacienda.Servicios;

namespace p_mvcHacienda
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // --- Configuración de Autenticación por Cookies ---
            builder.Services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", options =>
                {
                    options.Cookie.Name = "HaciendaSoft.Auth";
                    options.LoginPath = "/Account/Login"; // Página de login
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Duración de la sesión
                });

            // Agregar HttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IAlmacenamientoHacienda, AlmacenamientoArchivosHacienda>();
            builder.Services.AddSingleton<IValidadoresGuardado, ValidadoresPersistenciaInterceptados>();
            builder.Services.AddSingleton<IResultadoValidacion, ResultadoValidacionHttpContext>();
            builder.Services.AddSingleton<IValidador<Usuario>, ValidadorDatosRequeridosUsuario>();

            var creadoresProductosDerivados = new Dictionary<string, Func<string, string, uint, uint, ProductoDerivado>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Lacteo"] = (codigo, nombre, existencia, precioUnitario) => new Lacteo(codigo, nombre, existencia, precioUnitario),
                ["Carne"] = (codigo, nombre, existencia, precioUnitario) => new Carne(codigo, nombre, existencia, precioUnitario),
                ["Piel"] = (codigo, nombre, existencia, precioUnitario) => new Piel(codigo, nombre, existencia, precioUnitario)
            };
            builder.Services.AddSingleton<IReadOnlyDictionary<string, Func<string, string, uint, uint, ProductoDerivado>>>(creadoresProductosDerivados);
            builder.Services.AddSingleton<PersistenciaProductosDerivados>();
            builder.Services.AddSingleton<InventarioProductosDerivados>(sp =>
            {
                var persistenciaProductos = sp.GetRequiredService<PersistenciaProductosDerivados>();
                return new InventarioProductosDerivados(persistenciaProductos.CargarProductosDerivados());
            });
            builder.Services.AddSingleton<IInventarioProductosDerivados>(sp =>
                sp.GetRequiredService<InventarioProductosDerivados>());
            builder.Services.AddSingleton<IActualizacionProductosDerivados>(sp =>
                sp.GetRequiredService<PersistenciaProductosDerivados>());

            IReglaEdadRes reglaEdadTernero = new ReglaEdadTernero();
            IReglaEdadRes reglaEdadCebon = new ReglaEdadCebon();
            IReglaEdadRes reglaEdadNovillo = new ReglaEdadNovillo();

            IReglaPesoRes reglaPesoTernero = new ReglaPesoTernero();
            IReglaPesoRes reglaPesoCebon = new ReglaPesoCebon();
            IReglaPesoRes reglaPesoNovillo = new ReglaPesoNovillo();

            IReglasTipoPotrero[] reglas =
            {
                new ReglaTipoPotreroTernero(reglaEdadTernero, reglaPesoTernero),
                new ReglaTipoPotreroCebon(reglaEdadCebon, reglaPesoCebon),
                new ReglaTipoPotreroNovillo(reglaEdadNovillo, reglaPesoNovillo)
            };

            var reglasPorTipo = reglas.ToDictionary(
                r => r.TipoPotrero,
                StringComparer.OrdinalIgnoreCase);

            builder.Services.AddSingleton<IReadOnlyDictionary<string, IReglasTipoPotrero>>(reglasPorTipo);
            builder.Services.AddSingleton<ITiposPotreroDisponibles, TiposPotreroDisponibles>();
            IReglaCargaTipoVacuna[] reglasCargaVacuna =
            {
                new ReglaCargaBacteriana(),
                new ReglaCargaViva()
            };

            var reglasCargaVacunaPorTipo = reglasCargaVacuna.ToDictionary(
                r => r.TipoVacuna,
                StringComparer.OrdinalIgnoreCase);

            builder.Services.AddSingleton<ICargaVacuna>(_ => new CargaVacuna(
                reglasCargaVacunaPorTipo,
                reglasCargaVacunaPorTipo["Viva"]));
            IReglasAutorizacionRol[] reglasAutorizacion =
            {
                new ReglaAutorizacionAdministrador(),
                new ReglaAutorizacionEmpleado(),
                new ReglaAutorizacionVisitante()
            };

            var reglasAutorizacionPorRol = reglasAutorizacion.ToDictionary(
                r => r.Rol,
                StringComparer.Ordinal);

            builder.Services.AddSingleton<IReadOnlyDictionary<string, IReglasAutorizacionRol>>(reglasAutorizacionPorRol);

            builder.Services.AddSingleton<RegistroUsuarios>();
            builder.Services.AddSingleton<ILecturaUsuarios>(sp =>
                sp.GetRequiredService<RegistroUsuarios>());
            builder.Services.AddSingleton<IEscrituraUsuarios>(sp =>
                sp.GetRequiredService<RegistroUsuarios>());
            builder.Services.AddSingleton<IConsultaUsuarios, ConsultaUsuarios>();
            builder.Services.AddSingleton<ICreacionUsuario, CreacionUsuario>();
            builder.Services.AddSingleton<IInicializacionUsuarios, InicializacionUsuarios>();
            builder.Services.AddSingleton<IValidacionCredenciales, Autenticacion>();
            builder.Services.AddSingleton<IAutorizacionOperacion, AutorizacionOperacion>();

            builder.Services.AddSingleton<INotificadorPotreroMitad>(_ => new PublisherPotreroMitad());
            builder.Services.AddSingleton<INotificadorPotreroLleno>(_ => new PublisherPotreroLleno());
            builder.Services.AddSingleton<INotificadorPesoMin>(_ => new PublisherPesoMin());
            builder.Services.AddSingleton<INotificadorPesoVenta>(_ => new PublisherPesoVenta());
            builder.Services.AddSingleton<INotificacionIncorporacionRes>(sp => new NotificacionIncorporacionRes(
                sp.GetRequiredService<INotificadorPotreroMitad>(),
                sp.GetRequiredService<INotificadorPotreroLleno>(),
                sp.GetRequiredService<INotificadorPesoMin>(),
                sp.GetRequiredService<INotificadorPesoVenta>()));
            
            // Registrar como Singleton (sin InterceptorValidarInformacion)
            builder.Services.AddSingleton<PersistenciaService>();
            builder.Services.AddSingleton<ICargaUsuarios>(sp =>
                sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<IGuardadoUsuarios>(sp =>
                sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<IActualizacionPotreros>(sp =>
                sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<IActualizacionReses>(sp =>
                sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<IActualizacionVentas>(sp =>
                sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<ICargaVacunas>(sp =>
                sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<IActualizacionInventarioVacunas>(sp =>
                sp.GetRequiredService<PersistenciaService>());
            builder.Services.AddSingleton<IActualizacionHistorialVacunacion>(sp =>
                sp.GetRequiredService<PersistenciaService>());
            
            // Hacienda como Singleton - datos compartidos globalmente
            builder.Services.AddSingleton<Hacienda>(sp =>
            {
                var hacienda = new Hacienda();
                var persistencia = sp.GetRequiredService<PersistenciaService>();

                // Cargar datos al iniciar
                try
                {
                    var potreros = persistencia.CargarPotreros();
                    foreach (var potrero in potreros)
                    {
                        hacienda.L_potreros.Add(potrero);
                    }

                    // Cargar reses en los potreros
                    persistencia.CargarReses(hacienda.L_potreros);

                    // Cargar vacunas aplicadas a las reses
                    persistencia.CargarVacunasAplicadas(hacienda.L_potreros);

                    var ventas = persistencia.CargarVentas(hacienda.L_potreros);
                    foreach (var venta in ventas)
                    {
                        hacienda.L_ventas.Add(venta);
                    }

                    var vacunas = persistencia.CargarVacunas();
                    foreach (var vacuna in vacunas)
                    {
                        hacienda.L_vacunas.Add(vacuna);
                    }

                    Console.WriteLine($"Datos cargados: {potreros.Count} potreros, {ventas.Count} ventas, {vacunas.Count} vacunas");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar datos: {ex.Message}");
                }

                return hacienda;
            });

            builder.Services.AddSingleton<IEstadoPotreros>(sp =>
                sp.GetRequiredService<Hacienda>());
            builder.Services.AddSingleton<IRegistroPotrero>(sp =>
                sp.GetRequiredService<Hacienda>());
            builder.Services.AddSingleton<IInventarioVacunas>(sp =>
                sp.GetRequiredService<Hacienda>());
            builder.Services.AddSingleton<IEstadoVentas>(sp =>
                sp.GetRequiredService<Hacienda>());
            builder.Services.AddSingleton<IRegistroVenta>(sp =>
                sp.GetRequiredService<Hacienda>());
            builder.Services.AddSingleton<IConsultaPotreros, ConsultaPotreros>();
            builder.Services.AddSingleton<EstadisticasPotreros>();

            // Servicios como Singleton
            builder.Services.AddSingleton<PotreroService>();
            builder.Services.AddSingleton<ResService>();
            builder.Services.AddSingleton<InventarioVacunas>();
            builder.Services.AddSingleton<ConsultaVacunas>();
            builder.Services.AddSingleton<EstadisticasVacunas>();
            builder.Services.AddSingleton<NotificacionVacunacion>();
            builder.Services.AddSingleton<RegistroVacunacion>();
            builder.Services.AddSingleton<AplicacionVacuna>();
            builder.Services.AddSingleton<ICreacionVacunaBacteriana, CreacionVacunaBacteriana>();
            builder.Services.AddSingleton<ICreacionVacunaViva, CreacionVacunaViva>();
            builder.Services.AddSingleton<FormularioBacteriana>();
            builder.Services.AddSingleton<FormularioViva>();
            builder.Services.AddSingleton<IReadOnlyDictionary<string, ICreacionPorTipoVacuna>>(sp =>
            {
                var formularioBacteriana = sp.GetRequiredService<FormularioBacteriana>();
                var formularioViva = sp.GetRequiredService<FormularioViva>();

                return new Dictionary<string, ICreacionPorTipoVacuna>(StringComparer.Ordinal)
                {
                    ["Bacteriana"] = formularioBacteriana,
                    ["Viva"] = formularioViva
                };
            });
            builder.Services.AddSingleton<ICreacionPorTipoVacuna>(sp =>
                sp.GetRequiredService<FormularioViva>());
            builder.Services.AddSingleton<VentaService>();
            builder.Services.AddSingleton<IVentaRes>(sp =>
                sp.GetRequiredService<VentaService>());
            builder.Services.AddSingleton<IVentaProductoDerivado>(sp =>
                sp.GetRequiredService<VentaService>());
            builder.Services.AddSingleton<UsuariosRegistradosHacienda>(sp =>
            {
                var cargaUsuarios = sp.GetRequiredService<ICargaUsuarios>();
                var usuarios = cargaUsuarios.CargarUsuarios();
                return new UsuariosRegistradosHacienda(usuarios);
            });
            builder.Services.AddSingleton<CreacionUsuarioHacienda>();
            builder.Services.AddSingleton<ConsultaUsuariosHacienda>();
            builder.Services.AddSingleton<AutenticacionUsuarioHacienda>();
            builder.Services.AddSingleton<EstadisticasUsuariosHacienda>();
            builder.Services.AddSingleton<InicioSesionUsuario>();

            var app = builder.Build();

            app.Services.GetRequiredService<UsuariosRegistradosHacienda>();

            app.Services.GetRequiredService<IInicializacionUsuarios>().Inicializar();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // --- Habilitar Autenticación y Autorización ---
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
