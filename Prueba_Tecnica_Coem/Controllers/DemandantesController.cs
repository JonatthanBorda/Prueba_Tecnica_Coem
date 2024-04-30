using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prueba_Tecnica_Coem.Models;
using static System.Net.Mime.MediaTypeNames;
using static Prueba_Tecnica_Coem.Models.Enum;

namespace Prueba_Tecnica_Coem.Controllers
{
    public class DemandantesController : Controller
    {
        private readonly DbPortalCoemContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public DemandantesController(DbPortalCoemContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var demandanteId = await GetDemandanteIdAsync();
            var vacantes = await _context.Vacantes
                        .Include(v => v.IdEmpleadorNavigation)
                        .Include(v => v.Aplicaciones)
                        .ToListAsync();

            var viewModel = vacantes.Select(v => new DemandanteViewModel
            {
                Id = v.Id,
                RazonSocial = v.IdEmpleadorNavigation.RazonSocial,
                Descripcion = v.Descripcion,
                Requisitos = v.Requisitos,
                Industria = v.IdEmpleadorNavigation.Industria,
                HaAplicado = v.Aplicaciones.Any(a => a.IdDemandante == demandanteId),
                EstadoAplicacion = v.Aplicaciones
                    .Where(a => a.IdDemandante == demandanteId)
                    .Select(a => System.Enum.GetName(typeof(EstadosAplicacion), a.IdEstado))
                    .FirstOrDefault() ?? "No Aplicada"
            }).ToList();

            return View(viewModel);
        }

        // GET: Demandantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacantes = await _context.Vacantes
                .Include(v => v.IdEmpleadorNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vacantes == null)
            {
                return NotFound();
            }

            return View(vacantes);
        }

        // GET: Demandantes/Create
        public IActionResult Create(int userId)
        {
            ViewData["IdNivelEducativo"] = new SelectList(_context.NivelEducativos, "Id", "Nivel");
            ViewData["IdUsuario"] = userId;
            return View();
        }

        // POST: Demandantes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,Nombres,Apellidos,FechaNacimiento,Celular,IdNivelEducativo,Notas,ExperienciaAnterior")] Demandantes demandante)
        {
            if (ModelState.IsValid)
            {
                //Se crea el demandante:
                _context.Add(demandante);
                await _context.SaveChangesAsync();

                TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = true, Message = "Te has registrado correctamente." });
                return RedirectToAction("Login", "Home");
            }

            ViewData["IdNivelEducativo"] = new SelectList(_context.NivelEducativos, "Id", "Nivel", demandante.IdNivelEducativo);
            TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = false, Message = "Ha ocurrido un error con el modelo." });
            return View(demandante);
        }

        // GET: Demandantes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var demandante = await _context.Demandantes.FindAsync(id);
            if (demandante == null)
            {
                return NotFound();
            }
            ViewData["IdNivelEducativo"] = new SelectList(_context.NivelEducativos, "Id", "Id", demandante.IdNivelEducativo);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "Id", "Clave", demandante.IdUsuario);
            return View(demandante);
        }

        // POST: Demandantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdUsuario,Nombres,Apellidos,FechaNacimiento,Celular,IdNivelEducativo,Notas,ExperienciaAnterior")] Demandantes demandante)
        {
            if (id != demandante.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(demandante);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DemandanteExists(demandante.Id))
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
            ViewData["IdNivelEducativo"] = new SelectList(_context.NivelEducativos, "Id", "Id", demandante.IdNivelEducativo);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "Id", "Clave", demandante.IdUsuario);
            return View(demandante);
        }

        // GET: Demandantes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var demandante = await _context.Demandantes
                .Include(d => d.IdNivelEducativoNavigation)
                .Include(d => d.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (demandante == null)
            {
                return NotFound();
            }

            return View(demandante);
        }

        // POST: Demandantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var demandante = await _context.Demandantes.FindAsync(id);
            if (demandante != null)
            {
                _context.Demandantes.Remove(demandante);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DemandanteExists(int id)
        {
            return _context.Demandantes.Any(e => e.Id == id);
        }

        private async Task<int?> GetDemandanteIdAsync()
        {
            var userName = _userManager.GetUserName(User);
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userName);
            if (usuario == null)
            {
                return null;
            }

            var demandante = await _context.Demandantes.FirstOrDefaultAsync(e => e.IdUsuario == usuario.Id);
            return demandante?.Id;
        }
    }
}
