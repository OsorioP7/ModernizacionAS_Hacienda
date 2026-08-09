using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bib_Hacienda.Clases
{
    public class ConsultaUsuarios : IConsultaUsuarios
    {
        private readonly ILecturaUsuarios lectura_usuarios;

        public ConsultaUsuarios(ILecturaUsuarios lectura_usuarios)
        {
            this.lectura_usuarios = lectura_usuarios;
        }

        public List<Usuario> listar_usuarios()
        {
            return new List<Usuario>(lectura_usuarios.ObtenerTodos());
        }

        public Usuario buscar_usuario(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre de búsqueda no puede estar vacío.");
                }

                Usuario usuario = lectura_usuarios.ObtenerTodos().FirstOrDefault(u =>
                    u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));

                if (usuario == null)
                {
                    throw new Exception($"No se encontró el usuario '{nombre}'.");
                }

                return usuario;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método buscar_usuario: " + er.Message);
            }
        }

        public bool ExisteUsuario(string nombre)
        {
            return lectura_usuarios.ObtenerTodos().Any(u =>
                u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }
    }
}
