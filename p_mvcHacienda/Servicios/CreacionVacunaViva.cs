using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas.ReglasVacunacion;
using static Bib_Hacienda.Clases.Viva;

namespace p_mvcHacienda.Servicios
{
    public class CreacionVacunaViva : ICreacionVacunaViva
    {
        private readonly IInventarioVacunas _inventarioVacunas;
        private readonly IActualizacionInventarioVacunas _actualizacionInventarioVacunas;

        public CreacionVacunaViva(
            IInventarioVacunas inventarioVacunas,
            IActualizacionInventarioVacunas actualizacionInventarioVacunas)
        {
            _inventarioVacunas = inventarioVacunas;
            _actualizacionInventarioVacunas = actualizacionInventarioVacunas;
        }

        public string crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, enum_l_atenuaciones grado_atenuacion)
        {
            try
            {
                string resultadoDominio = CrearVacunaViva(
                    nombre,
                    lote,
                    fecha_vencimiento,
                    fecha_aplicacion,
                    grado_atenuacion);
                string validado = _actualizacionInventarioVacunas.GuardarVacunas(_inventarioVacunas.Vacunas);
                return $"{resultadoDominio}. {validado}";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        public string crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, enum_l_atenuaciones grado_atenuacion, uint cantidad)
        {
            try
            {
                string resultadoDominio = CrearLoteVacunasVivas(
                    nombre,
                    lote_base,
                    fecha_vencimiento,
                    fecha_aplicacion,
                    grado_atenuacion,
                    cantidad);
                string validado = _actualizacionInventarioVacunas.GuardarVacunas(_inventarioVacunas.Vacunas);
                return $"{resultadoDominio}. {validado}";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        private string CrearVacunaViva(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, enum_l_atenuaciones grado_atenuacion)
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

                Viva nuevaVacuna = new Viva(nombre, lote, fecha_vencimiento, fecha_aplicacion, grado_atenuacion);
                _inventarioVacunas.Vacunas.Add(nuevaVacuna);

                return $"Vacuna viva '{nombre}' del lote '{lote}' agregada al inventario con éxito. Grado de atenuación: {(int)grado_atenuacion}.";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (viva): " + er.Message);
            }
        }

        private string CrearLoteVacunasVivas(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, enum_l_atenuaciones grado_atenuacion, uint cantidad)
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

                    Viva nuevaVacuna = new Viva(nombre, loteNumerado, fecha_vencimiento, fecha_aplicacion, grado_atenuacion);
                    _inventarioVacunas.Vacunas.Add(nuevaVacuna);
                    vacunasCreadas++;
                }

                if (vacunasCreadas == 0)
                    throw new Exception("No se pudo crear ninguna vacuna. Todos los lotes ya existen en el inventario");

                return $"Lote de vacunas vivas creado con éxito:\n" +
                "- Nombre: {nombre}\n" +
                $"- Cantidad creada: {vacunasCreadas} de {cantidad}\n" +
                $"- Lotes: {lote_base}-001 a {lote_base}-{vacunasCreadas:D3}\n" +
                $"- Grado de atenuación: {(int)grado_atenuacion}";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (lote vivo): " + er.Message);
            }
        }
    }
}
