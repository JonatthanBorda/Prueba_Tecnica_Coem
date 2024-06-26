﻿using System;
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
using static Prueba_Tecnica_Coem.Models.Enum;

namespace Prueba_Tecnica_Coem.Controllers
{
    public class VacantesController : Controller
    {
        private readonly DbPortalCoemContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public VacantesController(DbPortalCoemContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "Empleador")]
        public async Task<IActionResult> Index()
        {
            var empleadorId = await GetEmpleadorIdAsync();

            if (empleadorId == null)
            {
                return View(new List<VacanteViewModel>());
            }

            var vacantes = await _context.Vacantes
                .Include(v => v.IdEmpleadorNavigation)
                .Where(v => v.IdEmpleador == empleadorId)
                .Select(v => new VacanteViewModel
                {
                    Id = v.Id,
                    Descripcion = v.Descripcion,
                    Requisitos = v.Requisitos,
                    Industria = v.IdEmpleadorNavigation.Industria,
                    NumeroDeAplicaciones = v.Aplicaciones.Count
                })
                .ToListAsync();

            return View(vacantes);
        }

        [Authorize(Roles = "Empleador")]
        public async Task<IActionResult> VerAplicantes(int id)
        {
            // Buscar todas las aplicaciones para la vacante especificada
            var aplicaciones = await _context.Aplicaciones
                .Include(a => a.IdDemandanteNavigation)
                .Include(a => a.IdDemandanteNavigation.IdNivelEducativoNavigation)
                .Where(a => a.IdVacante == id)
                .ToListAsync();

            // Opcionalmente, puedes transformar estas aplicaciones en un ViewModel si es necesario
            return View(aplicaciones); // Envía los datos a una vista que mostrará los demandantes
        }

        // GET: Vacantes/Details/5
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

        // GET: Vacantes/Details/5
        [Authorize(Roles = "Demandante")]
        public async Task<IActionResult> Aplicacion(int id)
        {
            Aplicaciones aplicacion = new()
            {
                IdVacante = id,
                IdDemandante = (int)await GetDemandanteIdAsync(),
                IdEstado = (int)EstadosAplicacion.Enviada
            };

            _context.Aplicaciones.Add(aplicacion);
            await _context.SaveChangesAsync();

            TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = true, Message = "Has aplicado correctamente a este empleo." });
            return RedirectToAction("Index", "Demandantes");
        }


        // GET: Vacantes/Create
        public async Task<IActionResult> Create()
        {
            var empleadorId = await GetEmpleadorIdAsync();

            ViewData["IdEmpleador"] = empleadorId;
            return View();
        }


        // POST: Vacantes/Create
        [Authorize(Roles = "Empleador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleador,Descripcion,Requisitos")] Vacantes vacantes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vacantes);
                await _context.SaveChangesAsync();

                TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = true, Message = "El empleo se ha publicado correctamente." });
                return RedirectToAction(nameof(Index));
            }
            TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = false, Message = "Ha ocurrido un error con el modelo." });
            return View(vacantes);
        }

        private async Task<int?> GetEmpleadorIdAsync()
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

            var empleador = await _context.Empleadores.FirstOrDefaultAsync(e => e.IdUsuario == usuario.Id);
            return empleador?.Id;
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

        // GET: Vacantes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacantes = await _context.Vacantes.FindAsync(id);
            if (vacantes == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleador"] = new SelectList(_context.Empleadores, "Id", "Industria", vacantes.IdEmpleador);
            return View(vacantes);
        }

        // POST: Vacantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdEmpleador,Descripcion,Requisitos")] Vacantes vacantes)
        {
            if (id != vacantes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vacantes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacantesExists(vacantes.Id))
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
            ViewData["IdEmpleador"] = new SelectList(_context.Empleadores, "Id", "Industria", vacantes.IdEmpleador);
            return View(vacantes);
        }

        // GET: Vacantes/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Vacantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vacantes = await _context.Vacantes.FindAsync(id);
            if (vacantes != null)
            {
                _context.Vacantes.Remove(vacantes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VacantesExists(int id)
        {
            return _context.Vacantes.Any(e => e.Id == id);
        }
    }
}
