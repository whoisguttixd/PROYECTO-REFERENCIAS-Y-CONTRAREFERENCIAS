using PROYECTO_REFERENCIAS.Data;
using PROYECTO_REFERENCIAS.Models;
using PROYECTO_REFERENCIAS.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PROYECTO_REFERENCIAS.Controllers
{
    public class LoginController : Controller
    {
        //Creacion de variable de tipo DbContext de solo lectura
        private readonly AppDbContext _appDbContext;
        //Inicialización de la variable de tipo DbContext con todos los permisos
        public LoginController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        //[get] = redirecciones entre paginas
        //[post] = envío de informacion a la DB
        [HttpGet]
        public IActionResult Registro()
        {
            ViewBag.Roles = _appDbContext.Roles.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registro(UsuarioVM usuarioVM)
        {
            ViewBag.Roles = _appDbContext.Roles.ToList();
            if(usuarioVM.Password != usuarioVM.RepPassword)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }
            Usuario usuario = new Usuario()
            {
                NomUsuario = usuarioVM.NomUsuario,
                Correo = usuarioVM.Correo,
                Telefono = usuarioVM.Telefono,
                Password = usuarioVM.Password,
                IdRol = usuarioVM.IdRol
            };
            await _appDbContext.Usuarios.AddAsync(usuario);
            await _appDbContext.SaveChangesAsync();
            if(usuario.IdUsuario != 0)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewData["Mensaje"] = "El usuario no pudo ser creado";
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            Usuario? usuario = await _appDbContext.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Correo == loginVM.Correo && u.Password == loginVM.Password);
            if(usuario == null)
            {
                ViewData["Mensaje"] = "El usuario no existe";
                return View();
            }
            //Claims
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()
                    ),
                new Claim(
                    ClaimTypes.Name,
                    usuario.NomUsuario
                    ),
                new Claim(
                    ClaimTypes.Email,
                    usuario.Correo),
                new Claim(
                    ClaimTypes.Role,
                    usuario.Rol.NomRol)
            };
            //crear identidad
            var identidad = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme
            );
            //claim principal
            var principal = new ClaimsPrincipal(identidad);
            //cookie para la autenticacion
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, principal
                );
            //crear sesion
            HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
            HttpContext.Session.SetString("NombreUsuario", usuario.NomUsuario);
            HttpContext.Session.SetString("CorreoUsuario", usuario.Correo);
            HttpContext.Session.SetString("RolUsuario", usuario.Rol.NomRol);
            //Redirecciones
            return RedirectToAction("Index", "Home");

            /*switch (usuario.Rol.NomRol)
            {
                case "Administrador":
                    return RedirectToAction("Index", "Home");
                case "Empleado":
                    return RedirectToAction("Privacy", "Home");
                case "Usuario":
                    return RedirectToAction("Registro", "Login");
                default:
                    return View();
            }*/
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
        //Acceso Denegado
        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
