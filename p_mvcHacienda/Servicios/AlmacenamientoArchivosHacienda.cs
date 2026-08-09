using Bib_Hacienda.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace p_mvcHacienda.Servicios
{
    public class AlmacenamientoArchivosHacienda : IAlmacenamientoHacienda
    {
        private readonly string _directorioArchivos;

        public AlmacenamientoArchivosHacienda(IWebHostEnvironment env)
        {
            _directorioArchivos = Path.Combine(env.ContentRootPath, "Datos");

            if (!Directory.Exists(_directorioArchivos))
            {
                Directory.CreateDirectory(_directorioArchivos);
            }
        }

        public void Guardar(string nombreDatos, IEnumerable<string> registros)
        {
            string rutaArchivo = Path.Combine(_directorioArchivos, nombreDatos + ".txt");
            File.WriteAllLines(rutaArchivo, registros);
        }

        public IReadOnlyCollection<string> Cargar(string nombreDatos)
        {
            string rutaArchivo = Path.Combine(_directorioArchivos, nombreDatos + ".txt");
            return File.ReadAllLines(rutaArchivo);
        }

        public bool Existe(string nombreDatos)
        {
            string rutaArchivo = Path.Combine(_directorioArchivos, nombreDatos + ".txt");
            return File.Exists(rutaArchivo);
        }
    }
}
