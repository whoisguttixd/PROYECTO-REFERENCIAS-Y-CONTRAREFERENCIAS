using PROYECTO_REFERENCIAS.Data;
using PROYECTO_REFERENCIAS.Models;
using PROYECTO_REFERENCIAS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYECTO_REFERENCIAS.Controllers
{
    [Authorize]
    public class EstablecimientoSaludController : Controller
    {
        private readonly AppDbContext _db;

        public EstablecimientoSaludController(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string filtro = "")
        {
            var query = _db.Establecimientos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.Trim();
                query = query.Where(e =>
                    e.Codigo.Contains(filtro) ||
                    e.Nombre.Contains(filtro) ||
                    e.TipoEstablecimiento.Contains(filtro) ||
                    e.RedSalud.Contains(filtro) ||
                    e.Ubicacion.Contains(filtro));
            }

            var list = await query
                .Include(e => e.EstablecimientoEspecialidades)
                .ThenInclude(ee => ee.Especialidad)
                .OrderBy(e => e.RedSalud)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            ViewBag.Filtro = filtro;
            return View(list);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Mensaje = null;
            return View(new EstablecimientoSalud());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(EstablecimientoSalud modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Datos inválidos. Revise el formulario.";
                return View(modelo);
            }

            var duplicado = await _db.Establecimientos.AnyAsync(e => e.Codigo == modelo.Codigo);
            if (duplicado)
            {
                ViewData["Mensaje"] = "Ya existe un establecimiento registrado con este código interno.";
                return View(modelo);
            }

            _db.Establecimientos.Add(modelo);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = $"Establecimiento {modelo.Nombre} registrado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var modelo = await _db.Establecimientos
                .Include(e => e.EstablecimientoEspecialidades)
                .FirstOrDefaultAsync(e => e.IdEstablecimiento == id);
            if (modelo == null) return NotFound();
            ViewBag.Mensaje = null;
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(EstablecimientoSalud modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Datos inválidos. Revise el formulario.";
                return View(modelo);
            }

            _db.Establecimientos.Update(modelo);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = $"Establecimiento {modelo.Nombre} actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            var modelo = await _db.Establecimientos
                .Include(e => e.EstablecimientoEspecialidades)
                .ThenInclude(ee => ee.Especialidad)
                .FirstOrDefaultAsync(e => e.IdEstablecimiento == id);
            if (modelo == null) return NotFound();
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> AsignarEspecialidades(int id)
        {
            var modelo = await _db.Establecimientos
                .Include(e => e.EstablecimientoEspecialidades)
                .ThenInclude(ee => ee.Especialidad)
                .FirstOrDefaultAsync(e => e.IdEstablecimiento == id);
            if (modelo == null) return NotFound();

            ViewBag.TodasEspecialidades = await _db.Especialidades.OrderBy(e => e.Nombre).ToListAsync();
            ViewBag.Mensaje = null;
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> AsignarEspecialidades(int id, List<int> especialidadesSeleccionadas, string? capacidad, string horario)
        {
            var modelo = await _db.Establecimientos
                .Include(e => e.EstablecimientoEspecialidades)
                .FirstOrDefaultAsync(e => e.IdEstablecimiento == id);
            if (modelo == null) return NotFound();

            if (especialidadesSeleccionadas == null || !especialidadesSeleccionadas.Any())
            {
                ViewBag.Mensaje = "Seleccione al menos una especialidad.";
                ViewBag.TodasEspecialidades = await _db.Especialidades.OrderBy(e => e.Nombre).ToListAsync();
                return View(modelo);
            }

            var especialidadesExistentes = modelo.EstablecimientoEspecialidades.Select(ee => ee.IdEspecialidad).ToHashSet();
            foreach (var idEspecialidad in especialidadesSeleccionadas)
            {
                if (!especialidadesExistentes.Contains(idEspecialidad))
                {
                    var nueva = new EstablecimientoEspecialidad
                    {
                        IdEstablecimiento = id,
                        IdEspecialidad = idEspecialidad,
                        CapacidadDisponible = int.TryParse(capacidad, out var cap) ? cap : 0,
                        HorarioAtencion = string.IsNullOrWhiteSpace(horario) ? "Por definir" : horario
                    };
                    _db.EstablecimientoEspecialidades.Add(nueva);
                }
            }

            await _db.SaveChangesAsync();
            TempData["Mensaje"] = $"Especialidades asignadas al establecimiento {modelo.Nombre}.";
            return RedirectToAction(nameof(Detalles), new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> QuitarEspecialidad(int idEstablecimiento, int idEspecialidad)
        {
            var ee = await _db.EstablecimientoEspecialidades
                .FirstOrDefaultAsync(x => x.IdEstablecimiento == idEstablecimiento && x.IdEspecialidad == idEspecialidad);
            if (ee != null)
            {
                _db.EstablecimientoEspecialidades.Remove(ee);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(AsignarEspecialidades), new { id = idEstablecimiento });
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var modelo = await _db.Establecimientos
                .Include(e => e.ReferenciasComoOrigen)
                .Include(e => e.ReferenciasComoDestino)
                .FirstOrDefaultAsync(e => e.IdEstablecimiento == id);
            if (modelo == null) return NotFound();

            if (modelo.ReferenciasComoOrigen.Any() || modelo.ReferenciasComoDestino.Any())
            {
                ViewData["Mensaje"] = "No se puede eliminar: el establecimiento tiene solicitudes de referencia asociadas.";
                return View("Index", await _db.Establecimientos.ToListAsync());
            }

            _db.Establecimientos.Remove(modelo);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = $"Establecimiento {modelo.Nombre} eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
