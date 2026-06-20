using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;

namespace ConfeanzaEcommerce.Controllers;

public class StoresController : Controller
{
    private readonly ApplicationDbContext _db;

    public StoresController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var stores = await _db.Stores
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.IsFeatured).ThenBy(s => s.SortOrder)
            .ToListAsync();

        return View(stores);
    }

    public async Task<IActionResult> Details(string slug)
    {
        var store = await _db.Stores.FirstOrDefaultAsync(s => s.Slug == slug && s.IsActive);
        if (store == null) return NotFound();

        var coupons = await _db.Coupons
            .Where(c => c.StoreId == store.Id && c.IsActive && (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow))
            .OrderByDescending(c => c.IsVerified).ToListAsync();

        var deals = await _db.Deals
            .Where(d => d.StoreId == store.Id && d.IsActive)
            .OrderByDescending(d => d.CreatedAt).ToListAsync();

        ViewBag.Coupons = coupons;
        ViewBag.Deals = deals;
        return View(store);
    }
}
