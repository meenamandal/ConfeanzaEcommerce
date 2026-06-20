using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class StoresController : Controller
{
    private readonly ApplicationDbContext _db;
    public StoresController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var stores = await _db.Stores.OrderBy(s => s.SortOrder).ThenBy(s => s.Name).ToListAsync();
        return View(stores);
    }

    public IActionResult Create() => View(new Store());

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Store model)
    {
        model.Id = Guid.NewGuid().ToString();
        model.Slug = Slugify(model.Name);
        model.CreatedAt = model.UpdatedAt = DateTime.UtcNow;
        _db.Stores.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Store created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var store = await _db.Stores.FindAsync(id);
        if (store == null) return NotFound();
        return View(store);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Store model)
    {
        var store = await _db.Stores.FindAsync(id);
        if (store == null) return NotFound();
        store.Name = model.Name;
        store.Description = model.Description;
        store.LogoUrl = model.LogoUrl;
        store.WebsiteUrl = model.WebsiteUrl;
        store.AffiliateNetwork = model.AffiliateNetwork;
        store.TrackingId = model.TrackingId;
        store.CommissionRate = model.CommissionRate;
        store.CookieDuration = model.CookieDuration;
        store.IsActive = model.IsActive;
        store.IsFeatured = model.IsFeatured;
        store.SortOrder = model.SortOrder;
        store.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Store updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var store = await _db.Stores.FindAsync(id);
        if (store != null) { _db.Stores.Remove(store); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Store deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static string Slugify(string text) =>
        System.Text.RegularExpressions.Regex.Replace(text.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-');
}
