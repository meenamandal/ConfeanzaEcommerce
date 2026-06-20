using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeanzaEcommerce.Models.Entities;

[Table("coupons")]
public class Coupon
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("store_id")] public string? StoreId { get; set; }
    [Column("CODE")] [Required] [MaxLength(100)] public string Code { get; set; } = "";
    [Column("title")] [Required] [MaxLength(255)] public string Title { get; set; } = "";
    [Column("DESCRIPTION")] public string? Description { get; set; }
    [Column("TYPE")] [Required] [MaxLength(30)] public string Type { get; set; } = "";
    [Column("discount_value")] public decimal DiscountValue { get; set; }
    [Column("min_order_value")] public decimal? MinOrderValue { get; set; }
    [Column("max_discount")] public decimal? MaxDiscount { get; set; }
    [Column("is_active")] public bool IsActive { get; set; } = true;
    [Column("is_verified")] public bool IsVerified { get; set; } = false;
    [Column("starts_at")] public DateTime? StartsAt { get; set; }
    [Column("expires_at")] public DateTime? ExpiresAt { get; set; }
    [Column("affiliate_url")] public string? AffiliateUrl { get; set; }
    [Column("used_count")] public int UsedCount { get; set; } = 0;
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("StoreId")] public Store? Store { get; set; }
}
