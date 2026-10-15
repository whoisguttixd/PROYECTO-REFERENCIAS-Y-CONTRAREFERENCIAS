using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_REFERENCIAS.Models
{
    /// <summary>
    /// Detalle de una SolicitudReferencia (Maestro-Detalle, Semana 9).
    /// Contiene: código CIE-10 / RENAES-cie, diagnóstico, tipo de examen/laboratorio,
    /// signos de alarma, observaciones. Es responsable del cálculo de gravedad parcial del patrón Strategy.
    /// </summary>
    public class DetalleReferencia
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDetalle { get; set; }

        [Required(ErrorMessage = "El código CIE-10 / RENAES-cie es obligatorio")]
        [RegularExpression(@"^[A-Z][0-9]{2}(\.[0-9]{1,2})?[A-Z]?$", ErrorMessage = "Formato CIE-10 inválido (ej. A00, A00.0, A00B)")]
        [StringLength(8, MinimumLength = 2, ErrorMessage = "El código CIE-10 debe tener entre {2} y {1} caracteres")]
        public string CodigoCie10 { get; set; } = string.Empty;

        [Required(ErrorMessage = "El diagnóstico es obligatorio")]
        [StringLength(300, ErrorMessage = "El diagnóstico no puede exceder {1} caracteres")]
        public string Diagnostico { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de diagnóstico es obligatorio")]
        [StringLength(20, ErrorMessage = "El tipo de diagnóstico no puede exceder {1} caracteres")]
        public string TipoDiagnostico { get; set; } = "Presuntivo";

        [Required(ErrorMessage = "El tipo de apoyo es obligatorio")]
        [StringLength(40, ErrorMessage = "El tipo de apoyo no puede exceder {1} caracteres")]
        public string TipoApoyo { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripción del examen/laboratorio no puede exceder {1} caracteres")]
        public string? DescripcionApoyo { get; set; }

        [StringLength(150, ErrorMessage = "El nombre del examen auxiliar adjunto no puede exceder {1} caracteres")]
        public string? ExamenAuxiliar { get; set; }

        [StringLength(600, ErrorMessage = "Los hallazgos clínicos no pueden exceder {1} caracteres")]
        public string? HallazgosClinicos { get; set; }

        [StringLength(600, ErrorMessage = "Las observaciones no pueden exceder {1} caracteres")]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "Los signos de alarma son obligatorios")]
        public bool SignosAlarma { get; set; }

        [Required(ErrorMessage = "La prioridad de este detalle es obligatoria")]
        public PrioridadReferencia PrioridadDetalle { get; set; } = PrioridadReferencia.SinCalcular;

        [ForeignKey(nameof(SolicitudReferencia))]
        public int IdReferencia { get; set; }

        public SolicitudReferencia? SolicitudReferencia { get; set; }

        [NotMapped]
        public string CieFormateado => string.IsNullOrWhiteSpace(CodigoCie10) ? "-" : CodigoCie10;
    }
}
