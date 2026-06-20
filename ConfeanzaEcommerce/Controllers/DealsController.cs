using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;

namespace ConfeanzaEcommerce.Controllers;

public class DealsController : Controller
{
    private readonly ApplicationDbContext _db;

    public DealsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? type)
    {
        var query = _db.Deals.Include(d => d.Store)
            .Where(d => d.IsActive && (d.ExpiresAt == null || d.ExpiresAt > DateTime.UtcNow));

        if (!string.IsNullOrEmpty(type))
            query = query.Where(d => d.DealType == type);

        var deals = await query.OrderByDescending(d => d.IsFeatured).ThenByDescending(d => d.CreatedAt).ToListAsync();
        ViewBag.Type = type;
        return View(deals);
    }
}
