using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace PROYECTO_REFERENCIAS.ViewModels
{
    /// <summary>
    /// ViewModel para registro/edición Maestro-Detalle de SolicitudReferencia.
    /// Patrón GRASP: Controller - el controlador no manipula entidades complejas directamente
    /// si no que opera sobre ViewModels validados.
    /// </summary>
    public class SolicitudReferenciaVM
    {
        public int IdReferencia { get; set; }

        [Required(ErrorMessage = "El número de referencia es obligatorio")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El número de referencia debe tener entre {2} y {1} caracteres")]
        [RegularExpression(@"^HRC-[0-9]{4,10}$", ErrorMessage = "Formato requerido: HRC-0000")]
        public string NumeroReferencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de registro es obligatoria")]
        public DateTime FechaRegistro { get; set; }

        [Required(ErrorMessage = "La fecha de derivación es obligatoria")]
        public DateTime FechaDerivacion { get; set; }

        [Required(ErrorMessage = "La prioridad es obligatoria")]
        public Models.PrioridadReferencia Prioridad { get; set; } = Models.PrioridadReferencia.SinCalcular;

        // Prioridad calculada por el patrón Strategy (nullable).
        public Models.PrioridadReferencia? PrioridadCalculada { get; set; }

        [Required(ErrorMessage = "El número de turno/Expediente es obligatorio")]
        [StringLength(30, ErrorMessage = "El número de turno no puede exceder {1} caracteres")]
        public string Requerimiento { get; set; } = string.Empty;

        public string? UbicacionUrgencia { get; set; }

        [Required(ErrorMessage = "El motivo de la referencia es obligatorio")]
        [StringLength(600, ErrorMessage = "El motivo no puede exceder {1} caracteres")]
        public string Motivo { get; set; } = string.Empty;

        public string? OrdenMedica { get; set; }

        public string? ObservacionGeneral { get; set; }

        // ===== Signos vitales (cabecera clínica) =====
        [StringLength(15, ErrorMessage = "La presión arterial no puede exceder {1} caracteres")]
        public string? PresionArterial { get; set; }

        [Range(0, 300, ErrorMessage = "La frecuencia cardíaca debe estar entre {1} y {2} lpm")]
        public int? FrecuenciaCardiaca { get; set; }

        [Range(0, 100, ErrorMessage = "La frecuencia respiratoria debe estar entre {1} y {2} rpm")]
        public int? FrecuenciaRespiratoria { get; set; }

        [Range(typeof(decimal), "30", "45", ErrorMessage = "La temperatura debe estar entre {1} y {2} °C")]
        public decimal? Temperatura { get; set; }

        [Range(0, 100, ErrorMessage = "La saturación de oxígeno debe estar entre {1} y {2} %")]
        public int? SaturacionOxigeno { get; set; }

        [Range(typeof(decimal), "0", "400", ErrorMessage = "El peso debe estar entre {1} y {2} kg")]
        public decimal? PesoKg { get; set; }

        [Range(typeof(decimal), "0", "300", ErrorMessage = "La talla debe estar entre {1} y {2} cm")]
        public decimal? TallaCm { get; set; }

        // Médico solicitante / responsable de la derivación
        public int? IdMedicoSolicitante { get; set; }

        [Required(ErrorMessage = "El estado inicial es obligatorio")]
        public Models.EstadoReferencia Estado { get; set; } = Models.EstadoReferencia.Registrada;

        // FKs
        public int IdPaciente { get; set; }
        public int IdEstablecimientoOrigen { get; set; }
        public int IdEstablecimientoDestino { get; set; }

        // Selector simple del paciente
        public string? PacienteDisplay { get; set; }

        // Detalles (Maestro-Detalle)
        public List<DetalleReferenciaVM> Detalles { get; set; } = new List<DetalleReferenciaVM>();

        // Listas del ViewBag
        public List<Models.Paciente> Pacientes { get; set; } = new();
        public List<Models.EstablecimientoSalud> EstablecimientosOrigen { get; set; } = new();
        public List<Models.EstablecimientoSalud> EstablecimientosDestino { get; set; } = new();
        public List<Models.Especialidad> EspecialidadesCie10 { get; set; } = new();
        public List<Models.PersonalSalud> MedicosSolicitantes { get; set; } = new();
    }

    public class DetalleReferenciaVM
    {
        public int IdDetalle { get; set; }
        public int? IdDetalleOriginal { get; set; }

        [Required(ErrorMessage = "El código CIE-10 / RENAES-cie es obligatorio")]
        [RegularExpression(@"^[A-Z][0-9]{2}(\.[0-9]{1,2})?[A-Z]?$", ErrorMessage = "Formato CIE-10 inválido (ej. A00, A00.0, A00B)")]
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

        public string? DescripcionApoyo { get; set; }
        public string? ExamenAuxiliar { get; set; }
        public string? HallazgosClinicos { get; set; }
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "Los signos de alarma son obligatorios")]
        public bool SignosAlarma { get; set; }

        public Models.PrioridadReferencia PrioridadDetalle { get; set; } = Models.PrioridadReferencia.SinCalcular;
    }

    public class ContrareferenciaVM
    {
        public int IdContrareferencia { get; set; }
        public int IdReferencia { get; set; }
        public string? NumeroReferencia { get; set; }

        [Required(ErrorMessage = "La fecha de emisión es obligatoria")]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "El resultado es obligatorio")]
        [StringLength(600, ErrorMessage = "El resultado no puede exceder {1} caracteres")]
        public string Resultado { get; set; } = string.Empty;

        [Required(ErrorMessage = "La indicación al centro de origen es obligatoria")]
        [StringLength(600, ErrorMessage = "La indicación no puede exceder {1} caracteres")]
        public string IndicacionCentroOrigen { get; set; } = string.Empty;

        public string? EvolucionClinica { get; set; }
        public string? TipoEgreso { get; set; }
    }

    public class EvaluarReferenciaVM
    {
        public int IdReferencia { get; set; }
        public Models.EstadoReferencia NuevoEstado { get; set; }
        public string? ObservacionEstado { get; set; }
    }

    // Propiedades de ayuda para mostrar nombres en selects (no contaminan el dominio)
    // se resuelven en tiempo de ejecución a partir de los ViewModels.
    // Nota: este archivo centraliza los ViewModels del módulo de referencias.
}
