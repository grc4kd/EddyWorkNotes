using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ui.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColType_WorkNote_Description : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "WorkNote");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WorkNote",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "WorkNote");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "WorkNote",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
