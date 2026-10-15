using PROYECTO_REFERENCIAS.Data;
using PROYECTO_REFERENCIAS.Models;
using PROYECTO_REFERENCIAS.Repository;
using PROYECTO_REFERENCIAS.Service;
using PROYECTO_REFERENCIAS.ViewModels;
using PROYECTO_REFERENCIAS.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYECTO_REFERENCIAS.Controllers
{
    [Authorize]
    public class SolicitudReferenciaController : Controller
    {
        private readonly UnitOfWork _uow;
        private readonly ConfiguracionHospitalariaSingleton _config;
        private readonly SolicitudReferenciaService _servicio;

        public SolicitudReferenciaController(UnitOfWork uow, ConfiguracionHospitalariaSingleton config)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _servicio = new SolicitudReferenciaService(config);
        }

        // ---- BANDECHA / LISTADO ----
        [HttpGet]
        public async Task<IActionResult> Index(string estado = "", string prioridad = "", int? idDestino = null, string busqueda = "")
        {
            var query = _uow.ReferenciaRepository.GetAll()
                .Include(s => s.Paciente)
                .Include(s => s.EstablecimientoOrigen)
                .Include(s => s.EstablecimientoDestino)
                .Include(s => s.Detalles)
                .Include(s => s.Contrareferencia)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado) && Enum.TryParse<EstadoReferencia>(estado, out var eEstado))
                query = query.Where(s => s.Estado == eEstado);

            if (!string.IsNullOrWhiteSpace(prioridad) && Enum.TryParse<PrioridadReferencia>(prioridad, out var ePrioridad))
                query = query.Where(s => s.Prioridad == ePrioridad);

            if (idDestino.HasValue)
                query = query.Where(s => s.IdEstablecimientoDestino == idDestino);

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.Trim();
                query = query.Where(s =>
                    s.NumeroReferencia.Contains(busqueda) ||
                    (s.Paciente != null && (s.Paciente.Apellidos.Contains(busqueda) || s.Paciente.Nombres.Contains(busqueda))) ||
                    (s.EstablecimientoOrigen != null && s.EstablecimientoOrigen.Nombre.Contains(busqueda)) ||
                    (s.EstablecimientoDestino != null && s.EstablecimientoDestino.Nombre.Contains(busqueda)));
            }

            var lista = await query.OrderByDescending(s => s.FechaRegistro).ToListAsync();
            ViewBag.Estados = Enum.GetValues<EstadoReferencia>();
            ViewBag.Prioridades = Enum.GetValues<PrioridadReferencia>();
            ViewBag.Destinos = await _uow.EstablecimientoRepository.GetAll().OrderBy(e => e.Nombre).ToListAsync();
            ViewBag.EstadoFiltro = estado;
            ViewBag.PrioridadFiltro = prioridad;
            ViewBag.IdDestinoFiltro = idDestino;
            ViewBag.Busqueda = busqueda;
            ViewBag.HayFiltros = !string.IsNullOrWhiteSpace(estado)
                || !string.IsNullOrWhiteSpace(prioridad)
                || idDestino.HasValue
                || !string.IsNullOrWhiteSpace(busqueda);
            return View(lista);
        }

        // ---- DASHBOARD EJECUTIVO ----
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var indicadores = new IndicadoresViewModel();

            var hoy = DateTime.Today;
            var semana = Enumerable.Range(0, 7).Select(d => hoy.AddDays(-d)).ToHashSet();

            var todas = await _uow.ReferenciaRepository.GetAll()
                .Include(s => s.Paciente)
                .Include(s => s.EstablecimientoDestino)
                .Include(s => s.Detalles)
                .ToListAsync();

            var filtrandoHoy = todas.Where(s => s.FechaRegistro.Date == hoy).ToList();
            var filtrandoSemana = todas.Where(s => semana.Contains(s.FechaRegistro.Date)).ToList();

            indicadores.TotalHoy = filtrandoHoy.Count;
            indicadores.TotalSemana = filtrandoSemana.Count;

            indicadores.EnEvaluacion = filtrandoSemana.Count(s => s.Estado == EstadoReferencia.EnEvaluacion);
            indicadores.Aceptadas = filtrandoSemana.Count(s => s.Estado == EstadoReferencia.Aceptada);
            indicadores.Observadas = filtrandoSemana.Count(s => s.Estado == EstadoReferencia.Observada);
            indicadores.Rechazadas = filtrandoSemana.Count(s => s.Estado == EstadoReferencia.Rechazada);
            indicadores.Contrareferidas = filtrandoSemana.Count(s => s.Estado == EstadoReferencia.Contrareferida);

            indicadores.EmergenciaCritica = filtrandoSemana.Count(s => s.Prioridad == PrioridadReferencia.EmergenciaCritica);
            indicadores.UrgenciaMayor = filtrandoSemana.Count(s => s.Prioridad == PrioridadReferencia.UrgenciaMayor);
            indicadores.ConsultaExterna = filtrandoSemana.Count(s => s.Prioridad == PrioridadReferencia.ConsultaExterna);

            var redGroups = filtrandoSemana
                .GroupBy(s => s.EstablecimientoDestino?.RedSalud ?? "(Sin destino)")
                .OrderByDescending(g => g.Count())
                .Select(g =>
                {
                    var red = g.Key;
                    var items = g.ToList();
                    return new FilaIndicadorRed
                    {
                        Red = red,
                        Total = items.Count,
                        EmergenciaCritica = items.Count(s => s.Prioridad == PrioridadReferencia.EmergenciaCritica),
                        UrgenciaMayor = items.Count(s => s.Prioridad == PrioridadReferencia.UrgenciaMayor),
                        ConsultaExterna = items.Count(s => s.Prioridad == PrioridadReferencia.ConsultaExterna)
                    };
                })
                .ToList();

            indicadores.ReferenciaPorRed = redGroups;
            indicadores.RenderEstado = RenderIndicadores(indicadores, "estado");
            indicadores.RenderPrioridad = RenderIndicadores(indicadores, "prioridad");
            indicadores.RenderRed = RenderIndicadores(indicadores, "red");
            return View(indicadores);
        }

        // ---- REGISTRO MAESTRO-DETALLE ----
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var vm = await PrepararViewModel(null);
            return View("Editar", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(SolicitudReferenciaVM vm)
        {
            var errores = await ValidarViewModel(vm);
            if (errores.Count > 0)
            {
                ViewData["Mensaje"] = string.Join(" ", errores);
                var modelo = await PrepararViewModel(vm);
                return View("Editar", modelo);
            }

            var solicitud = Mapear(vm);
            var resultado = _servicio.CalcularPrioridad(solicitud);
            solicitud.Prioridad = resultado.Prioridad;
            solicitud.PrioridadCalculada = resultado.Prioridad;
            solicitud.ObservacionGeneral = (solicitud.ObservacionGeneral ?? "")
                + $" [SLA: {resultado.MaxHorasRespuesta}h - Puntaje: {resultado.PuntajeGravedad} - {resultado.Justificacion} Protocolo: {resultado.ProtocoloRecepcion}]";

            await _uow.ReferenciaRepository.AddAsync(solicitud);
            // Agregar detalles al contexto antes de guardar (transacción atómica)
            foreach (var d in solicitud.Detalles)
            {
                await _uow.DetalleRepository.AddAsync(d);
            }

            var guardado = await _uow.SaveChangesAsync();

            if (solicitud.IdReferencia == 0 || guardado < 1)
            {
                ViewData["Mensaje"] = "No se pudo registrar la solicitud de referencia.";
                var modelo = await PrepararViewModel(vm);
                return View("Editar", modelo);
            }

            TempData["Mensaje"] = $"Referencia {solicitud.NumeroReferencia} registrada con prioridad: {solicitud.Prioridad} (SLA {resultado.MaxHorasRespuesta}h).";
            return RedirectToAction(nameof(Index));
        }

        // ---- VISTA PARA EVALUACIÓN (MÉDICO) ----
        [HttpGet]
        public async Task<IActionResult> Evaluar(int id)
        {
            var solicitud = await _uow.ReferenciaRepository.GetAll()
                .Include(s => s.Paciente)
                .Include(s => s.EstablecimientoOrigen)
                .Include(s => s.EstablecimientoDestino)
                .Include(s => s.Detalles)
                .Include(s => s.Contrareferencia)
                .Include(s => s.MedicoSolicitante)
                .FirstOrDefaultAsync(s => s.IdReferencia == id);

            if (solicitud == null) return NotFound();

            var vm = new EvaluarReferenciaVM
            {
                IdReferencia = solicitud.IdReferencia,
                NuevoEstado = solicitud.Estado
            };

            ViewBag.Solicitud = solicitud;
            ViewBag.ProximosEstados = ObtenerEstadosSiguientes(solicitud.Estado);
            ViewBag.EstadoActual = solicitud.Estado;
            ViewBag.PuedeContrareferir = solicitud.PuedeContrareferirse;
            ViewBag.Config = _config;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Evaluar(EvaluarReferenciaVM vm)
        {
            var solicitud = await _uow.ReferenciaRepository.GetAll()
                .Include(s => s.Contrareferencia)
                .FirstOrDefaultAsync(s => s.IdReferencia == vm.IdReferencia);

            if (solicitud == null) return NotFound();

            if (vm.NuevoEstado == EstadoReferencia.Contrareferida && solicitud.Contrareferencia == null)
            {
                ViewData["Mensaje"] = "Para contrareferir, primero debe emitir la contra-referencia.";
                return View("Evaluar", vm);
            }

            var ok = SolicitudReferenciaService.ProcesarTransicionEstado(solicitud, vm.NuevoEstado, vm.ObservacionEstado, out var mensaje);

            if (!ok)
            {
                ViewData["Mensaje"] = mensaje;
                ViewBag.Solicitud = solicitud;
                ViewBag.EstadoActual = solicitud.Estado;
                ViewBag.ProximosEstados = ObtenerEstadosSiguientes(solicitud.Estado);
                ViewBag.PuedeContrareferir = solicitud.PuedeContrareferirse;
                ViewBag.Config = _config;
                return View("Evaluar", vm);
            }

            await _uow.SaveChangesAsync();
            TempData["Mensaje"] = mensaje;

            if (solicitud.Estado == EstadoReferencia.Contrareferida)
                return RedirectToAction(nameof(ContrareferenciaVer), new { id = solicitud.IdReferencia });

            return RedirectToAction(nameof(Index));
        }

        // ---- EMISIÓN / VISTA DE CONTRA-REFERENCIA (RELACIÓN 1:1) ----
        [HttpGet]
        public async Task<IActionResult> ContrareferenciaEmitir(int id)
        {
            var solicitud = await _uow.ReferenciaRepository.GetAll()
                .Include(s => s.Paciente)
                .Include(s => s.EstablecimientoOrigen)
                .Include(s => s.EstablecimientoDestino)
                .Include(s => s.Detalles)
                .Include(s => s.Contrareferencia)
                .Include(s => s.MedicoSolicitante)
                .FirstOrDefaultAsync(s => s.IdReferencia == id);

            if (solicitud == null) return NotFound();

            if (!solicitud.PuedeContrareferirse)
            {
                ViewData["Mensaje"] = "Esta referencia no puede ser contrareferida en su estado actual.";
                return RedirectToAction(nameof(Evaluar), new { id = id });
            }

            var vm = new ContrareferenciaVM
            {
                IdReferencia = solicitud.IdReferencia,
                IdContrareferencia = solicitud.Contrareferencia?.IdContrareferencia ?? 0,
                NumeroReferencia = solicitud.NumeroReferencia,
                FechaEmision = DateTime.Today,
                Resultado = "",
                IndicacionCentroOrigen = "",
                EvolucionClinica = "",
                TipoEgreso = "Alta médica"
            };

            ViewBag.Solicitud = solicitud;
            ViewBag.YaTieneContrareferencia = solicitud.Contrareferencia != null;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContrareferenciaEmitir(ContrareferenciaVM vm)
        {
            var solicitud = await _uow.ReferenciaRepository.GetAll()
                .Include(s => s.Contrareferencia)
                .FirstOrDefaultAsync(s => s.IdReferencia == vm.IdReferencia);

            if (solicitud == null) return NotFound();

            if (!solicitud.PuedeContrareferirse)
            {
                ViewData["Mensaje"] = "Esta referencia no puede ser contrareferida.";
                return RedirectToAction(nameof(Evaluar), new { id = vm.IdReferencia });
            }

            Contrareferencia contra;

            if (solicitud.Contrareferencia != null)
            {
                contra = solicitud.Contrareferencia;
                contra.FechaEmision = vm.FechaEmision;
                contra.Resultado = vm.Resultado;
                contra.IndicacionCentroOrigen = vm.IndicacionCentroOrigen;
                contra.EvolucionClinica = vm.EvolucionClinica;
                contra.TipoEgreso = vm.TipoEgreso;
            }
            else
            {
                contra = new Contrareferencia
                {
                    IdReferencia = solicitud.IdReferencia,
                    FechaEmision = vm.FechaEmision,
                    Resultado = vm.Resultado,
                    IndicacionCentroOrigen = vm.IndicacionCentroOrigen,
                    EvolucionClinica = vm.EvolucionClinica,
                    TipoEgreso = vm.TipoEgreso
                };
                await _uow.ContrareferenciaRepository.AddAsync(contra);
            }

            // Relación 1:1: la Contrareferencia queda vinculada por su FK (IdReferencia).
            solicitud.Contrareferencia = contra;

            await _uow.SaveChangesAsync();
            TempData["Mensaje"] = "Contra-referencia emitida correctamente.";

            return RedirectToAction(nameof(Evaluar), new { id = vm.IdReferencia });
        }

        [HttpGet]
        public async Task<IActionResult> ContrareferenciaVer(int id)
        {
            var solicitud = await _uow.ReferenciaRepository.GetAll()
                .Include(s => s.Paciente)
                .Include(s => s.EstablecimientoOrigen)
                .Include(s => s.EstablecimientoDestino)
                .Include(s => s.Detalles)
                .Include(s => s.Contrareferencia)
                .Include(s => s.MedicoSolicitante)
                .FirstOrDefaultAsync(s => s.IdReferencia == id);

            if (solicitud == null) return NotFound();
            ViewBag.Solicitud = solicitud;
            return View(solicitud);
        }

        // ---- AYUDA ----
        private async Task<SolicitudReferenciaVM> PrepararViewModel(SolicitudReferenciaVM? vm)
        {
            var modelo = vm ?? new SolicitudReferenciaVM();

            modelo.Pacientes = await _uow.PacienteRepository.GetAll().OrderBy(p => p.Apellidos).ToListAsync();
            modelo.EstablecimientosOrigen = await _uow.EstablecimientoRepository.GetAll().OrderBy(e => e.Nombre).ToListAsync();
            modelo.EstablecimientosDestino = await _uow.EstablecimientoRepository.GetAll().OrderBy(e => e.Nombre).ToListAsync();
            modelo.EspecialidadesCie10 = await _uow.EspecialidadRepository.GetAll().OrderBy(e => e.Nombre).ToListAsync();
            modelo.MedicosSolicitantes = await _uow.PersonalRepository.GetAll().OrderBy(p => p.Apellidos).ToListAsync();

            if (modelo.Pacientes.Any())
            {
                var primerPaciente = modelo.Pacientes.First();
                modelo.IdPaciente = primerPaciente.IdPaciente;
                modelo.PacienteDisplay = $"{primerPaciente.Apellidos}, {primerPaciente.Nombres}";
            }

            if (modelo.EstablecimientosOrigen.Any())
            {
                var primerOrigen = modelo.EstablecimientosOrigen.First();
                modelo.IdEstablecimientoOrigen = primerOrigen.IdEstablecimiento;
            }

            if (modelo.EstablecimientosDestino.Any())
            {
                var primerDestino = modelo.EstablecimientosDestino.OrderBy(e => e.IdEstablecimiento).Skip(1).FirstOrDefault();
                if (primerDestino != null)
                    modelo.IdEstablecimientoDestino = primerDestino.IdEstablecimiento;
                else
                    modelo.IdEstablecimientoDestino = modelo.EstablecimientosDestino.Skip(1).FirstOrDefault()?.IdEstablecimiento ?? 1;
            }

            modelo.FechaRegistro = DateTime.Today;
            modelo.FechaDerivacion = DateTime.Today;
            modelo.Prioridad = PrioridadReferencia.SinCalcular;

            if (!modelo.Detalles.Any())
            {
                modelo.Detalles.Add(new DetalleReferenciaVM());
            }

            return modelo;
        }

        private async Task<List<string>> ValidarViewModel(SolicitudReferenciaVM vm)
        {
            var errores = new List<string>();

            if (!ModelState.IsValid)
            {
                errores.Add("Error de validación del formulario. Revise los campos marcados.");
            }

            if (vm.IdEstablecimientoOrigen == vm.IdEstablecimientoDestino)
            {
                errores.Add("El Establecimiento de Origen no puede ser igual al de Destino.");
            }

            if (vm.FechaDerivacion < vm.FechaRegistro)
            {
                errores.Add("La fecha de derivación no puede ser anterior a la fecha de registro.");
            }

            if (!vm.NumeroReferencia.StartsWith("HRC-"))
            {
                errores.Add("El número de referencia debe comenzar con HRC-.");
            }

            if (vm.Detalles == null || !vm.Detalles.Any())
            {
                errores.Add("La solicitud debe tener al menos un detalle.");
            }
            else
            {
                // Validar códigos CIE-10
                var detallesInvalidos = vm.Detalles.Where(d =>
                    string.IsNullOrWhiteSpace(d.CodigoCie10) ||
                    !System.Text.RegularExpressions.Regex.IsMatch(d.CodigoCie10, @"^[A-Z][0-9]{2}(\.[0-9]{1,2})?[A-Z]?$")
                ).ToList();
                if (detallesInvalidos.Any())
                {
                    errores.Add($"Los siguientes detalles tienen código CIE-10 inválido: {string.Join(", ", detallesInvalidos.Select(d => d.CodigoCie10))}");
                }
            }

            // Buscar paciente
            var paciente = await _uow.PacienteRepository.GetByIdAsync(vm.IdPaciente);
            if (paciente == null)
            {
                errores.Add("Seleccione un paciente válido.");
            }
            else
            {
                vm.PacienteDisplay = $"{paciente.Apellidos}, {paciente.Nombres}";
            }

            return errores;
        }

        private SolicitudReferencia Mapear(SolicitudReferenciaVM vm)
        {
            var detalles = new List<DetalleReferencia>();
            foreach (var d in vm.Detalles)
            {
                if (d == null) continue;
                var det = new DetalleReferencia
                {
                    CodigoCie10 = d.CodigoCie10,
                    Diagnostico = d.Diagnostico,
                    TipoApoyo = d.TipoApoyo,
                    DescripcionApoyo = d.DescripcionApoyo,
                    Observaciones = d.Observaciones,
                    TipoDiagnostico = d.TipoDiagnostico,
                    ExamenAuxiliar = d.ExamenAuxiliar,
                    HallazgosClinicos = d.HallazgosClinicos,
                    SignosAlarma = d.SignosAlarma,
                    PrioridadDetalle = d.PrioridadDetalle
                };
                detalles.Add(det);
            }

            return new SolicitudReferencia
            {
                NumeroReferencia = vm.NumeroReferencia,
                FechaRegistro = vm.FechaRegistro,
                FechaDerivacion = vm.FechaDerivacion,
                Prioridad = vm.Prioridad,
                PrioridadCalculada = vm.PrioridadCalculada,
                Requerimiento = vm.Requerimiento,
                UbicacionUrgencia = vm.UbicacionUrgencia,
                Motivo = vm.Motivo,
                OrdenMedica = vm.OrdenMedica,
                ObservacionGeneral = vm.ObservacionGeneral,
                PresionArterial = vm.PresionArterial,
                FrecuenciaCardiaca = vm.FrecuenciaCardiaca,
                FrecuenciaRespiratoria = vm.FrecuenciaRespiratoria,
                Temperatura = vm.Temperatura,
                SaturacionOxigeno = vm.SaturacionOxigeno,
                PesoKg = vm.PesoKg,
                TallaCm = vm.TallaCm,
                IdMedicoSolicitante = vm.IdMedicoSolicitante,
                Estado = vm.Estado,
                IdPaciente = vm.IdPaciente,
                IdEstablecimientoOrigen = vm.IdEstablecimientoOrigen,
                IdEstablecimientoDestino = vm.IdEstablecimientoDestino,
                Detalles = detalles
            };
        }

        private static List<EstadoReferencia> ObtenerEstadosSiguientes(EstadoReferencia estado)
        {
            var map = new Dictionary<EstadoReferencia, List<EstadoReferencia>>
            {
                { EstadoReferencia.Registrada, new List<EstadoReferencia> { EstadoReferencia.EnEvaluacion } },
                { EstadoReferencia.EnEvaluacion, new List<EstadoReferencia> { EstadoReferencia.Aceptada, EstadoReferencia.Observada, EstadoReferencia.Rechazada } },
                { EstadoReferencia.Observada, new List<EstadoReferencia> { EstadoReferencia.Aceptada, EstadoReferencia.Contrareferida } },
                { EstadoReferencia.Aceptada, new List<EstadoReferencia> { EstadoReferencia.Contrareferida } },
                { EstadoReferencia.Rechazada, new List<EstadoReferencia>() },
                { EstadoReferencia.Contrareferida, new List<EstadoReferencia>() }
            };
            return map.ContainsKey(estado) ? map[estado] : new List<EstadoReferencia>();
        }

        private string RenderIndicadores(IndicadoresViewModel m, string modo)
        {
            var registros = new List<string>();
            if (modo == "estado")
            {
                registros.Add($"<div class='row g-2'>");
                registros.Add($"<div class='col-md-3 col-sm-6'><div class='card h-100'><div class='card-body'><div class='h2 mb-0 fw-bold codigo-clinico'>{m.EnEvaluacion}</div><div class='small text-muted'>En Evaluación</div></div></div></div>");
                registros.Add($"<div class='col-md-3 col-sm-6'><div class='card h-100'><div class='card-body'><div class='h2 mb-0 fw-bold codigo-clinico'>{m.Aceptadas}</div><div class='small text-muted'>Aceptadas</div></div></div></div>");
                registros.Add($"<div class='col-md-3 col-sm-6'><div class='card h-100'><div class='card-body'><div class='h2 mb-0 fw-bold codigo-clinico'>{m.Observadas}</div><div class='small text-muted'>Observadas</div></div></div></div>");
                registros.Add($"<div class='col-md-3 col-sm-6'><div class='card h-100'><div class='card-body'><div class='h2 mb-0 fw-bold codigo-clinico'>{m.Rechazadas}</div><div class='small text-muted'>Rechazadas</div></div></div></div>");
                registros.Add($"<div class='col-md-3 col-sm-6'><div class='card h-100'><div class='card-body'><div class='h2 mb-0 fw-bold codigo-clinico'>{m.Contrareferidas}</div><div class='small text-muted'>Contrareferidas</div></div></div></div>");
                registros.Add($"</div>");
            }
            else if (modo == "prioridad")
            {
                registros.Add($"<div class='row g-2'>");
                registros.Add($"<div class='col-md-4 col-sm-12'><div class='card h-100'><div class='card-body'><div class='h2 mb-0 fw-bold codigo-clinico'>{m.EmergenciaCritica}</div><div class='small text-muted'>Emergencia / Crítica</div></div></div></div>");
                registros.Add($"<div class='col-md-4 col-sm-12'><div class='card h-100'><div class='card-body'><div class='h2 mb-0 fw-bold codigo-clinico'>{m.UrgenciaMayor}</div><div class='small text-muted'>Urgencia Mayor</div></div></div></div>");
                registros.Add($"<div class='col-md-4 col-sm-12'><div class='card h-100'><div class='card-body'><div class='h2 mb-0 fw-bold codigo-clinico'>{m.ConsultaExterna}</div><div class='small text-muted'>Consulta Externa</div></div></div></div>");
                registros.Add($"</div>");
            }
            else if (modo == "red")
            {
                registros.Add($"<table class='table table-sm table-hover align-middle'>");
                registros.Add($"<thead><tr><th>Red de Salud</th><th>Total</th><th>Emergencia</th><th>Urgencia</th><th>Consulta</th></tr></thead>");
                registros.Add($"<tbody>");
                foreach (var r in m.ReferenciaPorRed)
                {
                    registros.Add($"<tr><td><span class='fw-medium'>{r.Red}</span></td><td>{r.Total}</td><td>{r.EmergenciaCritica}</td><td>{r.UrgenciaMayor}</td><td>{r.ConsultaExterna}</td></tr>");
                }
                registros.Add($"</tbody></table>");
            }

            return string.Join(Environment.NewLine, registros);
        }
    }
}
