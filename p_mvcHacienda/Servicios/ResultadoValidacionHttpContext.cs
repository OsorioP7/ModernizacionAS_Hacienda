using Microsoft.AspNetCore.Http;
using Bib_Hacienda.Interfaces;
namespace p_mvcHacienda.Servicios
{
    public class ResultadoValidacionHttpContext : IResultadoValidacion
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ResultadoValidacionHttpContext(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string Obtener(string mensajeAlternativo)
        {
            return httpContextAccessor.HttpContext?
                .Items["ResultadoValidacion"]?
                .ToString()
                ?? mensajeAlternativo;
        }
    }
}
