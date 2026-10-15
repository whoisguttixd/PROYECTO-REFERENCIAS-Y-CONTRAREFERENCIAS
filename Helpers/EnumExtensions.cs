using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PROYECTO_REFERENCIAS.Helpers
{
    /// <summary>
    /// Utilidad de presentación: obtiene el nombre formal ([Display(Name)]) de un valor de enum
    /// para mostrarlo en las vistas (evita mostrar "EnEvaluacion" en vez de "En Evaluación").
    /// GRASP: Bajo Acoplamiento - la capa de presentación no conoce reglas de formato de los enums.
    /// </summary>
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum valor)
        {
            if (valor == null) return string.Empty;

            var tipo = valor.GetType();
            var miembro = tipo.GetMember(valor.ToString());
            if (miembro == null || miembro.Length == 0)
                return valor.ToString();

            var atributo = miembro[0].GetCustomAttribute<DisplayAttribute>();
            return atributo?.Name ?? valor.ToString();
        }
    }
}
