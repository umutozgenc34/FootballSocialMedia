using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballSocialMedia.Repository.Migrations
{
    /// <inheritdoc />
    public partial class PostEntityUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "Posts",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Posts",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }
    }
}
