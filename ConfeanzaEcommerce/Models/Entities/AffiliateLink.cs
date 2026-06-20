using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeanzaEcommerce.Models.Entities;

[Table("affiliate_links")]
public class AffiliateLink
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("product_id")] [Required] public string ProductId { get; set; } = "";
    [Column("store_id")] [Required] public string StoreId { get; set; } = "";
    [Column("affiliate_url")] [Required] public string AffiliateUrl { get; set; } = "";
    [Column("original_url")] public string? OriginalUrl { get; set; }
    [Column("price")] public decimal? Price { get; set; }
    [Column("currency")] [MaxLength(10)] public string Currency { get; set; } = "INR";
    [Column("in_stock")] public bool InStock { get; set; } = true;
    [Column("commission_rate")] public decimal? CommissionRate { get; set; }
    [Column("discount_percent")] public decimal? DiscountPercent { get; set; }
    [Column("mrp")] public decimal? Mrp { get; set; }
    [Column("is_active")] public bool IsActive { get; set; } = true;
    [Column("last_checked_at")] public DateTime? LastCheckedAt { get; set; }
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ProductId")] public Product? Product { get; set; }
    [ForeignKey("StoreId")] public Store? Store { get; set; }
}
