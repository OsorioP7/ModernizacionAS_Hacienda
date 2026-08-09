using Bib_Hacienda.Interfaces;
using System;
using System.Linq;

namespace Bib_Hacienda.Clases
{
    public class Autenticacion : IValidacionCredenciales
    {
        private readonly ILecturaUsuarios lectura_usuarios;

        public Autenticacion(ILecturaUsuarios lectura_usuarios)
        {
            this.lectura_usuarios = lectura_usuarios;
        }

        public bool ValidarCredenciales(string nombre, string contrasena)
        {
            return lectura_usuarios.ObtenerTodos().Any(u =>
                u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
                u.Contrasena == contrasena);
        }
    }
}
