using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_REFERENCIAS.Models
{
    /// <summary>
    /// Contra-referencia (1:1) devuelta al centro de salud de origen con el resultado del proceso.
    /// Relación 1:1 con SolicitudReferencia: la FK IdContrareferencia se sitúa en SolicitudReferencia
    /// para modelar la cardinalidad 1:1 clásica (una sola contrareferencia por referencia).
    /// </summary>
    public class Contrareferencia
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdContrareferencia { get; set; }

        [Required(ErrorMessage = "La fecha de emisión es obligatoria")]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "El resultado es obligatorio")]
        [StringLength(600, ErrorMessage = "El resultado no puede exceder {1} caracteres")]
        public string Resultado { get; set; } = string.Empty;

        [Required(ErrorMessage = "La indicación al centro de origen es obligatoria")]
        [StringLength(600, ErrorMessage = "La indicación no puede exceder {1} caracteres")]
        public string IndicacionCentroOrigen { get; set; } = string.Empty;

        [StringLength(400, ErrorMessage = "La evolución clínica no puede exceder {1} caracteres")]
        public string? EvolucionClinica { get; set; }

        [StringLength(100, ErrorMessage = "El tipo de egreso no puede exceder {1} caracteres")]
        public string? TipoEgreso { get; set; }

        [ForeignKey(nameof(SolicitudReferencia))]
        public int IdReferencia { get; set; }

        /// <summary>
        /// Navegación 1:1.
        /// </summary>
        [InverseProperty(nameof(SolicitudReferencia.Contrareferencia))]
        public SolicitudReferencia? SolicitudReferencia { get; set; }
    }
}
