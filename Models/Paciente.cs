using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_REFERENCIAS.Models
{
    /// <summary>
    /// Persona menor de edad o adulto que recibe atención en el HRC o en centros derivados.
    /// Patrón GRASP: Information Expert - los datos del paciente son propietarios de esta entidad.
    /// </summary>
    public class Paciente
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPaciente { get; set; }

        [Required(ErrorMessage = "El DNI/RUN es obligatorio")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El DNI/RUN debe tener entre {2} y {1} caracteres")]
        [RegularExpression(@"^[0-9]{6,20}$", ErrorMessage = "El DNI/RUN solo debe contener números")]
        public string Dni { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder {1} caracteres")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100, ErrorMessage = "Los nombres no pueden exceder {1} caracteres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        [StringLength(20, ErrorMessage = "El sexo registrado no puede exceder {1} caracteres")]
        public string? Sexo { get; set; }

        [StringLength(15, ErrorMessage = "El teléfono no puede exceder {1} caracteres")]
        public string? Telefono { get; set; }

        [StringLength(100, ErrorMessage = "La dirección no puede exceder {1} caracteres")]
        public string? Direccion { get; set; }

        [StringLength(200, ErrorMessage = "La alergia principal no puede exceder {1} caracteres")]
        public string? AlergiaPrincipal { get; set; }

        [StringLength(30, ErrorMessage = "El código RENAES no puede exceder {1} caracteres")]
        public string? CodigoRenaes { get; set; }

        /// <summary>
        /// GRASP Information Expert: el paciente conoce cómo presentarse (apellidos, nombres).
        /// Se usa como texto en los <select> del formulario Maestro-Detalle y mantenimientos.
        /// </summary>
        [NotMapped]
        public string NombreCompleto => $"{Apellidos}, {Nombres}";

        [NotMapped]
        public int Edad
        {
            get
            {
                var hoy = DateTime.Today;
                int edad = hoy.Year - FechaNacimiento.Year;
                if (hoy < FechaNacimiento.AddYears(edad)) edad--;
                return edad;
            }
        }

        // Relación 1:N: un paciente puede tener muchas solicitudes de referencia
        public ICollection<SolicitudReferencia> SolicitudesReferencia { get; set; } = new List<SolicitudReferencia>();
    }
}
