using PROYECTO_REFERENCIAS.Data;
using PROYECTO_REFERENCIAS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace PROYECTO_REFERENCIAS.Controllers
{
    [Authorize]
    public class EspecialidadController : Controller
    {
        private readonly AppDbContext _db;

        public EspecialidadController(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string filtro = "")
        {
            var query = _db.Especialidades.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.Trim();
                query = query.Where(e =>
                    e.Codigo.Contains(filtro) ||
                    e.Nombre.Contains(filtro) ||
                    (e.Descripcion != null && e.Descripcion.Contains(filtro)));
            }

            var list = await query.OrderBy(e => e.Nombre).ToListAsync();
            ViewBag.Filtro = filtro;
            return View(list);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new Especialidad());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Especialidad modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Datos inválidos. Revise el formulario.";
                return View(modelo);
            }

            var duplicado = await _db.Especialidades.AnyAsync(e => e.Codigo == modelo.Codigo);
            if (duplicado)
            {
                ViewData["Mensaje"] = "Ya existe una especialidad con el mismo código.";
                return View(modelo);
            }

            _db.Especialidades.Add(modelo);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = $"Especialidad {modelo.Nombre} registrada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var modelo = await _db.Especialidades.FindAsync(id);
            if (modelo == null) return NotFound();
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Especialidad modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Datos inválidos. Revise el formulario.";
                return View(modelo);
            }

            _db.Especialidades.Update(modelo);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = $"Especialidad {modelo.Nombre} actualizada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var modelo = await _db.Especialidades
                .Include(e => e.EstablecimientoEspecialidades)
                .FirstOrDefaultAsync(e => e.IdEspecialidad == id);
            if (modelo == null) return NotFound();

            if (modelo.EstablecimientoEspecialidades.Any())
            {
                ViewData["Mensaje"] = "No se puede eliminar: la especialidad tiene establecimientos asignados.";
                return View("Index", await _db.Especialidades.ToListAsync());
            }

            _db.Especialidades.Remove(modelo);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = $"Especialidad {modelo.Nombre} eliminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}
