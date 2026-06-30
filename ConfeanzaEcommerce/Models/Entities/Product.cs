using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeanzaEcommerce.Models.Entities;

[Table("products")]
public class Product
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("NAME")] [Required] public string Name { get; set; } = "";
    [Column("slug")] [Required] public string Slug { get; set; } = "";
    [Column("DESCRIPTION")] public string? Description { get; set; }
    [Column("short_description")] public string? ShortDescription { get; set; }
    [Column("category_id")] public string? CategoryId { get; set; }
    [Column("subcategory_id")] public string? SubcategoryId { get; set; }
    [Column("brand_id")] public string? BrandId { get; set; }
    [Column("sku")] public string? Sku { get; set; }
    [Column("STATUS")] [MaxLength(30)] public string Status { get; set; } = "draft";
    [Column("is_featured")] public bool IsFeatured { get; set; } = false;
    [Column("is_trending")] public bool IsTrending { get; set; } = false;
    [Column("is_deal_of_day")] public bool IsDealOfDay { get; set; } = false;
    [Column("show_visit_store")] public bool ShowVisitStore { get; set; } = false;
    [Column("lowest_price")] public decimal? LowestPrice { get; set; }
    [Column("highest_price")] public decimal? HighestPrice { get; set; }
    [Column("avg_rating")] public decimal AvgRating { get; set; } = 0;
    [Column("review_count")] public int ReviewCount { get; set; } = 0;
    [Column("click_count")] public int ClickCount { get; set; } = 0;
    [Column("view_count")] public int ViewCount { get; set; } = 0;
    [Column("wishlist_count")] public int WishlistCount { get; set; } = 0;
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [Column("deleted_at")] public DateTime? DeletedAt { get; set; }

    [ForeignKey("CategoryId")] public Category? Category { get; set; }
    [ForeignKey("BrandId")] public Brand? Brand { get; set; }
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<AffiliateLink> AffiliateLinks { get; set; } = new List<AffiliateLink>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
}

[Table("product_images")]
public class ProductImage
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("product_id")] [Required] public string ProductId { get; set; } = "";
    [Column("url")] [Required] public string Url { get; set; } = "";
    [Column("alt_text")] public string? AltText { get; set; }
    [Column("sort_order")] public int SortOrder { get; set; } = 0;
    [Column("is_primary")] public bool IsPrimary { get; set; } = false;
    [ForeignKey("ProductId")] public Product? Product { get; set; }
}

[Table("product_specifications")]
public class ProductSpecification
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("product_id")] [Required] public string ProductId { get; set; } = "";
    [Column("group_name")] [Required] [MaxLength(100)] public string GroupName { get; set; } = "";
    [Column("spec_key")] [Required] [MaxLength(150)] public string SpecKey { get; set; } = "";
    [Column("spec_value")] [Required] public string SpecValue { get; set; } = "";
    [Column("unit")] public string? Unit { get; set; }
    [Column("sort_order")] public int SortOrder { get; set; } = 0;
    [ForeignKey("ProductId")] public Product? Product { get; set; }
}
