using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Aspectos
{
    //Interceptor que simula conexión a BD usando Castle DynamicProxy y HTTP Context
    public class InterceptorValidarInformacion : IInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Constructor que recibe el contexto HTTP
        public InterceptorValidarInformacion(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Intercept(IInvocation invocation)
        {
            string tipoEntidad = invocation.Method.Name; // ValidarRes, ValidarPotrero, etc.
            string nombreEntidad = invocation.Arguments[0]?.GetType().Name ?? "Desconocido";

            try
            {
                // Guardar contexto en HttpContext.Items para uso posterior
                if (_httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Items["UltimaValidacion"] = tipoEntidad;
                    _httpContextAccessor.HttpContext.Items["EntidadValidada"] = nombreEntidad;
                    _httpContextAccessor.HttpContext.Items["MensajeValidacion"] = $"Simulando conexión a BD para: {tipoEntidad}";
                }

                // Ejecutar el método de validación
                invocation.Proceed();

                // Obtener resultado de la validación
                bool resultado = (bool)invocation.ReturnValue;

                // Guardar resultado en el contexto
                if (_httpContextAccessor.HttpContext != null)
                {
                    if (resultado)
                    {
                        _httpContextAccessor.HttpContext.Items["ResultadoValidacion"] = "Datos válidos. Guardado exitoso en BD";
                        _httpContextAccessor.HttpContext.Items["ValidacionExitosa"] = true;
                    }
                    else
                    {
                        _httpContextAccessor.HttpContext.Items["ResultadoValidacion"] = "Datos inválidos. NO se guardó en BD";
                        _httpContextAccessor.HttpContext.Items["ValidacionExitosa"] = false;
                    }
                }
            }
            catch (NotImplementedException ex)
            {
                // Capturar excepciones de métodos no implementados
                if (_httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Items["ResultadoValidacion"] = $"Método no implementado: {ex.Message}";
                    _httpContextAccessor.HttpContext.Items["ValidacionExitosa"] = false;
                }

                // Re-lanzar para que el Controller maneje la excepción
                throw;
            }
            catch (Exception ex)
            {
                // Capturar cualquier otro error
                if (_httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Items["ResultadoValidacion"] = $"Error durante validación: {ex.Message}";
                    _httpContextAccessor.HttpContext.Items["ValidacionExitosa"] = false;
                }

                // Re-lanzar para que el Controller maneje la excepción
                throw;
            }
        }
    }
}
