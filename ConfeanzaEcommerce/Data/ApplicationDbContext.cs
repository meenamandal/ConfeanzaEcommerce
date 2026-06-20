using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Models.Entities;

namespace ConfeanzaEcommerce.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Subcategory> Subcategories => Set<Subcategory>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductSpecification> ProductSpecifications => Set<ProductSpecification>();
    public DbSet<AffiliateLink> AffiliateLinks => Set<AffiliateLink>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<Deal> Deals => Set<Deal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Slug).IsUnique();

        modelBuilder.Entity<Brand>()
            .HasIndex(b => b.Slug).IsUnique();

        modelBuilder.Entity<Store>()
            .HasIndex(s => s.Slug).IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Slug).IsUnique();

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AffiliateLink>()
            .HasOne(a => a.Store)
            .WithMany(s => s.AffiliateLinks)
            .HasForeignKey(a => a.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Coupon>()
            .HasOne(c => c.Store)
            .WithMany(s => s.Coupons)
            .HasForeignKey(c => c.StoreId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
