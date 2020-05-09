using Microsoft.EntityFrameworkCore.Migrations;

namespace WebNews.Migrations
{
    public partial class NewViewCountGallery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Galleries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GalleryId",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_GalleryId",
                table: "Comments",
                column: "GalleryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Galleries_GalleryId",
                table: "Comments",
                column: "GalleryId",
                principalTable: "Galleries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Galleries_GalleryId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_GalleryId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "GalleryId",
                table: "Comments");
        }
    }
}
