using System.Collections.Generic;

namespace PROYECTO_REFERENCIAS.Service
{
    /// <summary>
    /// Patrón Strategy: Interfaz que define el contrato para calcular la prioridad clínica de una
    /// referencia según los signos de alarma y diagnósticos de su detalle. Cada implementación concreta
    /// (Strategy) encapsula un algoritmo distinto de triaje.
    ///
    /// Implementaciones:
    ///   - <see cref="Estrategias.TriajeEmergenciaPrioridadIStrategy"/> (Prioridad I – Emergencia)
    ///   - <see cref="Estrategias.TriajeUrgenciaPrioridadIIStrategy"/> (Prioridad II – Urgencia Mayor)
    ///   - <see cref="Estrategias.TriajeConsultaExternaPrioridadIIIStrategy"/> (Prioridad III – Consulta Externa)
    ///
    /// GRASP: Bajo Acoplamiento - los responsables de calcular la prioridad dependen de esta interfaz
    /// y no de las clases concretas de triaje.
    /// </summary>
    public interface IPrioridadTriajeStrategy
    {
        /// <summary>
        /// Código identificador del triaje (ej. "TriajeEmergenciaPrioridadI").
        /// </summary>
        string NombreEstrategia { get; }

        /// <summary>
        /// Calcula el puntaje clínico, la prioridad resultante, el SLA (horas) y el protocolo de recepción
        /// para la referencia a partir de sus detalles (donde vive la información experta).
        /// </summary>
        ResultadoTriaje Calcular(List<Models.DetalleReferencia> detalles);
    }

    /// <summary>
    /// Resultado del triaje clínico calculado por el patrón Strategy:
    /// puntaje de gravedad, prioridad, tiempo máximo de atención (SLA) y protocolo de recepción.
    /// </summary>
    public class ResultadoTriaje
    {
        public int PuntajeGravedad { get; set; }
        public Models.PrioridadReferencia Prioridad { get; set; } = Models.PrioridadReferencia.ConsultaExterna;
        public int MaxHorasRespuesta { get; set; } = 168;
        public string ProtocoloRecepcion { get; set; } = string.Empty;
        public string Justificacion { get; set; } = string.Empty;
    }
}
