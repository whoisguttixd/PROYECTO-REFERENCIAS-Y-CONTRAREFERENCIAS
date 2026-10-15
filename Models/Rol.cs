using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_REFERENCIAS.Models
{
    public class Rol
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRol {  get; set; }
        [Required, StringLength(50)]
        public string NomRol {  get; set; } = string.Empty;
        //Relacion de 1 : M con Usuario
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
