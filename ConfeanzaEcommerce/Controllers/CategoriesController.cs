using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;

namespace ConfeanzaEcommerce.Controllers;

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _db;

    public CategoriesController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var categories = await _db.Categories
            .Include(c => c.Subcategories)
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    public async Task<IActionResult> Details(string slug, string sort = "newest", int page = 1)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);
        if (category == null) return NotFound();

        const int pageSize = 24;
        var query = _db.Products
            .Include(p => p.Brand).Include(p => p.Images).Include(p => p.AffiliateLinks)
            .Where(p => p.CategoryId == category.Id && p.Status == "published" && p.DeletedAt == null);

        query = sort switch
        {
            "price_asc" => query.OrderBy(p => p.LowestPrice),
            "price_desc" => query.OrderByDescending(p => p.LowestPrice),
            "rating" => query.OrderByDescending(p => p.AvgRating),
            "popular" => query.OrderByDescending(p => p.ClickCount),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var total = await query.CountAsync();
        var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewBag.Category = category;
        ViewBag.Products = products;
        ViewBag.Total = total;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Sort = sort;
        return View(category);
    }
}
