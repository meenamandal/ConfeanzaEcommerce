using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeanzaEcommerce.Models.Entities;

[Table("categories")]
public class Category
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("NAME")] [Required] [MaxLength(150)] public string Name { get; set; } = "";
    [Column("slug")] [Required] [MaxLength(150)] public string Slug { get; set; } = "";
    [Column("DESCRIPTION")] public string? Description { get; set; }
    [Column("icon_url")] public string? IconUrl { get; set; }
    [Column("image_url")] public string? ImageUrl { get; set; }
    [Column("parent_id")] public string? ParentId { get; set; }
    [Column("sort_order")] public int SortOrder { get; set; } = 0;
    [Column("is_active")] public bool IsActive { get; set; } = true;
    [Column("is_featured")] public bool IsFeatured { get; set; } = false;
    [Column("meta_title")] public string? MetaTitle { get; set; }
    [Column("meta_description")] public string? MetaDescription { get; set; }
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}

[Table("subcategories")]
public class Subcategory
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("category_id")] [Required] public string CategoryId { get; set; } = "";
    [Column("NAME")] [Required] [MaxLength(150)] public string Name { get; set; } = "";
    [Column("slug")] [Required] [MaxLength(150)] public string Slug { get; set; } = "";
    [Column("DESCRIPTION")] public string? Description { get; set; }
    [Column("sort_order")] public int SortOrder { get; set; } = 0;
    [Column("is_active")] public bool IsActive { get; set; } = true;
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("CategoryId")] public Category? Category { get; set; }
}
