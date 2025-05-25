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
    public class FavorisController : Controller
    {
        private readonly OyunKedisiDbContext _context;

        public FavorisController(OyunKedisiDbContext context)
        {
            _context = context;
        }

        // GET: Favoris
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var oyunKedisiDbContext = _context.Favoris.Include(f => f.Oyun).Include(f => f.User);
            return View(await oyunKedisiDbContext.ToListAsync());
        }

        // GET: Favoris/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favori = await _context.Favoris
                .Include(f => f.Oyun)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (favori == null)
            {
                return NotFound();
            }

            return View(favori);
        }

        // GET: Favoris/Create
        public IActionResult Create(int? oyunId)
        {
            if (oyunId.HasValue)
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Users");
                }

                var favori = new Favori
                {
                    UserId = userId,
                    OyunId = oyunId.Value
                };

                ViewData["OyunId"] = new SelectList(_context.Oyunlars, "Id", "Id", oyunId.Value);
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userId);
                return View(favori);
            }

            ViewData["OyunId"] = new SelectList(_context.Oyunlars, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Favoris/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,OyunId")] Favori favori)
        {
            if (ModelState.IsValid)
            {
                _context.Add(favori);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OyunId"] = new SelectList(_context.Oyunlars, "Id", "Id", favori.OyunId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favori.UserId);
            return View(favori);
        }

        // GET: Favoris/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favori = await _context.Favoris.FindAsync(id);
            if (favori == null)
            {
                return NotFound();
            }
            ViewData["OyunId"] = new SelectList(_context.Oyunlars, "Id", "Id", favori.OyunId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favori.UserId);
            return View(favori);
        }

        // POST: Favoris/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,OyunId")] Favori favori)
        {
            if (id != favori.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favori);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavoriExists(favori.Id))
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
            ViewData["OyunId"] = new SelectList(_context.Oyunlars, "Id", "Id", favori.OyunId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favori.UserId);
            return View(favori);
        }

        // GET: Favoris/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favori = await _context.Favoris
                .Include(f => f.Oyun)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (favori == null)
            {
                return NotFound();
            }

            return View(favori);
        }

        // POST: Favoris/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var favori = await _context.Favoris.FindAsync(id);
            if (favori != null)
            {
                _context.Favoris.Remove(favori);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Favoris/ToggleFavorite/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            // Önce favori var mı kontrol et
            var existingFavorite = await _context.Favoris
                .FirstOrDefaultAsync(f => f.UserId == userId && f.OyunId == id);

            if (existingFavorite != null)
            {
                // Favori varsa kaldır
                _context.Favoris.Remove(existingFavorite);
            }
            else
            {
                // Favori yoksa ekle
                var favori = new Favori
                {
                    UserId = userId,
                    OyunId = id
                };
                _context.Favoris.Add(favori);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: Favoris/CheckFavorite/5
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CheckFavorite(int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            var isFavorite = await _context.Favoris
                .AnyAsync(f => f.UserId == userId && f.OyunId == id);

            return Json(new { isFavorite });
        }

        // GET: Favoris/GetUserFavorites
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserFavorites()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            var favoriteIds = await _context.Favoris
                .Where(f => f.UserId == userId)
                .Select(f => f.OyunId)
                .ToListAsync();

            return Json(favoriteIds);
        }

        private bool FavoriExists(int id)
        {
            return _context.Favoris.Any(e => e.Id == id);
        }
    }
}
