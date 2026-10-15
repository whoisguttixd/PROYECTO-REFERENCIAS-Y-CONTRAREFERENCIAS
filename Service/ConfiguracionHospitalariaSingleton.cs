using System;
using System.Collections.Generic;
using System.Linq;

namespace PROYECTO_REFERENCIAS.Service
{
    /// <summary>
    /// Patrón Singleton: mantiene en memoria los parámetros globales del HRC (tiempos máximos de atención
    /// por prioridad, catálogo base CIE-10 frecuente y parámetros de la Norma Técnica MINSA de Referencia y
    /// Contra-referencia). Se registra como Singleton en el contenedor DI (Program.cs) para que exista una
    /// única instancia compartida durante toda la vida de la aplicación.
    ///
    /// GRASP: Experto en Información - crea el experto encargado de conocer y proveer la configuración
    /// normativa del sistema (SLA, códigos CIE-10, normas técnicas).
    /// </summary>
    public class ConfiguracionHospitalariaSingleton
    {
        // Instancia única explícita (además del registro DI como Singleton), con estado protegido por lock.
        private static readonly Lazy<ConfiguracionHospitalariaSingleton> _instancia =
            new(() => new ConfiguracionHospitalariaSingleton());
        public static ConfiguracionHospitalariaSingleton Instancia => _instancia.Value;

        private static readonly object _lock = new object();

        public Dictionary<string, int> TiempoSlaPorPrioridad { get; } = new(StringComparer.OrdinalIgnoreCase)
        {
            ["EmergenciaCritica"] = 2,
            ["UrgenciaMayor"] = 24,
            ["ConsultaExterna"] = 168
        };

        /// <summary>
        /// Parámetros de la Norma Técnica de Salud (NTS N° 018-MINSA/DGSP-V.01) aplicados por el HRC.
        /// </summary>
        public Dictionary<string, string> ParametrosNormaTecnica { get; } = new(StringComparer.OrdinalIgnoreCase)
        {
            ["NTS"] = "NTS N° 018-MINSA/DGSP-V.01 - Sistema de Referencia y Contra-referencia",
            ["SLA_I"] = "Prioridad I (Emergencia): atención máxima 2 horas",
            ["SLA_II"] = "Prioridad II (Urgencia Mayor): atención máxima 24 horas",
            ["SLA_III"] = "Prioridad III (Consulta Externa): atención máxima 168 horas (7 días)"
        };

        public Dictionary<string, string> CodigosCie10Frecuentes { get; } = new(StringComparer.OrdinalIgnoreCase)
        {
            ["A00"] = "Cólera",
            ["A01"] = "Fiebre tifoidea y paratifoidea",
            ["A09"] = "Diarrea y gastroenteritis de presunta causa infecciosa",
            ["J06"] = "Infección aguda de vías respiratorias superiores",
            ["J18"] = "Neumonía, agente no especificado",
            ["J45"] = "Asma",
            ["I10"] = "Hipertensión esencial (primaria)",
            ["I25"] = "Enfermedad isquémica crónica del corazón",
            ["I50"] = "Insuficiencia cardíaca",
            ["K21"] = "Reflujo gastroesofágico",
            ["K35"] = "Apendicitis aguda",
            ["K80"] = "Colelitiasis",
            ["K81"] = "Colecistitis",
            ["N18"] = "Insuficiencia renal crónica",
            ["R10"] = "Dolor abdominal",
            ["R11"] = "Náusea y vómito",
            ["R50"] = "Fiebre no clasificada en otra parte",
            ["R51"] = "Cefalea",
            ["R55"] = "Síncope y colapso",
            ["O80"] = "Parto único espontáneo",
            ["P07"] = "Trastornos relacionados con prematurez",
            ["S06"] = "Traumatismo intracraneal"
        };

        public int MaxHorasRespuestaEmergenciaCritica => TiempoSlaPorPrioridad.GetValueOrDefault("EmergenciaCritica", 2);
        public int MaxHorasRespuestaUrgenciaMayor => TiempoSlaPorPrioridad.GetValueOrDefault("UrgenciaMayor", 24);
        public int MaxHorasRespuestaConsultaExterna => TiempoSlaPorPrioridad.GetValueOrDefault("ConsultaExterna", 168);

        /// <summary>
        /// Devuelve el SLA en horas para una prioridad dada.
        /// </summary>
        public int GetSlaHoras(Models.PrioridadReferencia prioridad)
        {
            var key = prioridad switch
            {
                Models.PrioridadReferencia.EmergenciaCritica => "EmergenciaCritica",
                Models.PrioridadReferencia.UrgenciaMayor => "UrgenciaMayor",
                _ => "ConsultaExterna"
            };
            lock (_lock)
            {
                return TiempoSlaPorPrioridad.GetValueOrDefault(key, 168);
            }
        }

        /// <summary>
        /// Devuelve la descripción (humana) de un código CIE-10 frecuente.
        /// </summary>
        public string? GetDescripcionCie10(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;
            lock (_lock)
            {
                return CodigosCie10Frecuentes.TryGetValue(codigo, out var desc) ? desc : null;
            }
        }

        /// <summary>
        /// Agrega un código CIE-10 frecuente al cache (demuestra que el singleton mantiene estado interno).
        /// </summary>
        public void AgregarCie10Frecuente(string codigo, string descripcion)
        {
            lock (_lock)
            {
                CodigosCie10Frecuentes[codigo] = descripcion;
            }
        }

        /// <summary>
        /// Indica si un código CIE-10 marca signos de alarma según la normativa MINSA.
        /// </summary>
        public bool CodigoMarcasAlarma(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return false;
            var baseCode = codigo.TrimEnd('.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9').ToUpperInvariant();
            return baseCode.StartsWith("R10") || baseCode.StartsWith("R50") || baseCode.StartsWith("R55")
                || baseCode.StartsWith("I25") || baseCode.StartsWith("I50") || baseCode.StartsWith("S06");
        }
    }
}
