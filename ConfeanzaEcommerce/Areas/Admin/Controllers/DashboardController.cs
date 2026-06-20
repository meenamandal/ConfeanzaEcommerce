using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;

    public DashboardController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.ProductCount = await _db.Products.CountAsync(p => p.DeletedAt == null);
        ViewBag.CategoryCount = await _db.Categories.CountAsync(c => c.IsActive);
        ViewBag.StoreCount = await _db.Stores.CountAsync(s => s.IsActive);
        ViewBag.BlogCount = await _db.Blogs.CountAsync(b => b.Status == "published");
        ViewBag.CouponCount = await _db.Coupons.CountAsync(c => c.IsActive);
        ViewBag.DealCount = await _db.Deals.CountAsync(d => d.IsActive);

        ViewBag.RecentProducts = await _db.Products
            .Include(p => p.Category).Include(p => p.Brand)
            .Where(p => p.DeletedAt == null)
            .OrderByDescending(p => p.CreatedAt).Take(5).ToListAsync();

        return View();
    }
}
