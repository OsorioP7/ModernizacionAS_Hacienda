using Bib_Hacienda.Aspectos;
using Bib_Hacienda.Clases;
using Bib_Hacienda.Clases.Validaciones;
using Bib_Hacienda.Interfaces;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;

namespace p_mvcHacienda.Servicios
{
    public class ValidadoresPersistenciaInterceptados : IValidadoresGuardado
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private InterceptorValidarInformacion? interceptorValidacion;

        private IValidador<Potrero>? validadorPotrero;
        private IValidador<Res>? validadorRes;
        private IValidador<Vacuna>? validadorVacuna;
        private IValidador<Venta>? validadorVenta;

        public ValidadoresPersistenciaInterceptados(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IValidador<Potrero> ValidadorPotrero
        {
            get
            {
                Inicializar();
                return validadorPotrero!;
            }
        }

        public IValidador<Res> ValidadorRes
        {
            get
            {
                Inicializar();
                return validadorRes!;
            }
        }

        public IValidador<Vacuna> ValidadorVacuna
        {
            get
            {
                Inicializar();
                return validadorVacuna!;
            }
        }

        public IValidador<Venta> ValidadorVenta
        {
            get
            {
                Inicializar();
                return validadorVenta!;
            }
        }

        private void Inicializar()
        {
            if (validadorVacuna == null)
            {
                if (interceptorValidacion == null)
                {
                    interceptorValidacion = new InterceptorValidarInformacion(httpContextAccessor);
                }

                var proxyGenerator = new ProxyGenerator();
                validadorVacuna = proxyGenerator.CreateClassProxy<ValidadorVacuna>(interceptorValidacion);
                validadorPotrero = proxyGenerator.CreateClassProxy<ValidadorPotrero>(interceptorValidacion);
                validadorRes = proxyGenerator.CreateClassProxy<ValidadorRes>(interceptorValidacion);
                validadorVenta = proxyGenerator.CreateClassProxy<ValidadorVenta>(interceptorValidacion);
            }
        }
    }
}
