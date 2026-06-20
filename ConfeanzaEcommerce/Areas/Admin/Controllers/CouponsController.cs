using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class CouponsController : Controller
{
    private readonly ApplicationDbContext _db;
    public CouponsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var coupons = await _db.Coupons.Include(c => c.Store)
            .OrderByDescending(c => c.CreatedAt).ToListAsync();
        return View(coupons);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Stores = await _db.Stores.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
        return View(new Coupon());
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Coupon model)
    {
        model.Id = Guid.NewGuid().ToString();
        model.Code = model.Code.Trim().ToUpper();
        model.CreatedAt = model.UpdatedAt = DateTime.UtcNow;
        _db.Coupons.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Coupon created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon == null) return NotFound();
        ViewBag.Stores = await _db.Stores.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
        return View(coupon);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Coupon model)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon == null) return NotFound();
        coupon.StoreId = string.IsNullOrEmpty(model.StoreId) ? null : model.StoreId;
        coupon.Code = model.Code.Trim().ToUpper();
        coupon.Title = model.Title;
        coupon.Description = model.Description;
        coupon.Type = model.Type;
        coupon.DiscountValue = model.DiscountValue;
        coupon.MinOrderValue = model.MinOrderValue;
        coupon.MaxDiscount = model.MaxDiscount;
        coupon.IsActive = model.IsActive;
        coupon.IsVerified = model.IsVerified;
        coupon.StartsAt = model.StartsAt;
        coupon.ExpiresAt = model.ExpiresAt;
        coupon.AffiliateUrl = model.AffiliateUrl;
        coupon.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Coupon updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon != null) { _db.Coupons.Remove(coupon); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Coupon deleted.";
        return RedirectToAction(nameof(Index));
    }
}
