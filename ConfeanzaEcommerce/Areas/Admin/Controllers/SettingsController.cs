using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class SettingsController : Controller
{
    private readonly ApplicationDbContext _db;
    public SettingsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var settings = await _db.SiteSettings.FirstOrDefaultAsync()
                       ?? new SiteSettings();
        return View(settings);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SiteSettings model)
    {
        var settings = await _db.SiteSettings.FirstOrDefaultAsync();

        if (settings == null)
        {
            settings = new SiteSettings { Id = 1 };
            _db.SiteSettings.Add(settings);
        }

        settings.ProductPageNotice = string.IsNullOrWhiteSpace(model.ProductPageNotice)
            ? null
            : model.ProductPageNotice.Trim();
        settings.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Settings saved.";
        return RedirectToAction(nameof(Index));
    }
}
