using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeanzaEcommerce.Models.Entities;

[Table("brands")]
public class Brand
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("NAME")] [Required] [MaxLength(150)] public string Name { get; set; } = "";
    [Column("slug")] [Required] [MaxLength(150)] public string Slug { get; set; } = "";
    [Column("DESCRIPTION")] public string? Description { get; set; }
    [Column("logo_url")] public string? LogoUrl { get; set; }
    [Column("website_url")] public string? WebsiteUrl { get; set; }
    [Column("country")] public string? Country { get; set; }
    [Column("is_active")] public bool IsActive { get; set; } = true;
    [Column("is_featured")] public bool IsFeatured { get; set; } = false;
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
