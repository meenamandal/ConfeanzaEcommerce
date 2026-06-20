using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeanzaEcommerce.Models.Entities;

[Table("deals")]
public class Deal
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("title")] [Required] [MaxLength(500)] public string Title { get; set; } = "";
    [Column("slug")] [Required] [MaxLength(500)] public string Slug { get; set; } = "";
    [Column("DESCRIPTION")] public string? Description { get; set; }
    [Column("deal_type")] [MaxLength(30)] public string DealType { get; set; } = "daily_deal";
    [Column("store_id")] public string? StoreId { get; set; }
    [Column("discount_percent")] public decimal? DiscountPercent { get; set; }
    [Column("banner_image")] public string? BannerImage { get; set; }
    [Column("is_active")] public bool IsActive { get; set; } = true;
    [Column("is_featured")] public bool IsFeatured { get; set; } = false;
    [Column("starts_at")] public DateTime StartsAt { get; set; } = DateTime.UtcNow;
    [Column("expires_at")] public DateTime? ExpiresAt { get; set; }
    [Column("affiliate_url")] public string? AffiliateUrl { get; set; }
    [Column("click_count")] public int ClickCount { get; set; } = 0;
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("StoreId")] public Store? Store { get; set; }
}
