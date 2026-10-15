namespace PROYECTO_REFERENCIAS.ViewModels
{
    public class UsuarioVM
    {
        public string NomUsuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RepPassword { get; set; } = string.Empty;
        public int Telefono { get; set; }
        public int IdRol { get; set; }
    }
}
