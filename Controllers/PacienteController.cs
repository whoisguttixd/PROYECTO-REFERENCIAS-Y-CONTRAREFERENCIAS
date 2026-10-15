using PROYECTO_REFERENCIAS.Data;
using PROYECTO_REFERENCIAS.Models;
using PROYECTO_REFERENCIAS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;

namespace PROYECTO_REFERENCIAS.Controllers
{
    [Authorize]
    public class PacienteController : Controller
    {
        private readonly AppDbContext _db;

        public PacienteController(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // GET: Paciente
        [HttpGet]
        public async Task<IActionResult> Index(string filtro = "")
        {
            var query = _db.Pacientes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.Trim();
                query = query.Where(p =>
                    p.Dni.Contains(filtro) ||
                    p.Apellidos.Contains(filtro) ||
                    p.Nombres.Contains(filtro) ||
                    (p.CodigoRenaes != null && p.CodigoRenaes.Contains(filtro)));
            }

            var lista = await query
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres)
                .ToListAsync();

            ViewBag.Filtro = filtro;
            return View(lista);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new Paciente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Paciente modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Datos inválidos. Revise el formulario.";
                return View(modelo);
            }

            var errores = ValidarNegocio(modelo);
            if (errores.Count > 0)
            {
                ViewData["Mensaje"] = string.Join(" ", errores);
                return View(modelo);
            }

            _db.Pacientes.Add(modelo);
            await _db.SaveChangesAsync();

            if (modelo.IdPaciente == 0)
            {
                ViewData["Mensaje"] = "No se pudo crear el paciente.";
                return View(modelo);
            }

            TempData["Mensaje"] = $"Paciente {modelo.Nombres} {modelo.Apellidos} creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var paciente = await _db.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Paciente modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Datos inválidos. Revise el formulario.";
                return View(modelo);
            }

            var errores = ValidarNegocio(modelo, modelo.IdPaciente);
            if (errores.Count > 0)
            {
                ViewData["Mensaje"] = string.Join(" ", errores);
                return View(modelo);
            }

            _db.Pacientes.Update(modelo);
            await _db.SaveChangesAsync();

            TempData["Mensaje"] = $"Paciente {modelo.Nombres} {modelo.Apellidos} actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var paciente = await _db.Pacientes.Include(p => p.SolicitudesReferencia).FirstOrDefaultAsync(p => p.IdPaciente == id);
            if (paciente == null)
            {
                return NotFound();
            }

            if (paciente.SolicitudesReferencia.Any())
            {
                ViewData["Mensaje"] = "No se puede eliminar: el paciente tiene referencias asociadas.";
                return View("Index", await _db.Pacientes.ToListAsync());
            }

            _db.Pacientes.Remove(paciente);
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = $"Paciente {paciente.Nombres} {paciente.Apellidos} eliminado.";

            return RedirectToAction(nameof(Index));
        }

        private List<string> ValidarNegocio(Paciente modelo, int? idExistente = null)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(modelo.Dni) || !Regex.IsMatch(modelo.Dni, @"^[0-9]{6,20}$"))
            {
                errores.Add("El DNI/RUN debe contener entre 6 y 20 números.");
            }

            var dniDuplicado = _db.Pacientes.Any(p => p.Dni == modelo.Dni && (!idExistente.HasValue || p.IdPaciente != idExistente));
            if (dniDuplicado)
            {
                errores.Add("Este DNI/RUN ya está registrado para otro paciente.");
            }

            if (modelo.FechaNacimiento >= DateTime.Today)
            {
                errores.Add("La fecha de nacimiento no puede ser posterior a la fecha actual.");
            }

            if (modelo.FechaNacimiento > DateTime.Today.AddYears(-2) && modelo.FechaNacimiento < DateTime.Today.AddYears(-18))
            {
                errores.Add("El paciente debe ser menor de 2 años o mayor de 18 para este registro. Revise la fecha de nacimiento.");
            }

            return errores;
        }
    }
}
