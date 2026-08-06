using Bib_Hacienda.Aspectos;
using Bib_Hacienda.Clases;
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
            
            // Registrar como Singleton (sin InterceptorValidarInformacion)
            builder.Services.AddSingleton<PersistenciaService>();
            
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

            // Servicios como Singleton
            builder.Services.AddSingleton<PotreroService>();
            builder.Services.AddSingleton<ResService>();
            builder.Services.AddSingleton<VacunaService>();
            builder.Services.AddSingleton<VentaService>();
            builder.Services.AddSingleton<UsuarioService>(sp =>
            {
                var persistencia = sp.GetRequiredService<PersistenciaService>();
                var usuarioService = new UsuarioService(persistencia);
                usuarioService.CargarUsuarios();
                return usuarioService;
            });

            var app = builder.Build();

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
