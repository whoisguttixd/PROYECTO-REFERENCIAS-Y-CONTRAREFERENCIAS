namespace PROYECTO_REFERENCIAS.Helpers
{
    public static class TextoHelper
    {
        public static bool TieneContenido(string? texto)
        {
            return !string.IsNullOrWhiteSpace(texto);
        }
    }
}