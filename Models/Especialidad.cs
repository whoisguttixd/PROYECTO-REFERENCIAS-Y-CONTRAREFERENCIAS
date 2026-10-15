using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_REFERENCIAS.Models
{
    /// <summary>
    /// Especialidad médica del sistema (ej. Medicina Interna, Pediatría, Traumatología, etc.).
    /// Relación N:M con EstablecimientoSalud mediante EstablecimientoEspecialidad.
    /// </summary>
    public class Especialidad
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEspecialidad { get; set; }

        [Required(ErrorMessage = "El código CIE-10 / código interno es obligatorio")]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "El código debe tener entre {2} y {1} caracteres")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de la especialidad es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "La descripción no puede exceder {1} caracteres")]
        public string? Descripcion { get; set; }

        [StringLength(3, ErrorMessage = "El nivel no puede exceder {1} caracteres")]
        public string? NivelAtencion { get; set; } = "I";

        /// <summary>
        /// Establecimientos que ofrecen esta especialidad (N:M).
        /// </summary>
        public ICollection<EstablecimientoEspecialidad> EstablecimientoEspecialidades { get; set; } = new List<EstablecimientoEspecialidad>();

        /// <summary>
        /// Relación N:M implícita: solicitudes que requieren esta especialidad (se resuelve por detalles).
        /// </summary>
        public ICollection<EstablecimientoEspecialidad> GetEstablecimientos(ICollection<EstablecimientoEspecialidad> tabla)
        {
            // Experto en información: permite conocer a qué establecimientos se puede derivar.
            return tabla?.Where(e => e.IdEspecialidad == IdEspecialidad).ToList() ?? new List<EstablecimientoEspecialidad>();
        }
    }
}
