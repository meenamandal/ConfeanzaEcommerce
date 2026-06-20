using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _db;

    public ProductsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        const int pageSize = 20;
        var query = _db.Products.Include(p => p.Category).Include(p => p.Brand)
            .Where(p => p.DeletedAt == null);

        if (!string.IsNullOrEmpty(q))
            query = query.Where(p => p.Name.Contains(q));

        var total = await query.CountAsync();
        var products = await query.OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewBag.Total = total;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Query = q;
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _db.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
        ViewBag.Brands = await _db.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();
        return View(new Product());
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        product.Id = Guid.NewGuid().ToString();
        product.Slug = Slugify(product.Name);
        product.CreatedAt = product.UpdatedAt = DateTime.UtcNow;
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Product created. Now add prices below.";
        return RedirectToAction("Index", "AffiliateLinks", new { productId = product.Id });
    }

    public async Task<IActionResult> Edit(string id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        ViewBag.Categories = await _db.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
        ViewBag.Brands = await _db.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();
        return View(product);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Product model)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        product.Name = model.Name;
        product.Description = model.Description;
        product.ShortDescription = model.ShortDescription;
        product.CategoryId = string.IsNullOrEmpty(model.CategoryId) ? null : model.CategoryId;
        product.BrandId = string.IsNullOrEmpty(model.BrandId) ? null : model.BrandId;
        product.Status = model.Status;
        product.IsFeatured = model.IsFeatured;
        product.IsTrending = model.IsTrending;
        product.IsDealOfDay = model.IsDealOfDay;
        product.Sku = model.Sku;
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Product updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product != null) { product.DeletedAt = DateTime.UtcNow; await _db.SaveChangesAsync(); }
        TempData["Success"] = "Product deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static string Slugify(string text) =>
        System.Text.RegularExpressions.Regex.Replace(text.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-');
}
