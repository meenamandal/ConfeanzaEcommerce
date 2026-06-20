using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class BrandsController : Controller
{
    private readonly ApplicationDbContext _db;
    public BrandsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var brands = await _db.Brands.OrderBy(b => b.Name).ToListAsync();
        return View(brands);
    }

    public IActionResult Create() => View(new Brand());

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Brand model)
    {
        model.Id = Guid.NewGuid().ToString();
        model.Slug = Slugify(model.Name);
        model.CreatedAt = model.UpdatedAt = DateTime.UtcNow;
        _db.Brands.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Brand created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var brand = await _db.Brands.FindAsync(id);
        if (brand == null) return NotFound();
        return View(brand);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Brand model)
    {
        var brand = await _db.Brands.FindAsync(id);
        if (brand == null) return NotFound();
        brand.Name = model.Name;
        brand.Description = model.Description;
        brand.LogoUrl = model.LogoUrl;
        brand.WebsiteUrl = model.WebsiteUrl;
        brand.Country = model.Country;
        brand.IsActive = model.IsActive;
        brand.IsFeatured = model.IsFeatured;
        brand.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Brand updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var brand = await _db.Brands.FindAsync(id);
        if (brand != null) { _db.Brands.Remove(brand); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Brand deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static string Slugify(string text) =>
        System.Text.RegularExpressions.Regex.Replace(text.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-');
}
