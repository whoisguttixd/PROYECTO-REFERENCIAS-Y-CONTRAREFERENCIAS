using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_REFERENCIAS.Models
{
    /// <summary>
    /// Tabla intermedia N:M opcional entre SolicitudReferencia y PersonalSalud.
    /// Permite asignar un equipo médico (uno o varios profesionales) a una referencia.
    /// </summary>
    public class SolicitudReferenciaPersonal
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSolicitudPersonal { get; set; }

        [Required(ErrorMessage = "La referencia es obligatoria")]
        public int IdReferencia { get; set; }

        [Required(ErrorMessage = "El profesional es obligatorio")]
        public int IdPersonalSalud { get; set; }

        public SolicitudReferencia? SolicitudReferencia { get; set; }

        public PersonalSalud? PersonalSalud { get; set; }

        public DateTime? FechaAsignacion { get; set; }
    }
}
