using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OyunKedisi.Models;
using Microsoft.EntityFrameworkCore;

namespace OyunKedisi.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly OyunKedisiDbContext _context;

    public HomeController(ILogger<HomeController> logger, OyunKedisiDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var oyunlar = _context.Oyunlars
            .Include(o => o.User)
            .Include(o => o.Yorumlars).ThenInclude(y => y.User)
            .ToList();
        return View(oyunlar);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminPanel()
    {
        ViewBag.TotalUsers = await _context.Users.CountAsync();
        ViewBag.TotalGames = await _context.Oyunlars.CountAsync();
        ViewBag.TotalComments = await _context.Yorumlars.CountAsync();
        ViewBag.TotalFavorites = await _context.Favoris.CountAsync();
        
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAdminStats()
    {
        var stats = new
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalGames = await _context.Oyunlars.CountAsync(),
            TotalComments = await _context.Yorumlars.CountAsync(),
            TotalFavorites = await _context.Favoris.CountAsync()
        };
        
        return Json(stats);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
