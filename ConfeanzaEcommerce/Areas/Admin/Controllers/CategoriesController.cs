using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public CategoriesController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var cats = await _db.Categories.Include(c => c.Subcategories)
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name).ToListAsync();
        return View(cats);
    }

    public IActionResult Create() => View(new Category());

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category model, IFormFile? imageFile)
    {
        model.Id = Guid.NewGuid().ToString();
        model.Slug = Slugify(model.Name);
        model.CreatedAt = model.UpdatedAt = DateTime.UtcNow;

        if (imageFile != null && imageFile.Length > 0)
            model.ImageUrl = await SaveImage(imageFile, model.Id);

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
    public async Task<IActionResult> Edit(string id, Category model, IFormFile? imageFile)
    {
        var cat = await _db.Categories.FindAsync(id);
        if (cat == null) return NotFound();

        cat.Name = model.Name;
        cat.Description = model.Description;
        cat.SortOrder = model.SortOrder;
        cat.IsActive = model.IsActive;
        cat.IsFeatured = model.IsFeatured;
        cat.MetaTitle = model.MetaTitle;
        cat.MetaDescription = model.MetaDescription;
        cat.PageNotice = string.IsNullOrWhiteSpace(model.PageNotice) ? null : model.PageNotice.Trim();
        cat.UpdatedAt = DateTime.UtcNow;

        if (imageFile != null && imageFile.Length > 0)
            cat.ImageUrl = await SaveImage(imageFile, id);

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

    private async Task<string> SaveImage(IFormFile file, string categoryId)
    {
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowed.Contains(ext)) return "";

        var dir = Path.Combine(_env.WebRootPath, "uploads", "categories");
        Directory.CreateDirectory(dir);

        foreach (var old in Directory.GetFiles(dir, $"{categoryId}.*"))
            System.IO.File.Delete(old);

        var filePath = Path.Combine(dir, $"{categoryId}{ext}");
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/categories/{categoryId}{ext}";
    }

    private static string Slugify(string text) =>
        System.Text.RegularExpressions.Regex.Replace(text.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-');
}
