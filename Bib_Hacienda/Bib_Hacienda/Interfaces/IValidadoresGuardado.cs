using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    public interface IValidadoresGuardado
    {
        IValidador<Potrero> ValidadorPotrero { get; }
        IValidador<Res> ValidadorRes { get; }
        IValidador<Vacuna> ValidadorVacuna { get; }
        IValidador<Venta> ValidadorVenta { get; }
    }
}
