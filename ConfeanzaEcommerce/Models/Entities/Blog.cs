using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeanzaEcommerce.Models.Entities;

[Table("blogs")]
public class Blog
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("title")] [Required] [MaxLength(500)] public string Title { get; set; } = "";
    [Column("slug")] [Required] [MaxLength(500)] public string Slug { get; set; } = "";
    [Column("excerpt")] public string? Excerpt { get; set; }
    [Column("content")] [Required] public string Content { get; set; } = "";
    [Column("cover_image")] public string? CoverImage { get; set; }
    [Column("author_id")] public string? AuthorId { get; set; }
    [Column("STATUS")] [MaxLength(20)] public string Status { get; set; } = "draft";
    [Column("is_featured")] public bool IsFeatured { get; set; } = false;
    [Column("category")] public string? Category { get; set; }
    [Column("read_time")] public int? ReadTime { get; set; }
    [Column("view_count")] public int ViewCount { get; set; } = 0;
    [Column("meta_title")] public string? MetaTitle { get; set; }
    [Column("meta_description")] public string? MetaDescription { get; set; }
    [Column("published_at")] public DateTime? PublishedAt { get; set; }
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
