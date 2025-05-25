using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OyunKedisi.Models;
using Microsoft.AspNetCore.Authorization;

namespace OyunKedisi.Controllers
{
    public class YetkisController : Controller
    {
        private readonly OyunKedisiDbContext _context;

        public YetkisController(OyunKedisiDbContext context)
        {
            _context = context;
        }

        // GET: Yetkis
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Yetkis.ToListAsync());
        }

        // GET: Yetkis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yetki = await _context.Yetkis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (yetki == null)
            {
                return NotFound();
            }

            return View(yetki);
        }

        // GET: Yetkis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Yetkis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,YetkiAdi")] Yetki yetki)
        {
            if (ModelState.IsValid)
            {
                _context.Add(yetki);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(yetki);
        }

        // GET: Yetkis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yetki = await _context.Yetkis.FindAsync(id);
            if (yetki == null)
            {
                return NotFound();
            }
            return View(yetki);
        }

        // POST: Yetkis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,YetkiAdi")] Yetki yetki)
        {
            if (id != yetki.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yetki);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YetkiExists(yetki.Id))
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
            return View(yetki);
        }

        // GET: Yetkis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yetki = await _context.Yetkis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (yetki == null)
            {
                return NotFound();
            }

            return View(yetki);
        }

        // POST: Yetkis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yetki = await _context.Yetkis.FindAsync(id);
            if (yetki != null)
            {
                _context.Yetkis.Remove(yetki);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool YetkiExists(int id)
        {
            return _context.Yetkis.Any(e => e.Id == id);
        }
    }
}
