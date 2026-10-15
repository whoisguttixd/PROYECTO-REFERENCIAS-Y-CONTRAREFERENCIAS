using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_REFERENCIAS.Models
{
    /// <summary>
    /// Estados posibles de una solicitud de referencia interhospitalaria (Máquina de Estados).
    /// Patrón GRASP: Controlador - la regla de negocio de qué transiciones son válidas
    /// se valida en servicio de dominio (ver Servicios/SolicitudReferenciaService).
    /// </summary>
    public enum EstadoReferencia
    {
        [Display(Name = "Registrada")]
        Registrada = 1,

        [Display(Name = "En Evaluación")]
        EnEvaluacion = 2,

        [Display(Name = "Aceptada")]
        Aceptada = 3,

        [Display(Name = "Observada")]
        Observada = 4,

        [Display(Name = "Rechazada")]
        Rechazada = 5,

        [Display(Name = "Contrareferida")]
        Contrareferida = 6
    }

    /// <summary>
    /// Prioridad de triaje calculada por el patrón Strategy (ver Servicios/Estrategias).
    /// </summary>
    public enum PrioridadReferencia
    {
        [Display(Name = "No calculada")]
        SinCalcular = 0,

        [Display(Name = "Consulta Externa")]
        ConsultaExterna = 1,

        [Display(Name = "Urgencia Mayor")]
        UrgenciaMayor = 2,

        [Display(Name = "Emergencia / Crítica")]
        EmergenciaCritica = 3
    }

    /// <summary>
    /// Cabecera de la Solicitud de Referencia Interhospitalaria del HRC.
    /// Relaciones:
    ///   - 1:N: Paciente (1) -> (N) SolicitudReferencia
    ///   - 1:N: EstablecimientoSalud (Origen)(1) -> (N) SolicitudReferencia
    ///   - 1:N: EstablecimientoSalud (Destino)(1) -> (N) SolicitudReferencia con DeleteBehavior.Restrict
    ///   - 1:1: SolicitudReferencia <-> Contrareferencia (Implementación de una relación 1:1)
    ///   - Maestro-Detalle: SolicitudReferencia (1) -> (N) DetalleReferencia
    ///   - N:M: SolicitudReferencia (N) <-> (M) PersonalSalud (via SolicitudReferenciaPersonal)
    /// </summary>
    public class SolicitudReferencia
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public PrioridadReferencia Prioridad { get; set; } = PrioridadReferencia.SinCalcular;

        // Prioridad calculada por el patrón Strategy (nullable: se llena al guardar/evaluar).
        public PrioridadReferencia? PrioridadCalculada { get; set; }

        [Required(ErrorMessage = "El número de turno/Expediente es obligatorio")]
        [StringLength(30, ErrorMessage = "El número de turno no puede exceder {1} caracteres")]
        public string Requerimiento { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "La urgencia clínica es opcional")]
        public string? UbicacionUrgencia { get; set; }

        [Required(ErrorMessage = "El motivo de la referencia es obligatorio")]
        [StringLength(600, ErrorMessage = "El motivo no puede exceder {1} caracteres")]
        public string Motivo { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La orden médica no puede exceder {1} caracteres")]
        public string? OrdenMedica { get; set; }

        [StringLength(500, ErrorMessage = "La observación general no puede exceder {1} caracteres")]
        public string? ObservacionGeneral { get; set; }

        // ===== Signos vitales del paciente al momento de la derivación (cabecera clínica) =====
        [StringLength(15, ErrorMessage = "La presión arterial no puede exceder {1} caracteres")]
        public string? PresionArterial { get; set; }

        [Range(0, 300, ErrorMessage = "La frecuencia cardíaca debe estar entre {1} y {2} lpm")]
        public int? FrecuenciaCardiaca { get; set; }

        [Range(0, 100, ErrorMessage = "La frecuencia respiratoria debe estar entre {1} y {2} rpm")]
        public int? FrecuenciaRespiratoria { get; set; }

        [Range(typeof(decimal), "30", "45", ErrorMessage = "La temperatura debe estar entre {1} y {2} °C")]
        [Column(TypeName = "decimal(4,1)")]
        public decimal? Temperatura { get; set; }

        [Range(0, 100, ErrorMessage = "La saturación de oxígeno debe estar entre {1} y {2} %")]
        public int? SaturacionOxigeno { get; set; }

        [Range(typeof(decimal), "0", "400", ErrorMessage = "El peso debe estar entre {1} y {2} kg")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? PesoKg { get; set; }

        [Range(typeof(decimal), "0", "300", ErrorMessage = "La talla debe estar entre {1} y {2} cm")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? TallaCm { get; set; }

        // ===== Médico solicitante / responsable de la derivación =====
        public int? IdMedicoSolicitante { get; set; }

        public PersonalSalud? MedicoSolicitante { get; set; }

        [Required(ErrorMessage = "El estado inicial es obligatorio")]
        public EstadoReferencia Estado { get; set; } = EstadoReferencia.Registrada;

        public DateTime? FechaEstado { get; set; }

        [StringLength(600, ErrorMessage = "La observación del estado no puede exceder {1} caracteres")]
        public string? ObservacionEstado { get; set; }

        public string? ObservacionSistema { get; set; }

        // FK 1:N: Paciente (siempre obligatorio)
        [Required(ErrorMessage = "El paciente es obligatorio")]
        public int IdPaciente { get; set; }

        // FK 1:N: Establecimiento de ORIGEN (obligatorio)
        [Required(ErrorMessage = "El establecimiento de origen es obligatorio")]
        public int IdEstablecimientoOrigen { get; set; }

        // FK 1:N: Establecimiento de DESTINO (obligatorio, con Restrict para evitar ciclos de cascada)
        [Required(ErrorMessage = "El establecimiento de destino es obligatorio")]
        public int IdEstablecimientoDestino { get; set; }

        // Navegación 1:N
        public Paciente? Paciente { get; set; }


        public EstablecimientoSalud? EstablecimientoOrigen { get; set; }
        public EstablecimientoSalud? EstablecimientoDestino { get; set; }

        // Navegación 1:1: Contrareferencia
        public Contrareferencia? Contrareferencia { get; set; }

        // Maestro-Detalle: 1:N con DetalleReferencia
        public ICollection<DetalleReferencia> Detalles { get; set; } = new List<DetalleReferencia>();

        // N:M con PersonalSalud (equipo médico asignado)
        public ICollection<SolicitudReferenciaPersonal> EquipoMedico { get; set; } = new List<SolicitudReferenciaPersonal>();

        public DateTime? FechaAceptacion { get; set; }
        public DateTime? FechaContrareferencia { get; set; }

        // GRASP Information Expert: indica si la referencia puede ser contrareferida
        [NotMapped]
        public bool PuedeContrareferirse => Estado == EstadoReferencia.Aceptada || Estado == EstadoReferencia.Observada;

        public string LineaResumen => $"{NumeroReferencia} - {Paciente?.Apellidos} {Paciente?.Nombres?.Substring(0, Math.Min(1, Paciente?.Nombres?.Length ?? 0))} | {Estado}";
    }
}
