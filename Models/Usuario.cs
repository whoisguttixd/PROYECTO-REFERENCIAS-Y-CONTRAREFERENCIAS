using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_REFERENCIAS.Models
{
    public class Usuario
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }
        [Required, StringLength(50)]
        public string NomUsuario { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string Correo { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public int Telefono { get; set; }
        //Relacion de M : 1 con Roles
        public int IdRol {  get; set; }
        public Rol? Rol { get; set; }
    }
}