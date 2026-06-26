using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.ViewModels;
using ConfeanzaEcommerce.Services;

namespace ConfeanzaEcommerce.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly EmailService _email;

    public HomeController(ApplicationDbContext db, EmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new HomeViewModel
        {
            FeaturedCategories = await _db.Categories
                .Where(c => c.IsActive && c.IsFeatured)
                .OrderBy(c => c.SortOrder).Take(12).ToListAsync(),

            FeaturedProducts = await _db.Products
                .Include(p => p.Category).Include(p => p.Brand)
                .Include(p => p.Images).Include(p => p.AffiliateLinks)
                .Where(p => p.Status == "published" && p.IsFeatured && p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt).Take(12).ToListAsync(),

            TrendingProducts = await _db.Products
                .Include(p => p.Category).Include(p => p.Images).Include(p => p.AffiliateLinks)
                .Where(p => p.Status == "published" && p.IsTrending && p.DeletedAt == null)
                .OrderByDescending(p => p.ClickCount).Take(8).ToListAsync(),

            ActiveDeals = await _db.Deals.Include(d => d.Store)
                .Where(d => d.IsActive && (d.ExpiresAt == null || d.ExpiresAt > DateTime.UtcNow))
                .OrderByDescending(d => d.IsFeatured).Take(6).ToListAsync(),

            ActiveCoupons = await _db.Coupons.Include(c => c.Store)
                .Where(c => c.IsActive && (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow))
                .OrderByDescending(c => c.IsVerified).Take(6).ToListAsync(),

            LatestBlogs = await _db.Blogs
                .Where(b => b.Status == "published")
                .OrderByDescending(b => b.PublishedAt).Take(3).ToListAsync(),

            FeaturedStores = await _db.Stores
                .Where(s => s.IsActive && s.IsFeatured)
                .OrderBy(s => s.SortOrder).Take(8).ToListAsync(),

            FeaturedBrands = await _db.Brands
                .Where(b => b.IsActive && b.IsFeatured).Take(12).ToListAsync(),
        };

        return View(vm);
    }

    public IActionResult Privacy() => View();

    [Route("about-us")]
    public IActionResult AboutUs() => View();

    [Route("contact-us")]
    public IActionResult ContactUs() => View();

    [HttpPost("contact-us")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ContactUs(string name, string email, string subject, string message)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
        {
            TempData["ContactError"] = "Please fill in all fields before sending.";
            return View();
        }

        try
        {
            await _email.SendContactFormAsync(name, email, subject, message);
            TempData["ContactSuccess"] = "Your message has been sent! We'll reply within 1 business day.";
        }
        catch (Exception ex)
        {
            TempData["ContactError"] = $"Failed to send: {ex.Message}";
        }

        return RedirectToAction(nameof(ContactUs));
    }

    [Route("privacy-policy")]
    public IActionResult PrivacyPolicy() => View();

    [Route("terms-and-conditions")]
    public IActionResult TermsAndConditions() => View();

    [Route("error")]
    public IActionResult Error() => View();
}
