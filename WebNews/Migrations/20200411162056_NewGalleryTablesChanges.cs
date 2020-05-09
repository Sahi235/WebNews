using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebNews.Migrations
{
    public partial class NewGalleryTablesChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsModified",
                table: "Galleries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Galleries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifierId",
                table: "Galleries",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedDate",
                table: "Galleries",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SeoUrl",
                table: "Galleries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Galleries",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GalleryTag",
                columns: table => new
                {
                    GalleryId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryTag", x => new { x.GalleryId, x.TagId });
                    table.ForeignKey(
                        name: "FK_GalleryTag_Galleries_GalleryId",
                        column: x => x.GalleryId,
                        principalTable: "Galleries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GalleryTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_UserId1",
                table: "Galleries",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryTag_TagId",
                table: "GalleryTag",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Galleries_AspNetUsers_UserId1",
                table: "Galleries",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Galleries_AspNetUsers_UserId1",
                table: "Galleries");

            migrationBuilder.DropTable(
                name: "GalleryTag");

            migrationBuilder.DropIndex(
                name: "IX_Galleries_UserId1",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "IsModified",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "ModifierId",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "SeoUrl",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Galleries");
        }
    }
}
