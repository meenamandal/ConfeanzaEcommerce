using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeanzaEcommerce.Models.Entities;

[Table("reviews")]
public class Review
{
    [Key] [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("product_id")] [Required] public string ProductId { get; set; } = "";
    [Column("user_id")] [Required] public string UserId { get; set; } = "";
    [Column("rating")] public byte Rating { get; set; }
    [Column("title")] public string? Title { get; set; }
    [Column("BODY")] public string? Body { get; set; }
    [Column("is_verified")] public bool IsVerified { get; set; } = false;
    [Column("STATUS")] [MaxLength(20)] public string Status { get; set; } = "pending";
    [Column("helpful_count")] public int HelpfulCount { get; set; } = 0;
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ProductId")] public Product? Product { get; set; }
}
