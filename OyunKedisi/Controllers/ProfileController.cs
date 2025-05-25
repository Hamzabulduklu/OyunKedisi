using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OyunKedisi.Models;

namespace OyunKedisi.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly OyunKedisiDbContext _context;

        public ProfileController(OyunKedisiDbContext context)
        {
            _context = context;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            // Kullanıcı bilgilerini al
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Users");
            }

            var user = await _context.Users
                .Include(u => u.Yetki)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Kullanıcının oyunlarını al
            var userGames = await _context.Oyunlars
                .Where(o => o.UserId == userId)
                .ToListAsync();

            // Kullanıcının favorilerini al
            var userFavorites = await _context.Favoris
                .Include(f => f.Oyun)
                .Where(f => f.UserId == userId)
                .ToListAsync();

            // ViewModel oluştur
            var viewModel = new ProfileViewModel
            {
                User = user,
                Games = userGames,
                Favorites = userFavorites
            };

            return View(viewModel);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Users");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,KullaniciAdi,SifreHash,Mail,Yas,YetkiId")] User user)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            if (userId != user.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Profile/MyGames
        public async Task<IActionResult> MyGames()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Users");
            }

            var userGames = await _context.Oyunlars
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return View(userGames);
        }

        // GET: Profile/MyFavorites
        public async Task<IActionResult> MyFavorites()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Users");
            }

            var userFavorites = await _context.Favoris
                .Include(f => f.Oyun)
                .Where(f => f.UserId == userId)
                .ToListAsync();

            return View(userFavorites);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
} 