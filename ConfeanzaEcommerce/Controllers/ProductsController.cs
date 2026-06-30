using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.ViewModels;

namespace ConfeanzaEcommerce.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _db;
    private const int PageSize = 24;

    public ProductsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, string? category, string? brand, string sort = "newest", int page = 1)
    {
        var query = _db.Products
            .Include(p => p.Category).Include(p => p.Brand)
            .Include(p => p.Images).Include(p => p.AffiliateLinks)
            .Where(p => p.Status == "published" && p.DeletedAt == null);

        if (!string.IsNullOrEmpty(q))
            query = query.Where(p => p.Name.Contains(q) || (p.Description != null && p.Description.Contains(q)));

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category != null && p.Category.Slug == category);

        if (!string.IsNullOrEmpty(brand))
            query = query.Where(p => p.Brand != null && p.Brand.Slug == brand);

        query = sort switch
        {
            "price_asc" => query.OrderBy(p => p.LowestPrice),
            "price_desc" => query.OrderByDescending(p => p.LowestPrice),
            "rating" => query.OrderByDescending(p => p.AvgRating),
            "popular" => query.OrderByDescending(p => p.ClickCount),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var products = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

        var vm = new ProductListViewModel
        {
            Products = products,
            Categories = await _db.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(),
            Brands = await _db.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync(),
            SearchTerm = q,
            CategorySlug = category,
            BrandSlug = brand,
            SortBy = sort,
            Page = page,
            TotalCount = totalCount,
            PageSize = PageSize
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(string slug)
    {
        var product = await _db.Products
            .Include(p => p.Category).Include(p => p.Brand)
            .Include(p => p.Images).Include(p => p.AffiliateLinks).ThenInclude(a => a.Store)
            .Include(p => p.Reviews).Include(p => p.Specifications)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == "published" && p.DeletedAt == null);

        if (product == null) return NotFound();

        product.ViewCount++;
        await _db.SaveChangesAsync();

        var related = await _db.Products
            .Include(p => p.Images).Include(p => p.AffiliateLinks)
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.Status == "published" && p.DeletedAt == null)
            .Take(6).ToListAsync();

        var settings = await _db.SiteSettings.FirstOrDefaultAsync();

        var vm = new ProductDetailViewModel
        {
            Product = product,
            AffiliateLinks = product.AffiliateLinks.ToList(),
            Reviews = product.Reviews.Where(r => r.Status == "approved").OrderByDescending(r => r.CreatedAt).ToList(),
            RelatedProducts = related,
            ProductPageNotice = string.IsNullOrWhiteSpace(settings?.ProductPageNotice) ? null : settings.ProductPageNotice,
            CategoryNotice = string.IsNullOrWhiteSpace(product.Category?.PageNotice) ? null : product.Category!.PageNotice
        };

        return View(vm);
    }
}
