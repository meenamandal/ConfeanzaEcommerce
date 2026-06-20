using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class AffiliateLinksController : Controller
{
    private readonly ApplicationDbContext _db;
    public AffiliateLinksController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string productId)
    {
        var product = await _db.Products.FindAsync(productId);
        if (product == null) return NotFound();
        var links = await _db.AffiliateLinks.Include(a => a.Store)
            .Where(a => a.ProductId == productId).ToListAsync();
        ViewBag.Product = product;
        return View(links);
    }

    public async Task<IActionResult> Create(string productId)
    {
        var product = await _db.Products.FindAsync(productId);
        if (product == null) return NotFound();
        ViewBag.Product = product;
        ViewBag.Stores = await _db.Stores.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
        return View(new AffiliateLink { ProductId = productId, Currency = "INR" });
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AffiliateLink model)
    {
        model.Id = Guid.NewGuid().ToString();
        model.CreatedAt = model.UpdatedAt = DateTime.UtcNow;
        _db.AffiliateLinks.Add(model);
        await UpdateProductPriceRange(model.ProductId);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Price link added.";
        return RedirectToAction(nameof(Index), new { productId = model.ProductId });
    }

    public async Task<IActionResult> Edit(string id)
    {
        var link = await _db.AffiliateLinks.Include(a => a.Product).FirstOrDefaultAsync(a => a.Id == id);
        if (link == null) return NotFound();
        ViewBag.Product = link.Product;
        ViewBag.Stores = await _db.Stores.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
        return View(link);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, AffiliateLink model)
    {
        var link = await _db.AffiliateLinks.FindAsync(id);
        if (link == null) return NotFound();
        link.StoreId = model.StoreId;
        link.AffiliateUrl = model.AffiliateUrl;
        link.OriginalUrl = model.OriginalUrl;
        link.Price = model.Price;
        link.Mrp = model.Mrp;
        link.DiscountPercent = model.DiscountPercent;
        link.Currency = model.Currency;
        link.InStock = model.InStock;
        link.CommissionRate = model.CommissionRate;
        link.IsActive = model.IsActive;
        link.UpdatedAt = DateTime.UtcNow;
        await UpdateProductPriceRange(link.ProductId);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Price link updated.";
        return RedirectToAction(nameof(Index), new { productId = link.ProductId });
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var link = await _db.AffiliateLinks.FindAsync(id);
        if (link != null)
        {
            var productId = link.ProductId;
            _db.AffiliateLinks.Remove(link);
            await UpdateProductPriceRange(productId);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Price link deleted.";
            return RedirectToAction(nameof(Index), new { productId });
        }
        return RedirectToAction(nameof(Index), "Products");
    }

    private async Task UpdateProductPriceRange(string productId)
    {
        var product = await _db.Products.FindAsync(productId);
        if (product == null) return;
        var prices = await _db.AffiliateLinks
            .Where(a => a.ProductId == productId && a.IsActive && a.Price.HasValue)
            .Select(a => a.Price!.Value).ToListAsync();
        product.LowestPrice = prices.Any() ? prices.Min() : null;
        product.HighestPrice = prices.Any() ? prices.Max() : null;
        product.UpdatedAt = DateTime.UtcNow;
    }
}
