using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeanzaEcommerce.Models.Entities;

[Table("stores")]
public class Store
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("NAME")] [Required] [MaxLength(150)] public string Name { get; set; } = "";
    [Column("slug")] [Required] [MaxLength(150)] public string Slug { get; set; } = "";
    [Column("DESCRIPTION")] public string? Description { get; set; }
    [Column("logo_url")] public string? LogoUrl { get; set; }
    [Column("website_url")] [Required] public string WebsiteUrl { get; set; } = "";
    [Column("affiliate_network")] [Required] [MaxLength(50)] public string AffiliateNetwork { get; set; } = "";
    [Column("tracking_id")] public string? TrackingId { get; set; }
    [Column("commission_rate")] public decimal? CommissionRate { get; set; }
    [Column("cookie_duration")] public int? CookieDuration { get; set; }
    [Column("is_active")] public bool IsActive { get; set; } = true;
    [Column("is_featured")] public bool IsFeatured { get; set; } = false;
    [Column("sort_order")] public int SortOrder { get; set; } = 0;
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AffiliateLink> AffiliateLinks { get; set; } = new List<AffiliateLink>();
    public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}
