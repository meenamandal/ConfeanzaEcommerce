using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeanzaEcommerce.Models.Entities;

[Table("site_settings")]
public class SiteSettings
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_page_notice")]
    public string? ProductPageNotice { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
