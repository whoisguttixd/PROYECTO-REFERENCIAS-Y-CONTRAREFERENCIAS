using System;
using System.Collections.Generic;
using System.Linq;
using PROYECTO_REFERENCIAS.Models;
using PROYECTO_REFERENCIAS.Service.Estrategias;

namespace PROYECTO_REFERENCIAS.Service
{
    /// <summary>
    /// Servicio de dominio para la Gestión de la Solicitud de Referencia Interhospitalaria del HRC.
    /// Patrones implementados:
    ///   - Strategy: selecciona e invoca el algoritmo de triaje (<see cref="IPrioridadTriajeStrategy"/>) según el contenido.
    ///   - GRASP Controlador: concentra la lógica de cambio de estado y validaciones de reglas de negocio.
    ///   - GRASP Experto en Información: calcula la prioridad final a partir de los detalles clínicos.
    /// </summary>
    public class SolicitudReferenciaService
    {
        private readonly ConfiguracionHospitalariaSingleton _config;

        public SolicitudReferenciaService(ConfiguracionHospitalariaSingleton config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Patrón Strategy: calcula la prioridad clínica de una solicitud seleccionando dinámicamente
        /// la estrategia de triaje adecuada según la gravedad de sus detalles.
        /// </summary>
        public ResultadoTriaje CalcularPrioridad(SolicitudReferencia solicitud)
        {
            if (solicitud == null) throw new ArgumentNullException(nameof(solicitud));

            var detalles = solicitud.Detalles?.ToList() ?? new List<DetalleReferencia>();

            var haySignosAlarma = detalles.Any(d => d.SignosAlarma);
            var hayDiagnosticoCritico = detalles.Any(d =>
                CodigosCriticos().Any(c => d.CodigoCie10.StartsWith(c, StringComparison.OrdinalIgnoreCase)));

            IPrioridadTriajeStrategy strategy;

            if (haySignosAlarma && hayDiagnosticoCritico)
            {
                // Strategy de emergencia (Prioridad I)
                strategy = new TriajeEmergenciaPrioridadIStrategy();
            }
            else if (haySignosAlarma || hayDiagnosticoCritico)
            {
                // Strategy de urgencia mayor (Prioridad II)
                strategy = new TriajeUrgenciaPrioridadIIStrategy();
            }
            else
            {
                // Strategy por defecto (Prioridad III)
                strategy = new TriajeConsultaExternaPrioridadIIIStrategy();
            }

            var resultado = strategy.Calcular(detalles);

            // GRASP Experto en Información: el servicio decide la prioridad final.
            // Si la solicitud ya tenía una prioridad mayor, se respeta.
            if (solicitud.PrioridadCalculada.HasValue &&
                resultado.Prioridad < solicitud.PrioridadCalculada.Value)
            {
                resultado.Prioridad = solicitud.PrioridadCalculada.Value;
            }

            return resultado;
        }

        /// <summary>
        /// Máquina de estados (Registrada -> EnEvaluacion -> Aceptada/Observada/Rechazada -> Contrareferida).
        /// Valida la transición, actualiza estado, fecha y auditoría.
        /// </summary>
        public static bool ProcesarTransicionEstado(SolicitudReferencia solicitud, EstadoReferencia nuevoEstado,
            string? observacion, out string mensaje)
        {
            mensaje = string.Empty;

            if (solicitud == null)
            {
                mensaje = "La solicitud es nula.";
                return false;
            }

            var permitidoDesde = new Dictionary<EstadoReferencia, List<EstadoReferencia>>
            {
                { EstadoReferencia.Registrada, new List<EstadoReferencia> { EstadoReferencia.EnEvaluacion } },
                { EstadoReferencia.EnEvaluacion, new List<EstadoReferencia> { EstadoReferencia.Aceptada, EstadoReferencia.Observada, EstadoReferencia.Rechazada } },
                { EstadoReferencia.Observada, new List<EstadoReferencia> { EstadoReferencia.Aceptada, EstadoReferencia.Contrareferida } },
                { EstadoReferencia.Aceptada, new List<EstadoReferencia> { EstadoReferencia.Contrareferida } },
                { EstadoReferencia.Rechazada, new List<EstadoReferencia>() }
            };

            if (!permitidoDesde.ContainsKey(solicitud.Estado))
            {
                mensaje = $"No se puede cambiar el estado desde '{solicitud.Estado}'.";
                return false;
            }

            if (!permitidoDesde[solicitud.Estado].Contains(nuevoEstado))
            {
                mensaje = $"Transición no permitida: '{solicitud.Estado}' -> '{nuevoEstado}'.";
                return false;
            }

            if (nuevoEstado == EstadoReferencia.Contrareferida && solicitud.Contrareferencia == null)
            {
                mensaje = "No se puede contrareferir una referencia sin Contra-referencia emitida.";
                return false;
            }

            if (nuevoEstado == EstadoReferencia.Aceptada)
            {
                if (solicitud.FechaDerivacion > DateTime.Today)
                {
                    mensaje = "No se puede aceptar una referencia cuya fecha de derivación es futura.";
                    return false;
                }
                solicitud.FechaAceptacion = DateTime.Today;
            }

            if (nuevoEstado == EstadoReferencia.Contrareferida)
                solicitud.FechaContrareferencia = DateTime.Today;

            solicitud.Estado = nuevoEstado;
            solicitud.FechaEstado = DateTime.Today;
            solicitud.ObservacionEstado = observacion;
            solicitud.ObservacionSistema = ModificarAuditoria(solicitud, observacion ?? string.Empty);

            mensaje = $"Transición realizada: {solicitud.Estado}.";
            return true;
        }

        /// <summary>
        /// Descripción de etiqueta MINSA CIE-10 frecuente para observaciones de auditoría.
        /// </summary>
        public string ObtenerDescripcionCie10DesdeDetalle(DetalleReferencia d)
        {
            if (d == null) throw new ArgumentNullException(nameof(d));
            return _config.GetDescripcionCie10(d.CodigoCie10) ?? d.CodigoCie10;
        }

        public static string ModificarAuditoria(SolicitudReferencia solicitud, string? observacion)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {observacion}";
        }

