using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfeanzaEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddProductShowVisitStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "show_visit_store",
                table: "products",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "show_visit_store",
                table: "products");
        }
    }
}
