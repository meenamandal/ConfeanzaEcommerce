using Microsoft.EntityFrameworkCore;
using ConfeanzaEcommerce.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Fix ENUM columns → VARCHAR so all string values are accepted
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var fixes = new[]
    {
        "ALTER TABLE `products`       MODIFY COLUMN `STATUS`     VARCHAR(30) NOT NULL DEFAULT 'draft'",
        "ALTER TABLE `blogs`          MODIFY COLUMN `STATUS`     VARCHAR(20) NOT NULL DEFAULT 'draft'",
        "ALTER TABLE `coupons`        MODIFY COLUMN `TYPE`       VARCHAR(30) NOT NULL DEFAULT 'percentage'",
        "ALTER TABLE `deals`          MODIFY COLUMN `deal_type`  VARCHAR(30) NOT NULL DEFAULT 'daily_deal'",
        "ALTER TABLE `reviews`        MODIFY COLUMN `STATUS`     VARCHAR(20) NOT NULL DEFAULT 'pending'",
        "ALTER TABLE `affiliate_links` MODIFY COLUMN `currency`  VARCHAR(10) NOT NULL DEFAULT 'INR'",
        "ALTER TABLE `blogs`          DROP FOREIGN KEY `fk_blog_author`",
        "ALTER TABLE `blogs`          MODIFY COLUMN `author_id`  VARCHAR(100) NULL",
    };
    foreach (var sql in fixes)
    {
        try { await db.Database.ExecuteSqlRawAsync(sql); }
        catch { /* column already VARCHAR or table not yet created */ }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
