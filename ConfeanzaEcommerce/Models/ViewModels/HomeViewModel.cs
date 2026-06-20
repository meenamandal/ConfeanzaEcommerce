using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Models.ViewModels;

public class HomeViewModel
{
    public List<Category> FeaturedCategories { get; set; } = new();
    public List<Product> FeaturedProducts { get; set; } = new();
    public List<Product> TrendingProducts { get; set; } = new();
    public List<Deal> ActiveDeals { get; set; } = new();
    public List<Coupon> ActiveCoupons { get; set; } = new();
    public List<Blog> LatestBlogs { get; set; } = new();
    public List<Store> FeaturedStores { get; set; } = new();
    public List<Brand> FeaturedBrands { get; set; } = new();
}

public class ProductListViewModel
{
    public List<Product> Products { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Brand> Brands { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string? CategorySlug { get; set; }
    public string? BrandSlug { get; set; }
    public string SortBy { get; set; } = "newest";
    public int Page { get; set; } = 1;
    public int TotalCount { get; set; }
    public int PageSize { get; set; } = 24;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class ProductDetailViewModel
{
    public Product Product { get; set; } = null!;
    public List<AffiliateLink> AffiliateLinks { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
    public List<Product> RelatedProducts { get; set; } = new();
    public AffiliateLink? BestPrice => AffiliateLinks.Where(a => a.IsActive && a.InStock).OrderBy(a => a.Price).FirstOrDefault();
}
