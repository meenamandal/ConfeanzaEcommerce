using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _db;
    public CategoriesController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var cats = await _db.Categories.Include(c => c.Subcategories)
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name).ToListAsync();
        return View(cats);
    }

    public IActionResult Create() => View(new Category());

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category model)
    {
        model.Id = Guid.NewGuid().ToString();
        model.Slug = Slugify(model.Name);
        model.CreatedAt = model.UpdatedAt = DateTime.UtcNow;
        _db.Categories.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Category created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var cat = await _db.Categories.FindAsync(id);
        if (cat == null) return NotFound();
        return View(cat);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Category model)
    {
        var cat = await _db.Categories.FindAsync(id);
        if (cat == null) return NotFound();
        cat.Name = model.Name;
        cat.Description = model.Description;
        cat.IconUrl = model.IconUrl;
        cat.ImageUrl = model.ImageUrl;
        cat.SortOrder = model.SortOrder;
        cat.IsActive = model.IsActive;
        cat.IsFeatured = model.IsFeatured;
        cat.MetaTitle = model.MetaTitle;
        cat.MetaDescription = model.MetaDescription;
        cat.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Category updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var cat = await _db.Categories.FindAsync(id);
        if (cat != null) { _db.Categories.Remove(cat); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Category deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static string Slugify(string text) =>
        System.Text.RegularExpressions.Regex.Replace(text.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-');
}
