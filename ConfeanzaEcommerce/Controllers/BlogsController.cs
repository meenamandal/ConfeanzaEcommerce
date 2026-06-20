using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;

namespace ConfeanzaEcommerce.Controllers;

public class BlogsController : Controller
{
    private readonly ApplicationDbContext _db;

    public BlogsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? category, int page = 1)
    {
        const int pageSize = 9;
        var query = _db.Blogs.Where(b => b.Status == "published");

        if (!string.IsNullOrEmpty(category))
            query = query.Where(b => b.Category == category);

        var total = await query.CountAsync();
        var blogs = await query.OrderByDescending(b => b.PublishedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewBag.Total = total;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Category = category;
        return View(blogs);
    }

    public async Task<IActionResult> Details(string slug)
    {
        var blog = await _db.Blogs.FirstOrDefaultAsync(b => b.Slug == slug && b.Status == "published");
        if (blog == null) return NotFound();

        blog.ViewCount++;
        await _db.SaveChangesAsync();

        var recent = await _db.Blogs
            .Where(b => b.Status == "published" && b.Id != blog.Id)
            .OrderByDescending(b => b.PublishedAt).Take(3).ToListAsync();

        ViewBag.RecentBlogs = recent;
        return View(blog);
    }
}
