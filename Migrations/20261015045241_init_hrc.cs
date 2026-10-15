using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PROYECTO_REFERENCIAS.Migrations
{
    /// <inheritdoc />
    public partial class init_hrc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Especialidades",
                columns: table => new
                {
                    IdEspecialidad = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    NivelAtencion = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.IdEspecialidad);
                });

            migrationBuilder.CreateTable(
                name: "Establecimientos",
                columns: table => new
                {
                    IdEstablecimiento = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    TipoEstablecimiento = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RedSalud = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Ubicacion = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    RepresentanteLegal = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establecimientos", x => x.IdEstablecimiento);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    IdPaciente = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Dni = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Apellidos = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Nombres = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Sexo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AlergiaPrincipal = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CodigoRenaes = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.IdPaciente);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomRol = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "EstablecimientoEspecialidades",
                columns: table => new
                {
                    IdEstablecimientoEspecialidad = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdEstablecimiento = table.Column<int>(type: "INTEGER", nullable: false),
                    IdEspecialidad = table.Column<int>(type: "INTEGER", nullable: false),
                    CapacidadDisponible = table.Column<int>(type: "INTEGER", nullable: false),
                    HorarioAtencion = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    ObservacionCapacidad = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablecimientoEspecialidades", x => x.IdEstablecimientoEspecialidad);
                    table.ForeignKey(
                        name: "FK_EstablecimientoEspecialidades_Especialidades_IdEspecialidad",
                        column: x => x.IdEspecialidad,
                        principalTable: "Especialidades",
                        principalColumn: "IdEspecialidad",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstablecimientoEspecialidades_Establecimientos_IdEstablecimiento",
                        column: x => x.IdEstablecimiento,
                        principalTable: "Establecimientos",
                        principalColumn: "IdEstablecimiento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalSalud",
                columns: table => new
                {
                    IdPersonal = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Apellidos = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Nombres = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ColegioProfesional = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    NumeroColegiado = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    TipoPersonal = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IdEstablecimiento = table.Column<int>(type: "INTEGER", nullable: false),
                    EspecialidadPrincipal = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Sip = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalSalud", x => x.IdPersonal);
                    table.ForeignKey(
                        name: "FK_PersonalSalud_Establecimientos_IdEstablecimiento",
                        column: x => x.IdEstablecimiento,
                        principalTable: "Establecimientos",
                        principalColumn: "IdEstablecimiento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomUsuario = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Correo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Telefono = table.Column<int>(type: "INTEGER", nullable: false),
                    IdRol = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesReferencia",
                columns: table => new
                {
                    IdReferencia = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroReferencia = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaDerivacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Prioridad = table.Column<int>(type: "INTEGER", nullable: false),
                    PrioridadCalculada = table.Column<int>(type: "INTEGER", nullable: true),
                    Requerimiento = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    UbicacionUrgencia = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 600, nullable: false),
                    OrdenMedica = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ObservacionGeneral = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PresionArterial = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    FrecuenciaCardiaca = table.Column<int>(type: "INTEGER", nullable: true),
                    FrecuenciaRespiratoria = table.Column<int>(type: "INTEGER", nullable: true),
                    Temperatura = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    SaturacionOxigeno = table.Column<int>(type: "INTEGER", nullable: true),
                    PesoKg = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TallaCm = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IdMedicoSolicitante = table.Column<int>(type: "INTEGER", nullable: true),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaEstado = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ObservacionEstado = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    ObservacionSistema = table.Column<string>(type: "TEXT", nullable: true),
                    IdPaciente = table.Column<int>(type: "INTEGER", nullable: false),
                    IdEstablecimientoOrigen = table.Column<int>(type: "INTEGER", nullable: false),
                    IdEstablecimientoDestino = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaAceptacion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FechaContrareferencia = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesReferencia", x => x.IdReferencia);
                    table.ForeignKey(
                        name: "FK_SolicitudesReferencia_Establecimientos_IdEstablecimientoDestino",
                        column: x => x.IdEstablecimientoDestino,
                        principalTable: "Establecimientos",
                        principalColumn: "IdEstablecimiento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesReferencia_Establecimientos_IdEstablecimientoOrigen",
                        column: x => x.IdEstablecimientoOrigen,
                        principalTable: "Establecimientos",
                        principalColumn: "IdEstablecimiento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesReferencia_Pacientes_IdPaciente",
                        column: x => x.IdPaciente,
                        principalTable: "Pacientes",
                        principalColumn: "IdPaciente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudesReferencia_PersonalSalud_IdMedicoSolicitante",
                        column: x => x.IdMedicoSolicitante,
                        principalTable: "PersonalSalud",
                        principalColumn: "IdPersonal",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contrareferencias",
                columns: table => new
                {
                    IdContrareferencia = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaEmision = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Resultado = table.Column<string>(type: "TEXT", maxLength: 600, nullable: false),
                    IndicacionCentroOrigen = table.Column<string>(type: "TEXT", maxLength: 600, nullable: false),
                    EvolucionClinica = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    TipoEgreso = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IdReferencia = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contrareferencias", x => x.IdContrareferencia);
                    table.ForeignKey(
                        name: "FK_Contrareferencias_SolicitudesReferencia_IdReferencia",
                        column: x => x.IdReferencia,
                        principalTable: "SolicitudesReferencia",
                        principalColumn: "IdReferencia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesReferencia",
                columns: table => new
                {
                    IdDetalle = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CodigoCie10 = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Diagnostico = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    TipoDiagnostico = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    TipoApoyo = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    DescripcionApoyo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ExamenAuxiliar = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    HallazgosClinicos = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    SignosAlarma = table.Column<bool>(type: "INTEGER", nullable: false),
                    PrioridadDetalle = table.Column<int>(type: "INTEGER", nullable: false),
                    IdReferencia = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesReferencia", x => x.IdDetalle);
                    table.ForeignKey(
                        name: "FK_DetallesReferencia_SolicitudesReferencia_IdReferencia",
                        column: x => x.IdReferencia,
                        principalTable: "SolicitudesReferencia",
                        principalColumn: "IdReferencia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudReferenciaPersonal",
                columns: table => new
                {
                    IdSolicitudPersonal = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdReferencia = table.Column<int>(type: "INTEGER", nullable: false),
                    IdPersonalSalud = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudReferenciaPersonal", x => x.IdSolicitudPersonal);
                    table.ForeignKey(
                        name: "FK_SolicitudReferenciaPersonal_PersonalSalud_IdPersonalSalud",
                        column: x => x.IdPersonalSalud,
                        principalTable: "PersonalSalud",
                        principalColumn: "IdPersonal",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudReferenciaPersonal_SolicitudesReferencia_IdReferencia",
                        column: x => x.IdReferencia,
                        principalTable: "SolicitudesReferencia",
                        principalColumn: "IdReferencia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Especialidades",
                columns: new[] { "IdEspecialidad", "Codigo", "Descripcion", "NivelAtencion", "Nombre" },
                values: new object[,]
                {
                    { 1, "CIE-10-MEDINT", "Patologías médicas generales de adultos.", "III", "Medicina Interna" },
                    { 2, "CIE-10-PEDI", "Atención del niño y adolescente.", "III", "Pediatría" },
                    { 3, "CIE-10-TRAU", "Lesiones del sistema osteomuscular.", "III", "Traumatología" },
                    { 4, "CIE-10-OBST", "Embarazo, parto y salud femenina.", "III", "Obstetricia y Ginecología" },
                    { 5, "CIE-10-NEURO", "Patologías del sistema nervioso.", "III", "Neurología" },
                    { 6, "CIE-10-CARD", "Patologías cardíacas.", "III", "Cardiología" },
                    { 7, "CIE-10-TOXI", "Intoxicaciones y haceres.", "II", "Toxicología" },
                    { 8, "CIE-10-UROL", "Patologías del tracto urinario.", "III", "Urología" }
                });

            migrationBuilder.InsertData(
                table: "Establecimientos",
                columns: new[] { "IdEstablecimiento", "Codigo", "Nombre", "RedSalud", "RepresentanteLegal", "Telefono", "TipoEstablecimiento", "Ubicacion" },
                values: new object[,]
                {
                    { 1, "HRC-DOC", "Hospital Regional Docente de Cajamarca", "Regional", "Dr. Director Médico", "042-411111", "Hospital Regional", "Cajamarca - San Juan" },
                    { 2, "CS-CH01", "Centro de Salud Alto Chota", "Chota", "Médico a Cargo", "042-555001", "Centro de Salud", "Chota" },
                    { 3, "HJS-CAD", "Hospital José Soto Cadenillas de Chota", "Chota", "Dr. Jefe de Hospital", "042-688002", "Hospital Distrital", "Chota - Caserío El Chacán" },
                    { 4, "HG-JAE", "Hospital General de Jaén", "Jaén", "Dr. Jefe de Hospital", "042-777300", "Hospital Distrital", "Jaén" },
                    { 5, "HTVC-BAM", "Hospital Tito Villar Cabezas de Bambamarca", "Bambamarca", "Dr. Jefe de Hospital", "076-558001", "Hospital Distrital", "Bambamarca - Hualgayoc" },
                    { 6, "CS-BDI", "Centro de Salud Baños del Inca", "Cajamarca", "Médico Jefe", "076-346120", "Centro de Salud", "Baños del Inca" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "IdRol", "NomRol" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Empleado" },
                    { 3, "Usuario" }
                });

            migrationBuilder.InsertData(
                table: "EstablecimientoEspecialidades",
                columns: new[] { "IdEstablecimientoEspecialidad", "CapacidadDisponible", "HorarioAtencion", "IdEspecialidad", "IdEstablecimiento", "ObservacionCapacidad" },
                values: new object[,]
                {
                    { 1, 24, "Lunes a Viernes 07:00-17:00", 1, 1, null },
                    { 2, 20, "Lunes a Viernes 07:00-17:00", 2, 1, null },
                    { 3, 16, "Lunes a Sábado 07:00-17:00", 3, 1, null },
                    { 4, 18, "Lunes a Viernes 07:00-17:00", 4, 1, null },
                    { 5, 10, "Lunes a Viernes 07:00-17:00", 5, 1, null },
                    { 6, 10, "Lunes a Viernes 07:00-17:00", 6, 1, null },
                    { 7, 8, "Lunes a Viernes 07:00-17:00", 7, 1, null },
                    { 8, 12, "Lunes a Viernes 07:00-17:00", 8, 1, null },
                    { 9, 4, "Lunes a Viernes 07:00-14:00", 1, 4, null },
                    { 10, 4, "Lunes a Viernes 07:00-14:00", 3, 4, null },
                    { 11, 6, "Lunes a Viernes 07:00-14:00", 1, 5, null },
                    { 12, 4, "Lunes a Viernes 07:00-14:00", 3, 5, null },
                    { 13, 3, "Lunes a Viernes 07:00-13:00", 1, 6, null },
                    { 14, 3, "Lunes a Viernes 07:00-13:00", 2, 6, null }
                });

            migrationBuilder.InsertData(
                table: "PersonalSalud",
                columns: new[] { "IdPersonal", "Apellidos", "Codigo", "ColegioProfesional", "EspecialidadPrincipal", "IdEstablecimiento", "Nombres", "NumeroColegiado", "Sip", "TipoPersonal" },
                values: new object[,]
                {
                    { 1, "Quispe Mamani", "MED-001", "CMP", "Medicina Interna", 1, "Luis Alberto", "88301", "SIP-01", "Médico" },
                    { 2, "Choquehuanca Rojas", "MED-002", "CMP", "Pediatría", 1, "María Elena", "88452", "SIP-02", "Médico" },
                    { 3, "Condori Ayala", "MED-003", "CMP", "Traumatología", 1, "Pedro Andrés", "88210", "SIP-03", "Médico" },
                    { 4, "Huamán Ccora", "MED-004", "CMP", "Obstetricia y Ginecología", 1, "Rosa Esmeralda", "88605", "SIP-04", "Médico" },
                    { 5, "Varillas Huamaní", "MED-005", "CMP", "Medicina Interna", 4, "Javier Alonso", "88110", "SIP-JAE-01", "Médico" },
                    { 6, "Tutors Quispe", "MED-006", "CEP", "Enfermería en Urgencias", 2, "Sandra Cecilia", "88770", "SIP-CH-01", "Enfermera" },
                    { 7, "Cruzado Vásquez", "MED-007", "CMP", "Cirugía General", 5, "Jorge Antonio", "88912", "SIP-BAM-01", "Médico" },
                    { 8, "Guevara Sánchez", "MED-008", "CMP", "Medicina Familiar", 6, "Lucía Marisol", "89044", "SIP-BDI-01", "Médico" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contrareferencias_IdReferencia",
                table: "Contrareferencias",
                column: "IdReferencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesReferencia_CodigoCie10",
                table: "DetallesReferencia",
                column: "CodigoCie10");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesReferencia_IdReferencia",
                table: "DetallesReferencia",
                column: "IdReferencia");

            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientoEspecialidades_IdEspecialidad",
                table: "EstablecimientoEspecialidades",
                column: "IdEspecialidad");

            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientoEspecialidades_IdEstablecimiento",
                table: "EstablecimientoEspecialidades",
                column: "IdEstablecimiento");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalSalud_IdEstablecimiento",
                table: "PersonalSalud",
                column: "IdEstablecimiento");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesReferencia_Estado",
                table: "SolicitudesReferencia",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesReferencia_IdEstablecimientoDestino",
                table: "SolicitudesReferencia",
                column: "IdEstablecimientoDestino");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesReferencia_IdEstablecimientoOrigen",
                table: "SolicitudesReferencia",
                column: "IdEstablecimientoOrigen");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesReferencia_IdMedicoSolicitante",
                table: "SolicitudesReferencia",
                column: "IdMedicoSolicitante");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesReferencia_IdPaciente",
                table: "SolicitudesReferencia",
                column: "IdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesReferencia_NumeroReferencia",
                table: "SolicitudesReferencia",
                column: "NumeroReferencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesReferencia_Prioridad",
                table: "SolicitudesReferencia",
                column: "Prioridad");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudReferenciaPersonal_IdPersonalSalud",
                table: "SolicitudReferenciaPersonal",
                column: "IdPersonalSalud");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudReferenciaPersonal_IdReferencia",
                table: "SolicitudReferenciaPersonal",
                column: "IdReferencia");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contrareferencias");

            migrationBuilder.DropTable(
                name: "DetallesReferencia");

            migrationBuilder.DropTable(
                name: "EstablecimientoEspecialidades");

            migrationBuilder.DropTable(
                name: "SolicitudReferenciaPersonal");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Especialidades");

            migrationBuilder.DropTable(
                name: "SolicitudesReferencia");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "PersonalSalud");

            migrationBuilder.DropTable(
                name: "Establecimientos");
        }
    }
}
