using PROYECTO_REFERENCIAS.Models;
using Microsoft.EntityFrameworkCore;

namespace PROYECTO_REFERENCIAS.Data
{
    /// <summary>
    /// Seeder ejecutado en Program.cs tras EnsureCreated/ Migrate para cargar datos semilla realistas
    /// de la red hospitalaria de Cajamarca (HRC, Chota, Jaén, Bambamarca, pacientes, médicos y referencias de prueba).
    /// </summary>
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context, bool force = false)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Roles ya están en HasData del DbContext; validar que existan
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Rol { IdRol = 1, NomRol = "Administrador" },
                    new Rol { IdRol = 2, NomRol = "Empleado" },
                    new Rol { IdRol = 3, NomRol = "Usuario" });
                await context.SaveChangesAsync();
            }

            // Pacientes semilla
            if (!context.Pacientes.Any())
            {
                var hoy = DateTime.Today;
                var pacientes = new List<Paciente>
                {
                    new Paciente { Dni = "12345678", Apellidos = "Quispe Mamani", Nombres = "Mario Ernesto", FechaNacimiento = hoy.AddYears(-35), Sexo = "M", Telefono = "987654321", Direccion = "Jr. San Martín 234, El Centro", AlergiaPrincipal = "Penicilina", CodigoRenaes = "REN-001-78" },
                    new Paciente { Dni = "23456789", Apellidos = "Choquehuana Rojas", Nombres = "Lucía Fernanda", FechaNacimiento = hoy.AddYears(-8), Sexo = "F", Telefono = "912345678", Direccion = "Av. Universitaria 123, Chota", AlergiaPrincipal = "Sulfas", CodigoRenaes = "REN-002-12" },
                    new Paciente { Dni = "34567890", Apellidos = "Condori Ayala", Nombres = "Pedro Alberto", FechaNacimiento = hoy.AddYears(-45), Sexo = "M", Telefono = "998877665", Direccion = "Dpto. Las Brisas, Jaén", AlergiaPrincipal = "", CodigoRenaes = "REN-003-45" },
                    new Paciente { Dni = "45678901", Apellidos = "Huamán Ccora", Nombres = "Mónica Isabel", FechaNacimiento = hoy.AddYears(-28), Sexo = "F", Telefono = "987123456", Direccion = "Caserío La Esperanza, Bambamarca", AlergiaPrincipal = "Aspirina", CodigoRenaes = "REN-004-28" },
                    new Paciente { Dni = "56789012", Apellidos = "Tutors Quispe", Nombres = "José Luis", FechaNacimiento = hoy.AddYears(-60), Sexo = "M", Telefono = "965432109", Direccion = "Jr. Libertad 89, Jaén", AlergiaPrincipal = "", CodigoRenaes = "REN-005-09" },
                    new Paciente { Dni = "67890123", Apellidos = "Varillas Huamaní", Nombres = "Rosa Elena", FechaNacimiento = hoy.AddYears(-6), Sexo = "F", Telefono = "981234567", Direccion = "Av. Chota 456, Chota", AlergiaPrincipal = "Ibuprofeno", CodigoRenaes = "REN-006-10" },
                };
                context.Pacientes.AddRange(pacientes);
                await context.SaveChangesAsync();
            }

            // Usuarios demo del sistema (para probar roles Admin/Empleado/Usuario)
            if (!context.Usuarios.Any())
            {
                var usuarios = new List<Usuario>
                {
                    new Usuario { NomUsuario = "AdminHRC", Correo = "admin@hrc.gob.pe", Password = "admin123", Telefono = 042411111, IdRol = 1 },
                    new Usuario { NomUsuario = "MedicoEvaluador", Correo = "medico@hrc.gob.pe", Password = "medico123", Telefono = 042411222, IdRol = 2 },
                    new Usuario { NomUsuario = "CentroOrigen", Correo = "centro@salud.gob.pe", Password = "centro123", Telefono = 042555001, IdRol = 3 },
                };
                context.Usuarios.AddRange(usuarios);
                await context.SaveChangesAsync();
            }

            // Referencias de prueba con detalles CIE-10 y máquina de estados
            if (!context.SolicitudesReferencia.Any())
            {
                var referencias = new List<SolicitudReferencia>
                {
                    new SolicitudReferencia
                    {
                        NumeroReferencia = "HRC-0001",
                        FechaRegistro = DateTime.Today.AddDays(-2),
                        FechaDerivacion = DateTime.Today.AddDays(-2),
                        Prioridad = PrioridadReferencia.SinCalcular,
                        PrioridadCalculada = null,
                        Requerimiento = "EXP-1234",
                        UbicacionUrgencia = "Urgencias",
                        Motivo = "Dolor abdominal intenso, fiebre, nausea y vómito. Sospecha de apendicitis aguda.",
                        OrdenMedica = "Tomografía abdominal, laboratorio CBC y Proteína C reactiva.",
                        ObservacionGeneral = "Paciente con fiebre de 38.5°C en la evaluación.",
                        PresionArterial = "110/70",
                        FrecuenciaCardiaca = 104,
                        FrecuenciaRespiratoria = 22,
                        Temperatura = 38.5m,
                        SaturacionOxigeno = 96,
                        PesoKg = 72.5m,
                        TallaCm = 168m,
                        IdMedicoSolicitante = 8,
                        Estado = EstadoReferencia.EnEvaluacion,
                        FechaEstado = DateTime.Today.AddDays(-1),
                        ObservacionEstado = "En evaluación por médico de guardia.",
                        ObservacionSistema = "Referencia auto-generada sistema.",
                        IdPaciente = 1,
                        IdEstablecimientoOrigen = 2,
                        IdEstablecimientoDestino = 1
                    },
                    new SolicitudReferencia
                    {
                        NumeroReferencia = "HRC-0002",
                        FechaRegistro = DateTime.Today.AddDays(-1),
                        FechaDerivacion = DateTime.Today.AddDays(-1),
                        Prioridad = PrioridadReferencia.SinCalcular,
                        PrioridadCalculada = null,
                        Requerimiento = "EXP-1235",
                        UbicacionUrgencia = "",
                        Motivo = "Niña de 8 años con infección respiratoria alta y fiebre persistente.",
                        OrdenMedica = "Radiografía de tórax, recuento leucocitario.",
                        ObservacionGeneral = "Estado general conservado, signos vitales estables.",
                        PresionArterial = "95/60",
                        FrecuenciaCardiaca = 98,
                        FrecuenciaRespiratoria = 20,
                        Temperatura = 37.8m,
                        SaturacionOxigeno = 98,
                        PesoKg = 26.4m,
                        TallaCm = 128m,
                        IdMedicoSolicitante = 6,
                        Estado = EstadoReferencia.Registrada,
                        FechaEstado = DateTime.Today,
                        IdPaciente = 2,
                        IdEstablecimientoOrigen = 2,
                        IdEstablecimientoDestino = 1
                    },
                    new SolicitudReferencia
                    {
                        NumeroReferencia = "HRC-0003",
                        FechaRegistro = DateTime.Today.AddDays(-3),
                        FechaDerivacion = DateTime.Today.AddDays(-3),
                        Prioridad = PrioridadReferencia.SinCalcular,
                        PrioridadCalculada = null,
                        Requerimiento = "EXP-1236",
                        UbicacionUrgencia = "Urgencias",
                        Motivo = "Paciente de 60 años con dolor torácico y palpitaciones. Sospecha de cardiopatía isquémica.",
                        OrdenMedica = "Electrocardiograma, enzimas cardíacas, ecocardiograma.",
                        ObservacionGeneral = "Signos vitales: PA 100/60, FC 112 lpm.",
                        PresionArterial = "100/60",
                        FrecuenciaCardiaca = 112,
                        FrecuenciaRespiratoria = 24,
                        Temperatura = 36.8m,
                        SaturacionOxigeno = 93,
                        PesoKg = 80.0m,
                        TallaCm = 172m,
                        IdMedicoSolicitante = 5,
                        Estado = EstadoReferencia.Aceptada,
                        FechaEstado = DateTime.Today.AddDays(-2),
                        FechaAceptacion = DateTime.Today.AddDays(-2),
                        ObservacionEstado = "Aceptado para evaluación cardiológica.",
                        IdPaciente = 3,
                        IdEstablecimientoOrigen = 4,
                        IdEstablecimientoDestino = 1
                    },
                    new SolicitudReferencia
                    {
                        NumeroReferencia = "HRC-0004",
                        FechaRegistro = DateTime.Today,
                        FechaDerivacion = DateTime.Today,
                        Prioridad = PrioridadReferencia.SinCalcular,
                        PrioridadCalculada = null,
                        Requerimiento = "EXP-1237",
                        UbicacionUrgencia = "Observación",
                        Motivo = "Paciente con dolor abdominal en fosa ilíaca derecha; requiere evaluación quirúrgica especializada.",
                        OrdenMedica = "Hemograma completo, ecografía abdominal.",
                        ObservacionGeneral = "Referido desde el Hospital Tito Villar Cabezas de Bambamarca.",
                        PresionArterial = "118/76",
                        FrecuenciaCardiaca = 88,
                        FrecuenciaRespiratoria = 18,
                        Temperatura = 37.4m,
                        SaturacionOxigeno = 97,
                        PesoKg = 65.0m,
                        TallaCm = 160m,
                        IdMedicoSolicitante = 7,
                        Estado = EstadoReferencia.Registrada,
                        FechaEstado = DateTime.Today,
                        IdPaciente = 4,
                        IdEstablecimientoOrigen = 5,
                        IdEstablecimientoDestino = 1
                    },
                };
                context.SolicitudesReferencia.AddRange(referencias);
                await context.SaveChangesAsync();

                // Detalles CIE-10 (Maestro-Detalle)
                var det1 = context.SolicitudesReferencia.First(s => s.NumeroReferencia == "HRC-0001");
                var det2 = context.SolicitudesReferencia.First(s => s.NumeroReferencia == "HRC-0002");
                var det3 = context.SolicitudesReferencia.First(s => s.NumeroReferencia == "HRC-0003");
                var det4 = context.SolicitudesReferencia.First(s => s.NumeroReferencia == "HRC-0004");

                var detalles = new List<DetalleReferencia>
                {
                    new DetalleReferencia
                    {
                        IdReferencia = det1.IdReferencia,
                        CodigoCie10 = "R10.1",
                        Diagnostico = "Dolor agudo en omento",
                        TipoApoyo = "Examen físico",
                        TipoDiagnostico = "Presuntivo",
                        DescripcionApoyo = "Ruptura abdominal positiva, defensa muscular",
                        ExamenAuxiliar = "Ecografía abdominal (informe N° 4412)",
                        HallazgosClinicos = "Dolor en fosa ilíaca derecha, signo de Blumberg positivo.",
                        Observaciones = "Dolor en fosa ilíaca derecha",
                        SignosAlarma = true,
                        PrioridadDetalle = PrioridadReferencia.EmergenciaCritica
                    },
                    new DetalleReferencia
                    {
                        IdReferencia = det1.IdReferencia,
                        CodigoCie10 = "R50.9",
                        Diagnostico = "Fiebre de causa desconocida",
                        TipoApoyo = "Signos vitales",
                        DescripcionApoyo = "T 38.5°C",
                        Observaciones = "Fiebre persistente",
                        SignosAlarma = true,
                        PrioridadDetalle = PrioridadReferencia.EmergenciaCritica
                    },
                    new DetalleReferencia
                    {
                        IdReferencia = det2.IdReferencia,
                        CodigoCie10 = "J06",
                        Diagnostico = "Infección aguda de vías respiratorias superiores",
                        TipoApoyo = "Examen físico",
                        DescripcionApoyo = "Rinitis, faringitis",
                        Observaciones = "Niña febril, pero estable",
                        SignosAlarma = false,
                        PrioridadDetalle = PrioridadReferencia.ConsultaExterna
                    },
                    new DetalleReferencia
                    {
                        IdReferencia = det2.IdReferencia,
                        CodigoCie10 = "R50.9",
                        Diagnostico = "Fiebre",
                        TipoApoyo = "Signos vitales",
                        DescripcionApoyo = "T 37.8°C",
                        Observaciones = "Fiebre de tiempo prolongado",
                        SignosAlarma = false,
                        PrioridadDetalle = PrioridadReferencia.ConsultaExterna
                    },
                    new DetalleReferencia
                    {
                        IdReferencia = det3.IdReferencia,
                        CodigoCie10 = "I25",
                        Diagnostico = "Enfermedad isquémica crónica del corazón",
                        TipoApoyo = "Examen complementario",
                        TipoDiagnostico = "Definitivo",
                        DescripcionApoyo = "ECG con alteraciones isquémicas",
                        ExamenAuxiliar = "ECG de 12 derivaciones (informe N° 7781)",
                        HallazgosClinicos = "Supradesnivel del ST en cara inferior.",
                        Observaciones = "Sospecha de IAM en evolución",
                        SignosAlarma = true,
                        PrioridadDetalle = PrioridadReferencia.EmergenciaCritica
                    },
                    new DetalleReferencia
                    {
                        IdReferencia = det3.IdReferencia,
                        CodigoCie10 = "R55.9",
                        Diagnostico = "Síncope",
                        TipoApoyo = "Examen físico",
                        DescripcionApoyo = "Episodio sincopal en urgencias",
                        Observaciones = "Pérdida de conocimiento breve",
                        SignosAlarma = true,
                        PrioridadDetalle = PrioridadReferencia.EmergenciaCritica
                    },
                    new DetalleReferencia
                    {
                        IdReferencia = det4.IdReferencia,
                        CodigoCie10 = "K35",
                        Diagnostico = "Apendicitis aguda",
                        TipoApoyo = "Imagenología",
                        TipoDiagnostico = "Presuntivo",
                        DescripcionApoyo = "Ecografía abdominal",
                        ExamenAuxiliar = "Ecografía abdominal (informe N° 5590)",
                        HallazgosClinicos = "Apéndice aumentado de tamaño, líquido libre periapendicular.",
                        Observaciones = "Requiere evaluación quirúrgica.",
                        SignosAlarma = true,
                        PrioridadDetalle = PrioridadReferencia.EmergenciaCritica
                    },
                };
                context.DetallesReferencia.AddRange(detalles);
                await context.SaveChangesAsync();

                // Contra-referencia (1:1) ejemplo para HRC-0003
                if (!context.Contrareferencias.Any())
                {
                    var refAceptada = context.SolicitudesReferencia.First(s => s.NumeroReferencia == "HRC-0003");
                    var contra = new Contrareferencia
                    {
                        IdReferencia = refAceptada.IdReferencia,
                        FechaEmision = DateTime.Today.AddDays(-1),
                        Resultado = "Paciente hospitalizado en UCI, coronariografía realizada, angioplastia exitosa.",
                        IndicacionCentroOrigen = "Continuar medicación antiagregante, control de factores de riesgo cardiovascular, seguimiento a 30 días.",
                        EvolucionClinica = "Evolución hospitalaria favorable, alta clínica sin complicaciones.",
                        TipoEgreso = "Alta médica"
                    };
                    // Relación 1:1: la Contrareferencia se asocia por su FK (IdReferencia).
                    context.Contrareferencias.Add(contra);
                    refAceptada.Contrareferencia = contra;
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
