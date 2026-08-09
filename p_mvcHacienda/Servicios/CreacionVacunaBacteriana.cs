using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas.ReglasVacunacion;

namespace p_mvcHacienda.Servicios
{
    public class CreacionVacunaBacteriana : ICreacionVacunaBacteriana
    {
        private readonly IInventarioVacunas _inventarioVacunas;
        private readonly IActualizacionInventarioVacunas _actualizacionInventarioVacunas;

        public CreacionVacunaBacteriana(
            IInventarioVacunas inventarioVacunas,
            IActualizacionInventarioVacunas actualizacionInventarioVacunas)
        {
            _inventarioVacunas = inventarioVacunas;
            _actualizacionInventarioVacunas = actualizacionInventarioVacunas;
        }

        public string crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion)
        {
            try
            {
                string resultadoDominio = CrearVacunaBacteriana(
                    nombre,
                    lote,
                    fecha_vencimiento,
                    fecha_aplicacion,
                    periodo_aplicacion);
                string validado = _actualizacionInventarioVacunas.GuardarVacunas(_inventarioVacunas.Vacunas);
                return $"{resultadoDominio}. {validado}";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        public string crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion, uint cantidad)
        {
            try
            {
                string resultadoDominio = CrearLoteVacunasBacterianas(
                    nombre,
                    lote_base,
                    fecha_vencimiento,
                    fecha_aplicacion,
                    periodo_aplicacion,
                    cantidad);
                string validado = _actualizacionInventarioVacunas.GuardarVacunas(_inventarioVacunas.Vacunas);
                return $"{resultadoDominio}. {validado}";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        private string CrearVacunaBacteriana(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre de la vacuna no puede estar vacío", nameof(nombre));

                if (string.IsNullOrWhiteSpace(lote))
                    throw new ArgumentException("El lote de la vacuna no puede estar vacío", nameof(lote));

                if (fecha_vencimiento <= fecha_aplicacion)
                    throw new Exception("La fecha de vencimiento debe ser posterior a la fecha de aplicación");

                if (_inventarioVacunas.Vacunas.Any(v => v.Lote.Equals(lote, StringComparison.OrdinalIgnoreCase)))
                    throw new Exception($"Ya existe una vacuna con el lote '{lote}' en el inventario");

                Bacteriana nuevaVacuna = new Bacteriana(nombre, lote, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion);
                _inventarioVacunas.Vacunas.Add(nuevaVacuna);

                return $"Vacuna bacteriana '{nombre}' del lote '{lote}' agregada al inventario con éxito. Período de aplicación: {periodo_aplicacion} semanas.";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (bacteriana): " + er.Message);
            }
        }

        private string CrearLoteVacunasBacterianas(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion, uint cantidad)
        {
            try
            {
                if (cantidad <= 0)
                    throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(cantidad));

                ReglaLoteVacunas.ValidarCantidad(cantidad);

                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre de la vacuna no puede estar vacío", nameof(nombre));

                if (string.IsNullOrWhiteSpace(lote_base))
                    throw new ArgumentException("El lote base no puede estar vacío", nameof(lote_base));

                if (fecha_vencimiento <= fecha_aplicacion)
                    throw new Exception("La fecha de vencimiento debe ser posterior a la fecha de aplicación");

                int vacunasCreadas = 0;

                for (int i = 1; i <= cantidad; i++)
                {
                    string loteNumerado = $"{lote_base}-{i:D3}";

                    if (_inventarioVacunas.Vacunas.Any(v => v.Lote.Equals(loteNumerado, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    Bacteriana nuevaVacuna = new Bacteriana(nombre, loteNumerado, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion);
                    _inventarioVacunas.Vacunas.Add(nuevaVacuna);
                    vacunasCreadas++;
                }

                if (vacunasCreadas == 0)
                    throw new Exception("No se pudo crear ninguna vacuna. Todos los lotes ya existen en el inventario");

                return $"Lote de vacunas bacterianas creado con éxito:\n" +
                "- Nombre: {nombre}\n" +
                $"- Cantidad creada: {vacunasCreadas} de {cantidad}\n" +
                $"- Lotes: {lote_base}-001 a {lote_base}-{vacunasCreadas:D3}\n" +
                $"- Período de aplicación: {periodo_aplicacion} semanas";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (lote bacteriano): " + er.Message);
            }
        }
    }
}
