using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_REFERENCIAS.Models
{
    /// <summary>
    /// Centro de salud de la red hospitalaria de Cajamarca (Hospital Regional, CS de distritos, etc.).
    /// Patrones:
    ///   - GRASP Information Expert: tiene la responsabilidad de conocer sus especialidades y ubicación.
    ///   - Relaciones 1:N: (1) EstablecimientoSalud -> (N) SolicitudReferencia (Origen y/o Destino).
    ///   - Relación N:M: con Especialidad a través de EstablecimientoEspecialidad.
    /// </summary>
    public class EstablecimientoSalud
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEstablecimiento { get; set; }

        [Required(ErrorMessage = "El código interno del establecimiento es obligatorio")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "El código debe tener entre {2} y {1} caracteres")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del establecimiento es obligatorio")]
        [StringLength(120, ErrorMessage = "El nombre no puede exceder {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Tipo de establecimiento es obligatorio")]
        [StringLength(50)]
        public string TipoEstablecimiento { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Red de salud es obligatoria")]
        [StringLength(60)]
        public string RedSalud { get; set; } = string.Empty;

        [Required(ErrorMessage = "El distrito/provincia es obligatorio")]
        [StringLength(60)]
        public string Ubicacion { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "El teléfono no puede exceder {1} caracteres")]
        public string? Telefono { get; set; }

        [StringLength(60, ErrorMessage = "El representante legal no puede exceder {1} caracteres")]
        public string? RepresentanteLegal { get; set; }

        /// <summary>
        /// Solicitudes de referencia que tienen este establecimiento como ORIGEN.
        /// </summary>
        public ICollection<SolicitudReferencia> ReferenciasComoOrigen { get; set; } = new List<SolicitudReferencia>();

        /// <summary>
        /// Solicitudes de referencia que tienen este establecimiento como DESTINO.
        /// </summary>
        public ICollection<SolicitudReferencia> ReferenciasComoDestino { get; set; } = new List<SolicitudReferencia>();

        /// <summary>
        /// Especialidades disponibles en este establecimiento (N:M).
        /// </summary>
        public ICollection<EstablecimientoEspecialidad> EstablecimientoEspecialidades { get; set; } = new List<EstablecimientoEspecialidad>();

        /// <summary>
        /// Personal de salud asociado a este establecimiento.
        /// </summary>
        public ICollection<PersonalSalud> Personal { get; set; } = new List<PersonalSalud>();

        public string NombreCompleto => $"{TipoEstablecimiento} - {Nombre} ({Ubicacion})";
    }
}
