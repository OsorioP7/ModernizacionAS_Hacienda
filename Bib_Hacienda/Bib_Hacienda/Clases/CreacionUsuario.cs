using Bib_Hacienda.Interfaces;
using System;

namespace Bib_Hacienda.Clases
{
    public class CreacionUsuario : ICreacionUsuario
    {
        private readonly IConsultaUsuarios consulta_usuarios;
        private readonly IEscrituraUsuarios escritura_usuarios;

        public CreacionUsuario(
            IConsultaUsuarios consulta_usuarios,
            IEscrituraUsuarios escritura_usuarios)
        {
            this.consulta_usuarios = consulta_usuarios;
            this.escritura_usuarios = escritura_usuarios;
        }

        public string crear_usuario(string nombre, string contrasena)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre del usuario no puede estar vacío", nameof(nombre));
                }

                if (string.IsNullOrWhiteSpace(contrasena))
                {
                    throw new ArgumentException("La contraseña no puede estar vacía", nameof(contrasena));
                }

                if (consulta_usuarios.ExisteUsuario(nombre))
                {
                    throw new InvalidOperationException($"Ya existe un usuario con el nombre '{nombre}'.");
                }

                Usuario nuevo_usuario = new Usuario(nombre, contrasena);
                escritura_usuarios.Agregar(nuevo_usuario);

                return $"Usuario '{nombre}' creado exitosamente en el sistema.";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_usuario: " + er.Message);
            }
        }
    }
}
