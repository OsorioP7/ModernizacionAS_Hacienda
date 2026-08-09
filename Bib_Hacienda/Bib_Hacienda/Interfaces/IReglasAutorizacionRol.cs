namespace Bib_Hacienda.Interfaces
{
    public interface IReglasAutorizacionRol
    {
        string Rol { get; }

        bool TienePermiso(string operacion);
    }
}
