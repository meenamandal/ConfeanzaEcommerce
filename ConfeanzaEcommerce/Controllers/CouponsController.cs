using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;

namespace ConfeanzaEcommerce.Controllers;

public class CouponsController : Controller
{
    private readonly ApplicationDbContext _db;

    public CouponsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? store, string? type)
    {
        var query = _db.Coupons.Include(c => c.Store)
            .Where(c => c.IsActive && (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow));

        if (!string.IsNullOrEmpty(store))
            query = query.Where(c => c.Store != null && c.Store.Slug == store);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(c => c.Type == type);

        var coupons = await query.OrderByDescending(c => c.IsVerified).ThenByDescending(c => c.CreatedAt).ToListAsync();
        var stores = await _db.Stores.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();

        ViewBag.Stores = stores;
        ViewBag.SelectedStore = store;
        ViewBag.SelectedType = type;
        return View(coupons);
    }
}
