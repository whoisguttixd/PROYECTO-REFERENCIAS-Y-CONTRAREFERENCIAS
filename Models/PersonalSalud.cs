using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_REFERENCIAS.Models
{
    /// <summary>
    /// Médico o profesional de la salud del HRC o de los Centros de Salud asociados.
    /// Relación N:M opcional: un equipo médico puede ser asignado a varias solicitudes
    /// y una solicitud puede tener varios profesionales asignados.
    /// </summary>
    public class PersonalSalud
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPersonal { get; set; }

        [Required(ErrorMessage = "El código profesional es obligatorio")]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "El código debe tener entre {2} y {1} caracteres")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos del profesional son obligatorios")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder {1} caracteres")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los nombres del profesional son obligatorios")]
        [StringLength(100, ErrorMessage = "Los nombres no pueden exceder {1} caracteres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El colegio profesional es obligatorio")]
        [StringLength(4, ErrorMessage = "El colegio profesional tiene {1} caracteres")]
        public string ColegioProfesional { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de colegiado es obligatorio")]
        [RegularExpression(@"^[0-9]{4,10}$", ErrorMessage = "El número de colegiado solo debe contener números")]
        [StringLength(10)]
        public string NumeroColegiado { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de profesional es obligatorio")]
        [StringLength(50)]
        public string TipoPersonal { get; set; } = string.Empty;

        [Required(ErrorMessage = "El establecimiento de adscripción es obligatorio")]
        public int IdEstablecimiento { get; set; }

        [StringLength(200, ErrorMessage = "La especialidad principal no puede exceder {1} caracteres")]
        public string? EspecialidadPrincipal { get; set; }

        [StringLength(20, ErrorMessage = "El SIP no puede exceder {1} caracteres")]
        public string? Sip { get; set; }

        /// <summary>
        /// Establecimiento de adscripción (N:1).
        /// </summary>
        [ForeignKey(nameof(IdEstablecimiento))]
        public EstablecimientoSalud? Establecimiento { get; set; }

        /// <summary>
        /// Relación N:M con SolicitudReferencia: equipo médico asignado (opcional).
        /// </summary>
        public ICollection<SolicitudReferenciaPersonal> SolicitudesAsignadas { get; set; } = new List<SolicitudReferenciaPersonal>();

        /// <summary>
        /// GRASP Information Expert: el profesional de salud conoce cómo presentarse.
        /// Se usa como texto en los <select> del formulario Maestro-Detalle y mantenimientos.
        /// </summary>
        [NotMapped]
        public string NombreCompleto => $"{Apellidos}, {Nombres} ({ColegioProfesional} {NumeroColegiado})";
    }
}