        /// <summary>
        /// Valida que origen y destino sean distintos y que las fechas sean coherentes.
        /// </summary>
        public static List<string> ValidarNegocioCorto(SolicitudReferencia solicitud)
        {
            var errores = new List<string>();

            if (solicitud == null) return new List<string> { "La solicitud es nula." };

            if (solicitud.IdEstablecimientoOrigen == solicitud.IdEstablecimientoDestino)
                errores.Add("El Establecimiento de Origen no puede ser igual al Establecimiento de Destino.");

            if (solicitud.FechaDerivacion < solicitud.FechaRegistro)
                errores.Add("La fecha de derivación no puede ser anterior a la fecha de registro.");

            if (!solicitud.NumeroReferencia.StartsWith("HRC-"))
                errores.Add("El número de referencia debe tener formato HRC-XXXX.");

            if (solicitud.Detalles == null || !solicitud.Detalles.Any())
                errores.Add("La solicitud debe tener al menos un detalle clínico.");

            return errores;
        }

        /// <summary>
        /// Devuelve el algoritmo de triaje preferido según el diagnóstico principal del primer detalle.
        /// </summary>
        public IPrioridadTriajeStrategy EstrategiaPorDefectoPara(SolicitudReferencia solicitud)
        {
            var detalles = solicitud.Detalles?.ToList() ?? new List<DetalleReferencia>();
            if (!detalles.Any())
                return new TriajeConsultaExternaPrioridadIIIStrategy();

            var primero = detalles.First();
            bool critico = CodigosCriticos().Any(c => primero.CodigoCie10.StartsWith(c, StringComparison.OrdinalIgnoreCase));
            bool urgencia = CodigosUrgencia().Any(c => primero.CodigoCie10.StartsWith(c, StringComparison.OrdinalIgnoreCase));

            if (critico) return new TriajeEmergenciaPrioridadIStrategy();
            if (urgencia) return new TriajeUrgenciaPrioridadIIStrategy();
            return new TriajeConsultaExternaPrioridadIIIStrategy();
        }

        private static IEnumerable<string> CodigosCriticos() => new List<string>
        {
            "R50","R55","I50","I25","I95","I96","K35","K80","K81","A00","A01"
        };

        private static IEnumerable<string> CodigosUrgencia() => new List<string>
        {
            "R10","R11","K21","K80","K81","N18","N39","J18","J20","J45"
        };
    }
}
