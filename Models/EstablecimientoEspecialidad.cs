using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_REFERENCIAS.Models
{
    /// <summary>
    /// Tabla intermedia de relación N:M entre EstablecimientoSalud y Especialidad.
    /// Indica capacidad disponible y horario de atención de una especialidad en un establecimiento.
    /// </summary>
    public class EstablecimientoEspecialidad
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEstablecimientoEspecialidad { get; set; }

        [Required(ErrorMessage = "El establecimiento es obligatorio")]
        public int IdEstablecimiento { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria")]
        public int IdEspecialidad { get; set; }

        [Required(ErrorMessage = "La capacidad de atención es obligatoria")]
        [Range(0, 500, ErrorMessage = "La capacidad debe estar entre {2} y {1}")]
        public int CapacidadDisponible { get; set; } = 0;

        [Required(ErrorMessage = "Los horarios son obligatorios")]
        [StringLength(60)]
        public string HorarioAtencion { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Observaciones de la capacidad no pueden exceder {1} caracteres")]
        public string? ObservacionCapacidad { get; set; }

        /// <summary>
        /// Navegación 1:N con EstablecimientoSalud.
        /// </summary>
        [ForeignKey(nameof(IdEstablecimiento))]
        public EstablecimientoSalud? Establecimiento { get; set; }

        /// <summary>
        /// Navegación 1:N con Especialidad.
        /// </summary>
        [ForeignKey(nameof(IdEspecialidad))]
        public Especialidad? Especialidad { get; set; }

        /// <summary>
        /// Clave única compuesta para garantizar una sola fila por (Establecimiento, Especialidad).
        /// </summary>
        [NotMapped]
        public string LlaveCompuesta => $"{IdEstablecimiento}:{IdEspecialidad}";
    }
}
