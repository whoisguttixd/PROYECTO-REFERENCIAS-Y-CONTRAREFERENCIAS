using PROYECTO_REFERENCIAS.Models;
using Microsoft.EntityFrameworkCore;

namespace PROYECTO_REFERENCIAS.Data
{
    /// <summary>
    /// Contexto de Entity Framework Core del sistema de Referencias y Contra-referencias Interhospitalarias del HRC.
    /// Configuración mixta: Data Annotations en entidades + Fluent API en OnModelCreating para
    /// DeleteBehavior, tablas intermedias N:M y semilla.
    /// Patrones:
    ///   - GRASP Information Expert: las entidades conocen sus propias reglas de formato (Data Annotations).
    ///   - Repository - este DbContext se envuelve en repositorios (ver Repositorios) para desacoplar el acceso.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }

        // Nuevas entidades hospitalarias
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<EstablecimientoSalud> Establecimientos { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<PersonalSalud> PersonalSalud { get; set; }
        public DbSet<SolicitudReferencia> SolicitudesReferencia { get; set; }
        public DbSet<DetalleReferencia> DetallesReferencia { get; set; }
        public DbSet<Contrareferencia> Contrareferencias { get; set; }
        public DbSet<EstablecimientoEspecialidad> EstablecimientoEspecialidades { get; set; }
        public DbSet<SolicitudReferenciaPersonal> SolicitudReferenciaPersonal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Relación 1:N original (Usuario <-> Rol) =====
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.IdRol)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rol>().HasData(
                new Rol { IdRol = 1, NomRol = "Administrador" },
                new Rol { IdRol = 2, NomRol = "Empleado" },
                new Rol { IdRol = 3, NomRol = "Usuario" });

            // ===== Semilla: roles, estab. de salud, especialidades =====
            modelBuilder.Entity<EstablecimientoSalud>().HasData(
                new EstablecimientoSalud
                {
                    IdEstablecimiento = 1,
                    Codigo = "HRC-DOC",
                    Nombre = "Hospital Regional Docente de Cajamarca",
                    TipoEstablecimiento = "Hospital Regional",
                    RedSalud = "Regional",
                    Ubicacion = "Cajamarca - San Juan",
                    Telefono = "042-411111",
                    RepresentanteLegal = "Dr. Director Médico"
                },
                new EstablecimientoSalud
                {
                    IdEstablecimiento = 2,
                    Codigo = "CS-CH01",
                    Nombre = "Centro de Salud Alto Chota",
                    TipoEstablecimiento = "Centro de Salud",
                    RedSalud = "Chota",
                    Ubicacion = "Chota",
                    Telefono = "042-555001",
                    RepresentanteLegal = "Médico a Cargo"
                },
                new EstablecimientoSalud
                {
                    IdEstablecimiento = 3,
                    Codigo = "HJS-CAD",
                    Nombre = "Hospital José Soto Cadenillas de Chota",
                    TipoEstablecimiento = "Hospital Distrital",
                    RedSalud = "Chota",
                    Ubicacion = "Chota - Caserío El Chacán",
                    Telefono = "042-688002",
                    RepresentanteLegal = "Dr. Jefe de Hospital"
                },
                new EstablecimientoSalud
                {
                    IdEstablecimiento = 4,
                    Codigo = "HG-JAE",
                    Nombre = "Hospital General de Jaén",
                    TipoEstablecimiento = "Hospital Distrital",
                    RedSalud = "Jaén",
                    Ubicacion = "Jaén",
                    Telefono = "042-777300",
                    RepresentanteLegal = "Dr. Jefe de Hospital"
                },
                new EstablecimientoSalud
                {
                    IdEstablecimiento = 5,
                    Codigo = "HTVC-BAM",
                    Nombre = "Hospital Tito Villar Cabezas de Bambamarca",
                    TipoEstablecimiento = "Hospital Distrital",
                    RedSalud = "Bambamarca",
                    Ubicacion = "Bambamarca - Hualgayoc",
                    Telefono = "076-558001",
                    RepresentanteLegal = "Dr. Jefe de Hospital"
                },
                new EstablecimientoSalud
                {
                    IdEstablecimiento = 6,
                    Codigo = "CS-BDI",
                    Nombre = "Centro de Salud Baños del Inca",
                    TipoEstablecimiento = "Centro de Salud",
                    RedSalud = "Cajamarca",
                    Ubicacion = "Baños del Inca",
                    Telefono = "076-346120",
                    RepresentanteLegal = "Médico Jefe"
                });

