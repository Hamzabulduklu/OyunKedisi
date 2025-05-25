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
    public class YorumlarsController : Controller
    {
        private readonly OyunKedisiDbContext _context;

        public YorumlarsController(OyunKedisiDbContext context)
        {
            _context = context;
        }

        // GET: Yorumlars
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var oyunKedisiDbContext = _context.Yorumlars.Include(y => y.Oyun).Include(y => y.User);
            ViewBag.Users = new SelectList(_context.Users, "Id", "KullaniciAdi");
            ViewBag.Oyunlar = new SelectList(_context.Oyunlars, "Id", "OyunAdi");
            return View(await oyunKedisiDbContext.ToListAsync());
        }

        // GET: Yorumlars/GetYorumlar
        [HttpGet]
        public async Task<IActionResult> GetYorumlar(int oyunId)
        {
            var yorumlar = await _context.Yorumlars
                .Include(y => y.User)
                .Where(y => y.OyunId == oyunId)
                .OrderByDescending(y => y.Id)
                .ToListAsync();

            return Json(yorumlar);
        }

        // GET: Yorumlars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yorumlar = await _context.Yorumlars
                .Include(y => y.Oyun)
                .Include(y => y.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (yorumlar == null)
            {
                return NotFound();
            }

            return View(yorumlar);
        }

        // GET: Yorumlars/Create
        public IActionResult Create()
        {
            ViewData["OyunId"] = new SelectList(_context.Oyunlars, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Yorumlars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] Yorumlar yorumlar)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(yorumlar);
                    await _context.SaveChangesAsync();
                    
                    var yeniYorum = await _context.Yorumlars
                        .Include(y => y.User)
                        .Include(y => y.Oyun)
                        .FirstOrDefaultAsync(y => y.Id == yorumlar.Id);
                    
                    return Json(new { 
                        success = true, 
                        yorum = new {
                            id = yeniYorum.Id,
                            yorumlar1 = yeniYorum.Yorumlar1,
                            user = new {
                                kullaniciAdi = yeniYorum.User?.KullaniciAdi
                            },
                            oyun = new {
                                oyunAdi = yeniYorum.Oyun?.OyunAdi
                            }
                        }
                    });
                }
                return Json(new { 
                    success = false, 
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList() 
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    error = "Yorum eklenirken bir hata oluştu: " + ex.Message 
                });
            }
        }

        // GET: Yorumlars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yorumlar = await _context.Yorumlars.FindAsync(id);
            if (yorumlar == null)
            {
                return NotFound();
            }
            ViewData["OyunId"] = new SelectList(_context.Oyunlars, "Id", "Id", yorumlar.OyunId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", yorumlar.UserId);
            return View(yorumlar);
        }

        // POST: Yorumlars/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] Yorumlar yorumlar)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yorumlar);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YorumlarExists(yorumlar.Id))
                    {
                        return Json(new { success = false, error = "Yorum bulunamadı." });
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        // GET: Yorumlars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yorumlar = await _context.Yorumlars
                .Include(y => y.Oyun)
                .Include(y => y.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (yorumlar == null)
            {
                return NotFound();
            }

            return View(yorumlar);
        }

        // POST: Yorumlars/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var yorumlar = await _context.Yorumlars.FindAsync(id);
            if (yorumlar != null)
            {
                _context.Yorumlars.Remove(yorumlar);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, error = "Yorum bulunamadı." });
        }

        private bool YorumlarExists(int id)
        {
            return _context.Yorumlars.Any(e => e.Id == id);
        }
    }
}
