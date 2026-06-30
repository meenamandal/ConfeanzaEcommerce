using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;

namespace ConfeanzaEcommerce.ViewComponents;

public class CategoriesDropdownViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _db;
    public CategoriesDropdownViewComponent(ApplicationDbContext db) => _db = db;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cats = await _db.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
            .Select(c => new { c.Name, c.Slug, c.ImageUrl, c.IconUrl })
            .ToListAsync();

        return View(cats.Select(c => new CategoryDropdownItem
        {
            Name = c.Name,
            Slug = c.Slug,
            ImageUrl = c.ImageUrl,
            IconUrl = c.IconUrl
        }).ToList());
    }
}

public class CategoryDropdownItem
{
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? ImageUrl { get; set; }
    public string? IconUrl { get; set; }
}