            modelBuilder.Entity<Especialidad>().HasData(
                new Especialidad { IdEspecialidad = 1, Codigo = "CIE-10-MEDINT", Nombre = "Medicina Interna", Descripcion = "Patologías médicas generales de adultos.", NivelAtencion = "III" },
                new Especialidad { IdEspecialidad = 2, Codigo = "CIE-10-PEDI", Nombre = "Pediatría", Descripcion = "Atención del niño y adolescente.", NivelAtencion = "III" },
                new Especialidad { IdEspecialidad = 3, Codigo = "CIE-10-TRAU", Nombre = "Traumatología", Descripcion = "Lesiones del sistema osteomuscular.", NivelAtencion = "III" },
                new Especialidad { IdEspecialidad = 4, Codigo = "CIE-10-OBST", Nombre = "Obstetricia y Ginecología", Descripcion = "Embarazo, parto y salud femenina.", NivelAtencion = "III" },
                new Especialidad { IdEspecialidad = 5, Codigo = "CIE-10-NEURO", Nombre = "Neurología", Descripcion = "Patologías del sistema nervioso.", NivelAtencion = "III" },
                new Especialidad { IdEspecialidad = 6, Codigo = "CIE-10-CARD", Nombre = "Cardiología", Descripcion = "Patologías cardíacas.", NivelAtencion = "III" },
                new Especialidad { IdEspecialidad = 7, Codigo = "CIE-10-TOXI", Nombre = "Toxicología", Descripcion = "Intoxicaciones y haceres.", NivelAtencion = "II" },
                new Especialidad { IdEspecialidad = 8, Codigo = "CIE-10-UROL", Nombre = "Urología", Descripcion = "Patologías del tracto urinario.", NivelAtencion = "III" });

