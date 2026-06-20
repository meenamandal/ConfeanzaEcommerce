using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.ViewModels;

namespace ConfeanzaEcommerce.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new HomeViewModel
        {
            FeaturedCategories = await _db.Categories
                .Where(c => c.IsActive && c.IsFeatured)
                .OrderBy(c => c.SortOrder).Take(12).ToListAsync(),

            FeaturedProducts = await _db.Products
                .Include(p => p.Category).Include(p => p.Brand)
                .Include(p => p.Images).Include(p => p.AffiliateLinks)
                .Where(p => p.Status == "published" && p.IsFeatured && p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt).Take(12).ToListAsync(),

            TrendingProducts = await _db.Products
                .Include(p => p.Category).Include(p => p.Images).Include(p => p.AffiliateLinks)
                .Where(p => p.Status == "published" && p.IsTrending && p.DeletedAt == null)
                .OrderByDescending(p => p.ClickCount).Take(8).ToListAsync(),

            ActiveDeals = await _db.Deals.Include(d => d.Store)
                .Where(d => d.IsActive && (d.ExpiresAt == null || d.ExpiresAt > DateTime.UtcNow))
                .OrderByDescending(d => d.IsFeatured).Take(6).ToListAsync(),

            ActiveCoupons = await _db.Coupons.Include(c => c.Store)
                .Where(c => c.IsActive && (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow))
                .OrderByDescending(c => c.IsVerified).Take(6).ToListAsync(),

            LatestBlogs = await _db.Blogs
                .Where(b => b.Status == "published")
                .OrderByDescending(b => b.PublishedAt).Take(3).ToListAsync(),

            FeaturedStores = await _db.Stores
                .Where(s => s.IsActive && s.IsFeatured)
                .OrderBy(s => s.SortOrder).Take(8).ToListAsync(),

            FeaturedBrands = await _db.Brands
                .Where(b => b.IsActive && b.IsFeatured).Take(12).ToListAsync(),
        };

        return View(vm);
    }

    public IActionResult Privacy() => View();

    [Route("error")]
    public IActionResult Error() => View();
}
