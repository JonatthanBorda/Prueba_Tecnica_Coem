using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prueba_Tecnica_Coem.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Prueba_Tecnica_Coem.Models.Enum;

namespace Prueba_Tecnica_Coem.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DbPortalCoemContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UsuariosController(DbPortalCoemContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var dbPortalCoemContext = _context.Usuarios.Include(u => u.IdTipoUsuarioNavigation);
            return View(await dbPortalCoemContext.ToListAsync());
        }

        // POST: Usuarios/login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Clave")] Usuarios usuario)
        {
            if (ModelState.IsValid)
            {
                //Se busca el usuario con las credenciales:
                var usuarioEncontrado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);

                if (usuarioEncontrado != null)
                {
                    //Se validan las credenciales de acceso:
                    var result = await _signInManager.PasswordSignInAsync(usuario.Email, usuario.Clave, isPersistent: false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        //Se redirecciona de acuerdo con el tipo de usuario:
                        if (usuarioEncontrado.IdTipoUsuario == (int)TiposUsuario.Demandante)
                        {
                            return RedirectToAction("Index", "Demandantes");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Vacantes");
                        }
                    }
                    else
                    {
                        TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = false, Message = "Las credenciales no son validas." });
                        return RedirectToAction("Login", "Home");
                    }
                }
                else
                {
                    //Usuario no encontrado o credenciales incorrectas:
                    TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = false, Message = "Aún no tienes una cuenta. Debes registrarte." });
                    return RedirectToAction("Login", "Home");
                }
            }

            TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = false, Message = "Errores en la validacion del modelo." });
            return View("~/Views/Home/Login.cshtml", usuario);
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

        // POST: Usuarios/logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuarios, "Id", "Tipo");
            return View();   
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create (Usuarios usuario)
        {
            if (ModelState.IsValid)
            {
                ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuarios, "Id", "Tipo", usuario.IdTipoUsuario);

                //Se consulta si ya existe el email en el sistema:
                var usuarioExiste = await _userManager.FindByEmailAsync(usuario.Email);
                if (usuarioExiste == null)
                {
                    var user = new IdentityUser { UserName = usuario.Email, Email = usuario.Email };
                    var result = await _userManager.CreateAsync(user, usuario.Clave);

                    if (result.Succeeded)
                    {
                        //Se añaden roles basados en el tipo de usuario:
                        string roleName = usuario.IdTipoUsuario == (int)TiposUsuario.Demandante ? "Demandante" : "Empleador";
                        await _userManager.AddToRoleAsync(user, roleName);

                        //Se agregan claims específicos:
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Role, roleName),
                        };

                        await _userManager.AddClaimsAsync(user, claims);
                        _context.Add(usuario);
                        int idUsuario = await _context.SaveChangesAsync();

                        //Redireccionar al controlador correspondiente dependiendo del tipo de usuario:
                        return usuario.IdTipoUsuario == (int)TiposUsuario.Demandante
                        ? RedirectToAction("Create", "Demandantes", new { userId = usuario.Id })
                        : RedirectToAction("Create", "Empleadores", new { userId = usuario.Id });
                    }
                    else
                    {
                        //Maneja fallos, como contraseñas que no cumplen con la política de seguridad:
                        foreach (var error in result.Errors)
                        {
                            string translatedError = error.Description switch
                            {
                                "Passwords must be at least 8 characters." => "La contraseña debe tener al menos 8 carácter.",
                                "Passwords must have at least one non alphanumeric character." => "La contraseña debe tener al menos un carácter no alfanumérico.",
                                "Passwords must have at least one digit ('0'-'9')." => "La contraseña debe tener al menos un dígito ('0'-'9').",
                                "Passwords must have at least one uppercase ('A'-'Z')." => "La contraseña debe tener al menos una letra mayúscula ('A'-'Z').",
                                _ => error.Description
                            };

                            TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = false, Message = translatedError });
                            return View();
                        }
                    }
                }
                else
                {
                    TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = false, Message = "El correo electrónico ya está en uso." });
                    return View(usuario);
                }
            }

            TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = false, Message = "Ha ocurrido un error con el modelo." });
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuarios, "Id", "Id", usuario.IdTipoUsuario);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Clave,IdTipoUsuario")] Usuarios usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Email))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuarios, "Id", "Id", usuario.IdTipoUsuario);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdTipoUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(string email)
        {
            return _context.Usuarios.Any(e => e.Email == email);
        }

        public IActionResult Contacto()
        {
            return View();
        }
    }
}
