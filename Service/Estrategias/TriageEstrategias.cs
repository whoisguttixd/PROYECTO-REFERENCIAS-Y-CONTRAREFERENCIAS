using System;
using System.Collections.Generic;
using System.Linq;

namespace PROYECTO_REFERENCIAS.Service.Estrategias
{
    /// <summary>
    /// Patrón Strategy (implementación): Triaje de EMERGENCIA (Prioridad I).
    /// Calcula automáticamente el puntaje de gravedad, el SLA máximo en horas, la prioridad y el
    /// protocolo de recepción para una referencia con signos de alarma graves o diagnósticos críticos.
    /// </summary>
    public class TriajeEmergenciaPrioridadIStrategy : IPrioridadTriajeStrategy
    {
        /// <summary>
        /// Patrón Strategy: cada clase implementa el mismo contrato (IPrioridadTriajeStrategy) con su propia lógica.
        /// Los algoritmos se intercambian en el servicio de dominio, demostrando el poder del patrón.
        /// </summary>
        public string NombreEstrategia => "TriajeEmergenciaPrioridadI";

        private static readonly HashSet<string> CodigosCriticos = new(StringComparer.OrdinalIgnoreCase)
        {
            "R50", "R55", "I50", "I25", "I95", "I96", "K35", "K80", "K81", "A00", "A01", "Z02", "Z03"
        };

        public ResultadoTriaje Calcular(List<Models.DetalleReferencia> detalles)
        {
            var gravedad = 0;
            var criterios = new List<string>();

            if (detalles == null || detalles.Count == 0)
                return new ResultadoTriaje
                {
                    PuntajeGravedad = 0,
                    Prioridad = Models.PrioridadReferencia.ConsultaExterna,
                    MaxHorasRespuesta = 168,
                    ProtocoloRecepcion = "Atención programada en consulta externa.",
                    Justificacion = "Sin detalles evaluables: se asigna Consulta Externa."
                };

            foreach (var d in detalles)
            {
                if (d == null) continue;
                if (d.SignosAlarma)
                {
                    gravedad += 4;
                    criterios.Add($"Detalle {d.CodigoCie10}: signos de alarma presentes.");
                }

                if (CodigosCriticos.Any(c => d.CodigoCie10.StartsWith(c, StringComparison.OrdinalIgnoreCase)))
                {
                    gravedad += 5;
                    criterios.Add($"Detalle {d.CodigoCie10}: diagnóstico de riesgo vital.");
                }
            }

            var numSignos = detalles.Count(d => d.SignosAlarma);
            if (numSignos > (detalles.Count / 2))
            {
                gravedad += 3;
                criterios.Add($"Signos de alarma en la mayoría de los detalles ({numSignos}/{detalles.Count}).");
            }

            if (gravedad >= 7)
                return new ResultadoTriaje
                {
                    PuntajeGravedad = gravedad,
                    Prioridad = Models.PrioridadReferencia.EmergenciaCritica,
                    MaxHorasRespuesta = 2,
                    ProtocoloRecepcion = "Activación inmediata de la Unidad de Emergencia, reanimación y evaluación por especialista de guardia. Máximo 2 horas para la admisión en el HRC.",
                    Justificacion = string.Join(" ", criterios) + $" Puntaje clínico: {gravedad}."
                };

            if (gravedad >= 4)
                return new ResultadoTriaje
                {
                    PuntajeGravedad = gravedad,
                    Prioridad = Models.PrioridadReferencia.UrgenciaMayor,
                    MaxHorasRespuesta = 24,
                    ProtocoloRecepcion = "Atención por urgencias en menos de 24 horas; preparar carro de paro y monitorización.",
                    Justificacion = string.Join(" ", criterios) + $" Puntaje clínico: {gravedad}."
                };

            return new ResultadoTriaje
            {
                PuntajeGravedad = gravedad,
                Prioridad = Models.PrioridadReferencia.ConsultaExterna,
                MaxHorasRespuesta = 168,
                ProtocoloRecepcion = "Atención programada en consulta externa; agenda dentro de 7 días.",
                Justificacion = string.Join(" ", criterios) + $" Puntaje clínico: {gravedad}."
            };
        }
    }

