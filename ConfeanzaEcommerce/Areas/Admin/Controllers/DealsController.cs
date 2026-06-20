using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class DealsController : Controller
{
    private readonly ApplicationDbContext _db;
    public DealsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var deals = await _db.Deals.Include(d => d.Store)
            .OrderByDescending(d => d.CreatedAt).ToListAsync();
        return View(deals);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Stores = await _db.Stores.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
        return View(new Deal { StartsAt = DateTime.Now });
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Deal model)
    {
        model.Id = Guid.NewGuid().ToString();
        model.Slug = Slugify(model.Title) + "-" + DateTime.UtcNow.Ticks.ToString()[^6..];
        model.CreatedAt = model.UpdatedAt = DateTime.UtcNow;
        _db.Deals.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Deal created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var deal = await _db.Deals.FindAsync(id);
        if (deal == null) return NotFound();
        ViewBag.Stores = await _db.Stores.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
        return View(deal);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Deal model)
    {
        var deal = await _db.Deals.FindAsync(id);
        if (deal == null) return NotFound();
        deal.Title = model.Title;
        deal.Description = model.Description;
        deal.DealType = model.DealType;
        deal.StoreId = string.IsNullOrEmpty(model.StoreId) ? null : model.StoreId;
        deal.DiscountPercent = model.DiscountPercent;
        deal.BannerImage = model.BannerImage;
        deal.IsActive = model.IsActive;
        deal.IsFeatured = model.IsFeatured;
        deal.StartsAt = model.StartsAt;
        deal.ExpiresAt = model.ExpiresAt;
        deal.AffiliateUrl = model.AffiliateUrl;
        deal.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Deal updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var deal = await _db.Deals.FindAsync(id);
        if (deal != null) { _db.Deals.Remove(deal); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Deal deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static string Slugify(string text) =>
        System.Text.RegularExpressions.Regex.Replace(text.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-');
}
