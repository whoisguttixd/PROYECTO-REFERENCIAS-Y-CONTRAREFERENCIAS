using PROYECTO_REFERENCIAS.Data;
using PROYECTO_REFERENCIAS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PROYECTO_REFERENCIAS.Controllers
{
    [Authorize]
    public class PersonalSaludController : Controller
    {
        private readonly AppDbContext _db;

        public PersonalSaludController(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string filtro = "", int? idEstablecimiento = null)
        {
            var query = _db.PersonalSalud
                .Include(p => p.Establecimiento)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.Trim();
                query = query.Where(p =>
                    p.Codigo.Contains(filtro) ||
                    p.Apellidos.Contains(filtro) ||
                    p.Nombres.Contains(filtro) ||
                    p.NumeroColegiado.Contains(filtro) ||
                    (p.ColegioProfesional != null && p.ColegioProfesional.Contains(filtro)));
            }

            if (idEstablecimiento.HasValue)
            {
                query = query.Where(p => p.IdEstablecimiento == idEstablecimiento.Value);
            }

            var list = await query
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres)
                .ToListAsync();

            ViewBag.Filtro = filtro;
            ViewBag.IdEstablecimiento = idEstablecimiento;
            ViewBag.HayFiltros = !string.IsNullOrWhiteSpace(filtro) || idEstablecimiento.HasValue;
            ViewBag.Establecimientos = await _db.Establecimientos.OrderBy(e => e.Nombre).ToListAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            ViewBag.Establecimientos = await _db.Establecimientos.OrderBy(e => e.Nombre).ToListAsync();
            return View(new PersonalSalud());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(PersonalSalud modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Datos inválidos. Revise el formulario.";
                ViewBag.Establecimientos = await _db.Establecimientos.OrderBy(e => e.Nombre).ToListAsync();
                return View(modelo);
            }

            var duplicado = await _db.PersonalSalud.AnyAsync(p => p.NumeroColegiado == modelo.NumeroColegiado);
            if (duplicado)
            {
                ViewData["Mensaje"] = "Ya existe un profesional registrado con este número de colegiado.";
                ViewBag.Establecimientos = await _db.Establecimientos.OrderBy(e => e.Nombre).ToListAsync();
                return View(modelo);
            }

            _db.PersonalSalud.Add(modelo);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = $"Personal {modelo.Nombres} {modelo.Apellidos} registrado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var modelo = await _db.PersonalSalud.FindAsync(id);
            if (modelo == null) return NotFound();
            ViewBag.Establecimientos = await _db.Establecimientos.OrderBy(e => e.Nombre).ToListAsync();
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(PersonalSalud modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Datos inválidos. Revise el formulario.";
                ViewBag.Establecimientos = await _db.Establecimientos.OrderBy(e => e.Nombre).ToListAsync();
                return View(modelo);
            }

            _db.PersonalSalud.Update(modelo);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = $"Personal {modelo.Nombres} {modelo.Apellidos} actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var modelo = await _db.PersonalSalud
                .Include(p => p.SolicitudesAsignadas)
                .FirstOrDefaultAsync(p => p.IdPersonal == id);
            if (modelo == null) return NotFound();

            if (modelo.SolicitudesAsignadas.Any())
            {
                ViewData["Mensaje"] = "No se puede eliminar: el profesional tiene solicitudes asignadas.";
                return View("Index", await _db.PersonalSalud.ToListAsync());
            }

            _db.PersonalSalud.Remove(modelo);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = $"Personal {modelo.Nombres} {modelo.Apellidos} eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
