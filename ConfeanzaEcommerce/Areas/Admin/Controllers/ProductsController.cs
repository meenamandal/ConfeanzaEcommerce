using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ProductsController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

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

    public async Task<IActionResult> Details(string id)
    {
        var product = await _db.Products
            .Include(p => p.Category).Include(p => p.Brand)
            .Include(p => p.Images).Include(p => p.AffiliateLinks).ThenInclude(a => a.Store)
            .Include(p => p.Specifications).Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);
        if (product == null) return NotFound();
        return View(product);
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
        product.ShowVisitStore = model.ShowVisitStore;
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

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadImage(string id, IList<IFormFile>? imageFiles, string? altText)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();

        if (imageFiles == null || imageFiles.Count == 0)
        {
            TempData["Error"] = "Please select at least one image.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "products", id);
        Directory.CreateDirectory(uploadDir);

        var sortOrder = await _db.ProductImages.CountAsync(i => i.ProductId == id);
        var hasImages = sortOrder > 0;
        int saved = 0, skipped = 0;

        foreach (var file in imageFiles)
        {
            if (file.Length == 0) { skipped++; continue; }

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowed.Contains(ext)) { skipped++; continue; }

            if (file.Length > 5 * 1024 * 1024) { skipped++; continue; }

            var fileName = $"{Guid.NewGuid()}{ext}";
            var diskPath = Path.Combine(uploadDir, fileName);
            await using (var stream = new FileStream(diskPath, FileMode.Create))
                await file.CopyToAsync(stream);

            _db.ProductImages.Add(new ProductImage
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = id,
                Url = $"/uploads/products/{id}/{fileName}",
                AltText = string.IsNullOrWhiteSpace(altText) ? product.Name : altText,
                SortOrder = sortOrder++,
                IsPrimary = !hasImages && saved == 0   // first uploaded image becomes primary if none existed
            });
            saved++;
        }

        if (saved > 0) await _db.SaveChangesAsync();

        TempData["Success"] = saved == 1
            ? "1 image uploaded successfully."
            : $"{saved} images uploaded successfully." +
              (skipped > 0 ? $" ({skipped} skipped — invalid type or too large)" : "");

        if (saved == 0)
            TempData["Error"] = "No valid images found. Use JPG, PNG, WebP or GIF under 5 MB.";

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(string imageId, string productId)
    {
        var image = await _db.ProductImages.FindAsync(imageId);
        if (image == null) return NotFound();

        var diskPath = Path.Combine(_env.WebRootPath,
            image.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(diskPath))
            System.IO.File.Delete(diskPath);

        bool wasPrimary = image.IsPrimary;
        _db.ProductImages.Remove(image);
        await _db.SaveChangesAsync();

        if (wasPrimary)
        {
            var next = await _db.ProductImages.FirstOrDefaultAsync(i => i.ProductId == productId);
            if (next != null) { next.IsPrimary = true; await _db.SaveChangesAsync(); }
        }

        TempData["Success"] = "Image deleted.";
        return RedirectToAction(nameof(Details), new { id = productId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPrimaryImage(string imageId, string productId)
    {
        await _db.ProductImages
            .Where(i => i.ProductId == productId)
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.IsPrimary, false));

        var image = await _db.ProductImages.FindAsync(imageId);
        if (image != null) { image.IsPrimary = true; await _db.SaveChangesAsync(); }

        TempData["Success"] = "Primary image updated.";
        return RedirectToAction(nameof(Details), new { id = productId });
    }

    private static string Slugify(string text) =>
        System.Text.RegularExpressions.Regex.Replace(text.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-');
}
