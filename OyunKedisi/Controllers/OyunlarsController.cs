using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OyunKedisi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace OyunKedisi.Controllers
{
    public class OyunlarsController : Controller
    {
        private readonly OyunKedisiDbContext _context;

        public OyunlarsController(OyunKedisiDbContext context)
        {
            _context = context;
        }

        // GET: Oyunlars
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.User;
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "UserId");
            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role);
            IQueryable<Oyunlar> oyunlarQuery = _context.Oyunlars.Include(o => o.User);
            if (roleClaim == null || roleClaim.Value != "Admin")
            {
                if (userIdClaim != null)
                {
                    int userId = int.Parse(userIdClaim.Value);
                    oyunlarQuery = oyunlarQuery.Where(o => o.UserId == userId);
                }
            }
            return View(await oyunlarQuery.ToListAsync());
        }

        // GET: Oyunlars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oyunlar = await _context.Oyunlars
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (oyunlar == null)
            {
                return NotFound();
            }

            return View(oyunlar);
        }

        // GET: Oyunlars/Create
        [Authorize]
        public IActionResult Create()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Users", new { returnUrl = Url.Action("Create", "Oyunlars") });
            }

            var oyunlar = new Oyunlar
            {
                UserId = userId
            };
            return View(oyunlar);
        }

        // POST: Oyunlars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,OyunAdi,OyunLinki")] Oyunlar oyunlar, IFormFile oyunFotografi)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Users", new { returnUrl = Url.Action("Create", "Oyunlars") });
            }

            if (ModelState.IsValid)
            {
                if (oyunFotografi != null && oyunFotografi.Length > 0)
                {
                    // Dosya uzantısını kontrol et
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(oyunFotografi.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("OyunFotograflari", "Sadece .jpg, .jpeg, .png ve .gif uzantılı dosyalar yüklenebilir.");
                        return View(oyunlar);
                    }

                    // Dosya boyutunu kontrol et (max 5MB)
                    if (oyunFotografi.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("OyunFotograflari", "Dosya boyutu 5MB'dan büyük olamaz.");
                        return View(oyunlar);
                    }

                    // Benzersiz dosya adı oluştur
                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "oyunlar");
                    
                    // Uploads klasörü yoksa oluştur
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Dosyayı kaydet
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await oyunFotografi.CopyToAsync(stream);
                    }

                    // Veritabanına kaydedilecek yolu ayarla
                    oyunlar.OyunFotograflari = $"/uploads/oyunlar/{fileName}";
                }

                oyunlar.UserId = userId;
                _context.Add(oyunlar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(oyunlar);
        }

        // GET: Oyunlars/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oyunlar = await _context.Oyunlars.FindAsync(id);
            if (oyunlar == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && userId != oyunlar.UserId)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(oyunlar);
        }

        // POST: Oyunlars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OyunAdi,UserId,OyunFotograflari,OyunLinki")] Oyunlar oyunlar)
        {
            if (id != oyunlar.Id)
            {
                return NotFound();
            }

            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && userId != oyunlar.UserId)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(oyunlar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OyunlarExists(oyunlar.Id))
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
            return View(oyunlar);
        }

        // GET: Oyunlars/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oyunlar = await _context.Oyunlars
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (oyunlar == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && userId != oyunlar.UserId)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(oyunlar);
        }

        // POST: Oyunlars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oyunlar = await _context.Oyunlars.FindAsync(id);
            if (oyunlar == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && userId != oyunlar.UserId)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            _context.Oyunlars.Remove(oyunlar);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OyunlarExists(int id)
        {
            return _context.Oyunlars.Any(e => e.Id == id);
        }
    }
}