    /// <summary>
    /// Patrón Strategy (implementación): Triaje de URGENCIA MAYOR (Prioridad II).
    /// Se aplica cuando existen signos de alarma moderados o diagnósticos que requieren atención urgente.
    /// </summary>
    public class TriajeUrgenciaPrioridadIIStrategy : IPrioridadTriajeStrategy
    {
        /// <summary>
        /// Estrategia intercambiable del patrón Strategy.
        /// </summary>
        public string NombreEstrategia => "TriajeUrgenciaPrioridadII";

        private static readonly HashSet<string> CodigosUrgencia = new(StringComparer.OrdinalIgnoreCase)
        {
            "R10", "R11", "K21", "K80", "K81", "N18", "N39", "J18", "J20", "J45"
        };

        public ResultadoTriaje Calcular(List<Models.DetalleReferencia> detalles)
        {
            var gravedad = 0;
            var criterios = new List<string>();

            if (detalles == null || detalles.Count == 0)
                return new ResultadoTriaje
                {
                    PuntajeGravedad = 0,
                    Prioridad = Models.PrioridadReferencia.ConsultaExterna,
                    MaxHorasRespuesta = 168,
                    ProtocoloRecepcion = "Atención programada en consulta externa.",
                    Justificacion = "Sin detalles evaluables."
                };

            foreach (var d in detalles)
            {
                if (d == null) continue;
                if (d.SignosAlarma) gravedad += 2;

                if (CodigosUrgencia.Any(c => d.CodigoCie10.StartsWith(c, StringComparison.OrdinalIgnoreCase)))
                {
                    gravedad += 3;
                    criterios.Add($"Detalle {d.CodigoCie10}: diagnóstico de atención urgente.");
                }
            }

            if (gravedad >= 5)
                return new ResultadoTriaje
                {
                    PuntajeGravedad = gravedad,
                    Prioridad = Models.PrioridadReferencia.UrgenciaMayor,
                    MaxHorasRespuesta = 24,
                    ProtocoloRecepcion = "Atención por emergencia/urgencias en menos de 24 horas con evaluación por especialista.",
                    Justificacion = string.Join(" ", criterios) + $" Puntaje clínico: {gravedad}."
                };

            return new ResultadoTriaje
            {
                PuntajeGravedad = gravedad,
                Prioridad = Models.PrioridadReferencia.ConsultaExterna,
                MaxHorasRespuesta = 168,
                ProtocoloRecepcion = "Atención programada en consulta externa.",
                Justificacion = string.Join(" ", criterios) + $" Puntaje clínico: {gravedad}."
            };
        }
    }

    /// <summary>
    /// Patrón Strategy (implementación): Triaje de CONSULTA EXTERNA (Prioridad III).
    /// Se aplica por defecto cuando no hay signos de alarma ni diagnósticos críticos/urgentes.
    /// </summary>
    public class TriajeConsultaExternaPrioridadIIIStrategy : IPrioridadTriajeStrategy
    {
        /// <summary>
        /// Estrategia intercambiable del patrón Strategy.
        /// </summary>
        public string NombreEstrategia => "TriajeConsultaExternaPrioridadIII";

        public ResultadoTriaje Calcular(List<Models.DetalleReferencia> detalles)
        {
            var criterios = new List<string>();
            if (detalles == null || detalles.Count == 0)
                return new ResultadoTriaje
                {
                    PuntajeGravedad = 0,
                    Prioridad = Models.PrioridadReferencia.ConsultaExterna,
                    MaxHorasRespuesta = 168,
                    ProtocoloRecepcion = "Atención programada en consulta externa.",
                    Justificacion = "Sin detalles evaluables. Atención programada."
                };

            var numSignos = detalles.Count(d => d != null && d.SignosAlarma);
            if (numSignos > 0)
                criterios.Add($"Signos de alarma en {numSignos} detalle(s) pero no críticos.");

            // Patrón GRASP: Experto en Información - la decisión de prioridad se basa en los detalles conocidos.
            return new ResultadoTriaje
            {
                PuntajeGravedad = numSignos,
                Prioridad = Models.PrioridadReferencia.ConsultaExterna,
                MaxHorasRespuesta = 168,
                ProtocoloRecepcion = "Atención programada en consulta externa del HRC dentro de 7 días.",
                Justificacion = string.Join(" ", criterios) + " Atención en consulta externa programada."
            };
        }
    }
}
