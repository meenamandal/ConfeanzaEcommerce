using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class BlogsController : Controller
{
    private readonly ApplicationDbContext _db;
    public BlogsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var blogs = await _db.Blogs.OrderByDescending(b => b.CreatedAt).ToListAsync();
        return View(blogs);
    }

    public IActionResult Create() => View(new Blog());

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Blog model)
    {
        model.Id = Guid.NewGuid().ToString();
        model.Slug = Slugify(model.Title) + "-" + DateTime.UtcNow.Ticks.ToString()[^6..];
        model.AuthorId = "admin";
        model.CreatedAt = model.UpdatedAt = DateTime.UtcNow;
        if (model.Status == "published" && !model.PublishedAt.HasValue)
            model.PublishedAt = DateTime.UtcNow;
        _db.Blogs.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Blog post created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var blog = await _db.Blogs.FindAsync(id);
        if (blog == null) return NotFound();
        return View(blog);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Blog model)
    {
        var blog = await _db.Blogs.FindAsync(id);
        if (blog == null) return NotFound();
        blog.Title = model.Title;
        blog.Excerpt = model.Excerpt;
        blog.Content = model.Content;
        blog.CoverImage = model.CoverImage;
        blog.Status = model.Status;
        blog.IsFeatured = model.IsFeatured;
        blog.Category = model.Category;
        blog.ReadTime = model.ReadTime;
        blog.MetaTitle = model.MetaTitle;
        blog.MetaDescription = model.MetaDescription;
        blog.UpdatedAt = DateTime.UtcNow;
        if (model.Status == "published" && !blog.PublishedAt.HasValue)
            blog.PublishedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Blog post updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var blog = await _db.Blogs.FindAsync(id);
        if (blog != null) { _db.Blogs.Remove(blog); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Blog post deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static string Slugify(string text) =>
        System.Text.RegularExpressions.Regex.Replace(text.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-');
}