            modelBuilder.Entity<EstablecimientoEspecialidad>().HasData(
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 1, IdEstablecimiento = 1, IdEspecialidad = 1, CapacidadDisponible = 24, HorarioAtencion = "Lunes a Viernes 07:00-17:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 2, IdEstablecimiento = 1, IdEspecialidad = 2, CapacidadDisponible = 20, HorarioAtencion = "Lunes a Viernes 07:00-17:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 3, IdEstablecimiento = 1, IdEspecialidad = 3, CapacidadDisponible = 16, HorarioAtencion = "Lunes a Sábado 07:00-17:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 4, IdEstablecimiento = 1, IdEspecialidad = 4, CapacidadDisponible = 18, HorarioAtencion = "Lunes a Viernes 07:00-17:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 5, IdEstablecimiento = 1, IdEspecialidad = 5, CapacidadDisponible = 10, HorarioAtencion = "Lunes a Viernes 07:00-17:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 6, IdEstablecimiento = 1, IdEspecialidad = 6, CapacidadDisponible = 10, HorarioAtencion = "Lunes a Viernes 07:00-17:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 7, IdEstablecimiento = 1, IdEspecialidad = 7, CapacidadDisponible = 8, HorarioAtencion = "Lunes a Viernes 07:00-17:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 8, IdEstablecimiento = 1, IdEspecialidad = 8, CapacidadDisponible = 12, HorarioAtencion = "Lunes a Viernes 07:00-17:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 9, IdEstablecimiento = 4, IdEspecialidad = 1, CapacidadDisponible = 4, HorarioAtencion = "Lunes a Viernes 07:00-14:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 10, IdEstablecimiento = 4, IdEspecialidad = 3, CapacidadDisponible = 4, HorarioAtencion = "Lunes a Viernes 07:00-14:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 11, IdEstablecimiento = 5, IdEspecialidad = 1, CapacidadDisponible = 6, HorarioAtencion = "Lunes a Viernes 07:00-14:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 12, IdEstablecimiento = 5, IdEspecialidad = 3, CapacidadDisponible = 4, HorarioAtencion = "Lunes a Viernes 07:00-14:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 13, IdEstablecimiento = 6, IdEspecialidad = 1, CapacidadDisponible = 3, HorarioAtencion = "Lunes a Viernes 07:00-13:00" },
                new EstablecimientoEspecialidad { IdEstablecimientoEspecialidad = 14, IdEstablecimiento = 6, IdEspecialidad = 2, CapacidadDisponible = 3, HorarioAtencion = "Lunes a Viernes 07:00-13:00" });

            modelBuilder.Entity<PersonalSalud>().HasData(
                new PersonalSalud { IdPersonal = 1, Codigo = "MED-001", Apellidos = "Quispe Mamani", Nombres = "Luis Alberto", ColegioProfesional = "CMP", NumeroColegiado = "88301", TipoPersonal = "Médico", IdEstablecimiento = 1, EspecialidadPrincipal = "Medicina Interna", Sip = "SIP-01" },
                new PersonalSalud { IdPersonal = 2, Codigo = "MED-002", Apellidos = "Choquehuanca Rojas", Nombres = "María Elena", ColegioProfesional = "CMP", NumeroColegiado = "88452", TipoPersonal = "Médico", IdEstablecimiento = 1, EspecialidadPrincipal = "Pediatría", Sip = "SIP-02" },
                new PersonalSalud { IdPersonal = 3, Codigo = "MED-003", Apellidos = "Condori Ayala", Nombres = "Pedro Andrés", ColegioProfesional = "CMP", NumeroColegiado = "88210", TipoPersonal = "Médico", IdEstablecimiento = 1, EspecialidadPrincipal = "Traumatología", Sip = "SIP-03" },
                new PersonalSalud { IdPersonal = 4, Codigo = "MED-004", Apellidos = "Huamán Ccora", Nombres = "Rosa Esmeralda", ColegioProfesional = "CMP", NumeroColegiado = "88605", TipoPersonal = "Médico", IdEstablecimiento = 1, EspecialidadPrincipal = "Obstetricia y Ginecología", Sip = "SIP-04" },
                new PersonalSalud { IdPersonal = 5, Codigo = "MED-005", Apellidos = "Varillas Huamaní", Nombres = "Javier Alonso", ColegioProfesional = "CMP", NumeroColegiado = "88110", TipoPersonal = "Médico", IdEstablecimiento = 4, EspecialidadPrincipal = "Medicina Interna", Sip = "SIP-JAE-01" },
                new PersonalSalud { IdPersonal = 6, Codigo = "MED-006", Apellidos = "Tutors Quispe", Nombres = "Sandra Cecilia", ColegioProfesional = "CEP", NumeroColegiado = "88770", TipoPersonal = "Enfermera", IdEstablecimiento = 2, EspecialidadPrincipal = "Enfermería en Urgencias", Sip = "SIP-CH-01" },
                new PersonalSalud { IdPersonal = 7, Codigo = "MED-007", Apellidos = "Cruzado Vásquez", Nombres = "Jorge Antonio", ColegioProfesional = "CMP", NumeroColegiado = "88912", TipoPersonal = "Médico", IdEstablecimiento = 5, EspecialidadPrincipal = "Cirugía General", Sip = "SIP-BAM-01" },
                new PersonalSalud { IdPersonal = 8, Codigo = "MED-008", Apellidos = "Guevara Sánchez", Nombres = "Lucía Marisol", ColegioProfesional = "CMP", NumeroColegiado = "89044", TipoPersonal = "Médico", IdEstablecimiento = 6, EspecialidadPrincipal = "Medicina Familiar", Sip = "SIP-BDI-01" });

            // ===== Relaciones nuevas (Hospitalario) =====
            // Paciente (1) -> (N) SolicitudReferencia
            modelBuilder.Entity<SolicitudReferencia>()
                .HasOne(s => s.Paciente)
                .WithMany(p => p.SolicitudesReferencia)
                .HasForeignKey(s => s.IdPaciente)
                .OnDelete(DeleteBehavior.Cascade);

            // Establecimiento (1) -> (N) SolicitudReferencia como ORIGEN
            modelBuilder.Entity<SolicitudReferencia>()
                .HasOne(s => s.EstablecimientoOrigen)
                .WithMany(e => e.ReferenciasComoOrigen)
                .HasForeignKey(s => s.IdEstablecimientoOrigen)
                .OnDelete(DeleteBehavior.Restrict); // IMPIDE ciclo de cascada

            // Establecimiento (1) -> (N) SolicitudReferencia como DESTINO
            modelBuilder.Entity<SolicitudReferencia>()
                .HasOne(s => s.EstablecimientoDestino)
                .WithMany(e => e.ReferenciasComoDestino)
                .HasForeignKey(s => s.IdEstablecimientoDestino)
                .OnDelete(DeleteBehavior.Restrict); // IMPIDE ciclo de cascada

            // 1:N SolicitudReferencia (1) -> (N) DetalleReferencia (Maestro-Detalle)
            modelBuilder.Entity<DetalleReferencia>()
                .HasOne(d => d.SolicitudReferencia)
                .WithMany(s => s.Detalles)
                .HasForeignKey(d => d.IdReferencia)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:1 SolicitudReferencia <-> Contrareferencia
            // Relación uno a uno: la Contrareferencia es el lado dependiente y lleva la FK única (IdReferencia).
            modelBuilder.Entity<SolicitudReferencia>()
                .HasOne(s => s.Contrareferencia)
                .WithOne(c => c.SolicitudReferencia)
                .HasForeignKey<Contrareferencia>(c => c.IdReferencia)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N: PersonalSalud (1) -> (N) SolicitudReferencia (médico solicitante de la derivación)
            modelBuilder.Entity<SolicitudReferencia>()
                .HasOne(s => s.MedicoSolicitante)
                .WithMany()
                .HasForeignKey(s => s.IdMedicoSolicitante)
                .OnDelete(DeleteBehavior.Restrict);

            // N:M EstablecimientoSalud <-> Especialidad (tabla intermedia)
            modelBuilder.Entity<EstablecimientoEspecialidad>()
                .HasOne(ee => ee.Establecimiento)
                .WithMany(e => e.EstablecimientoEspecialidades)
                .HasForeignKey(ee => ee.IdEstablecimiento)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EstablecimientoEspecialidad>()
                .HasOne(ee => ee.Especialidad)
                .WithMany(e => e.EstablecimientoEspecialidades)
                .HasForeignKey(ee => ee.IdEspecialidad)
                .OnDelete(DeleteBehavior.Cascade);

            // N:M opcional: SolicitudReferencia <-> PersonalSalud (equipo médico asignado)
            modelBuilder.Entity<SolicitudReferenciaPersonal>()
                .HasOne(srp => srp.SolicitudReferencia)
                .WithMany(s => s.EquipoMedico)
                .HasForeignKey(srp => srp.IdReferencia)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SolicitudReferenciaPersonal>()
                .HasOne(srp => srp.PersonalSalud)
                .WithMany(p => p.SolicitudesAsignadas)
                .HasForeignKey(srp => srp.IdPersonalSalud)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices útiles para filtrado del dashboard y bandeja
            modelBuilder.Entity<SolicitudReferencia>().HasIndex(s => s.NumeroReferencia).IsUnique();
            modelBuilder.Entity<SolicitudReferencia>().HasIndex(s => s.Estado);
            modelBuilder.Entity<SolicitudReferencia>().HasIndex(s => s.Prioridad);
            modelBuilder.Entity<SolicitudReferencia>().HasIndex(s => s.IdEstablecimientoDestino);
            modelBuilder.Entity<SolicitudReferencia>().HasIndex(s => s.IdMedicoSolicitante);
            modelBuilder.Entity<DetalleReferencia>().HasIndex(d => d.CodigoCie10);
        }
    }
}
