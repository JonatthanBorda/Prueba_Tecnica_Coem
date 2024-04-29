using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prueba_Tecnica_Coem.Models;

namespace Prueba_Tecnica_Coem.Controllers
{
    public class EmpleadoresController : Controller
    {
        private readonly DbPortalCoemContext _context;

        public EmpleadoresController(DbPortalCoemContext context)
        {
            _context = context;
        }

        // GET: Empleadores
        public async Task<IActionResult> Index()
        {
            var dbPortalCoemContext = _context.Empleadores.Include(e => e.IdUsuarioNavigation);
            return View(await dbPortalCoemContext.ToListAsync());
        }

        // GET: Empleadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadore = await _context.Empleadores
                .Include(e => e.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleadore == null)
            {
                return NotFound();
            }

            return View(empleadore);
        }

        // GET: Empleadores/Create
        public IActionResult Create(int userId)
        {
            ViewData["IdUsuario"] = userId;
            return View();
        }

        // POST: Empleadores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,RazonSocial,Localizacion,Industria,NumeroEmpleados")] Empleadores empleadore)
        {
            if (ModelState.IsValid)
            {
                //Se crea el demandante:
                _context.Add(empleadore);
                await _context.SaveChangesAsync();

                TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = true, Message = "Te has registrado correctamente." });
                return RedirectToAction("Login", "Home");
            }

            TempData["result"] = JsonSerializer.Serialize(new Result { IsSuccess = false, Message = "Ha ocurrido un error con el modelo." });
            return View(empleadore);
        }

        // GET: Empleadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadore = await _context.Empleadores.FindAsync(id);
            if (empleadore == null)
            {
                return NotFound();
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "Id", "Clave", empleadore.IdUsuario);
            return View(empleadore);
        }

        // POST: Empleadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdUsuario,RazonSocial,Localizacion,Industria,NumeroEmpleados")] Empleadores empleadore)
        {
            if (id != empleadore.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleadore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoreExists(empleadore.Id))
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
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "Id", "Clave", empleadore.IdUsuario);
            return View(empleadore);
        }

        // GET: Empleadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadore = await _context.Empleadores
                .Include(e => e.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleadore == null)
            {
                return NotFound();
            }

            return View(empleadore);
        }

        // POST: Empleadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleadore = await _context.Empleadores.FindAsync(id);
            if (empleadore != null)
            {
                _context.Empleadores.Remove(empleadore);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadoreExists(int id)
        {
            return _context.Empleadores.Any(e => e.Id == id);
        }
    }
}
