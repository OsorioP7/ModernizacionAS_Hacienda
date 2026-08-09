using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bib_Hacienda.Clases
{
    public class AutorizacionOperacion : IAutorizacionOperacion
    {
        private readonly ILecturaUsuarios lectura_usuarios;
        private readonly IReadOnlyDictionary<string, IReglasAutorizacionRol> reglas_por_rol;

        public AutorizacionOperacion(
            ILecturaUsuarios lectura_usuarios,
            IReadOnlyDictionary<string, IReglasAutorizacionRol> reglas_por_rol)
        {
            this.lectura_usuarios = lectura_usuarios;
            this.reglas_por_rol = reglas_por_rol;
        }

        public void AutorizarOperacion(Usuario usuario, string operacion)
        {
            if (usuario == null)
            {
                throw new Exception("✗ Usuario no autenticado. Debe iniciar sesión para realizar operaciones");
            }

            Usuario usuarioRegistrado = lectura_usuarios.ObtenerTodos().FirstOrDefault(u =>
                u.Nombre == usuario.Nombre && u.Contrasena == usuario.Contrasena);

            if (usuarioRegistrado == null)
            {
                throw new Exception($"✗ Usuario '{usuario.Nombre}' no está registrado en el sistema");
            }

            bool tienePermiso = reglas_por_rol.TryGetValue(
                usuarioRegistrado.Rol ?? string.Empty,
                out IReglasAutorizacionRol regla) &&
                regla.TienePermiso(operacion);

            if (tienePermiso)
            {
                throw new Exception($"✓ Usuario '{usuario.Nombre}' autorizado para ejecutar: {operacion}");
            }
            else
            {
                throw new Exception($"✗ Acceso DENEGADO. Usuario '{usuario.Nombre}' NO tiene permisos para: {operacion}");
            }
        }
    }
}
