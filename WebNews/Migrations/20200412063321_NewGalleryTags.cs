using Microsoft.EntityFrameworkCore.Migrations;

namespace WebNews.Migrations
{
    public partial class NewGalleryTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryTag_Galleries_GalleryId",
                table: "GalleryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_GalleryTag_Tags_TagId",
                table: "GalleryTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GalleryTag",
                table: "GalleryTag");

            migrationBuilder.RenameTable(
                name: "GalleryTag",
                newName: "GalleryTags");

            migrationBuilder.RenameIndex(
                name: "IX_GalleryTag_TagId",
                table: "GalleryTags",
                newName: "IX_GalleryTags_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GalleryTags",
                table: "GalleryTags",
                columns: new[] { "GalleryId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryTags_Galleries_GalleryId",
                table: "GalleryTags",
                column: "GalleryId",
                principalTable: "Galleries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryTags_Tags_TagId",
                table: "GalleryTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryTags_Galleries_GalleryId",
                table: "GalleryTags");

            migrationBuilder.DropForeignKey(
                name: "FK_GalleryTags_Tags_TagId",
                table: "GalleryTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GalleryTags",
                table: "GalleryTags");

            migrationBuilder.RenameTable(
                name: "GalleryTags",
                newName: "GalleryTag");

            migrationBuilder.RenameIndex(
                name: "IX_GalleryTags_TagId",
                table: "GalleryTag",
                newName: "IX_GalleryTag_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GalleryTag",
                table: "GalleryTag",
                columns: new[] { "GalleryId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryTag_Galleries_GalleryId",
                table: "GalleryTag",
                column: "GalleryId",
                principalTable: "Galleries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryTag_Tags_TagId",
                table: "GalleryTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
