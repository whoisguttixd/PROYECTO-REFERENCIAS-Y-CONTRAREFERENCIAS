using System.Collections.Generic;

namespace PROYECTO_REFERENCIAS.ViewModels.Dashboard
{
    public class IndicadoresViewModel
    {
        public int TotalHoy { get; set; }
        public int TotalSemana { get; set; }
        public int EnEvaluacion { get; set; }
        public int Aceptadas { get; set; }
        public int Observadas { get; set; }
        public int Rechazadas { get; set; }
        public int Contrareferidas { get; set; }
        public int EmergenciaCritica { get; set; }
        public int UrgenciaMayor { get; set; }
        public int ConsultaExterna { get; set; }
        public List<FilaIndicadorRed> ReferenciaPorRed { get; set; } = new();
        public string RenderEstado { get; set; } = string.Empty;
        public string RenderPrioridad { get; set; } = string.Empty;
        public string RenderRed { get; set; } = string.Empty;
    }

    public class FilaIndicadorRed
    {
        public string Red { get; set; } = string.Empty;
        public int Total { get; set; }
        public int EmergenciaCritica { get; set; }
        public int UrgenciaMayor { get; set; }
        public int ConsultaExterna { get; set; }
    }
}
