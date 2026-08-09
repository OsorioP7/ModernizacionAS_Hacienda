namespace Bib_Hacienda.Interfaces
{
    public interface IValidador<T>
    {
        bool Validar(T elemento);
    }
}
