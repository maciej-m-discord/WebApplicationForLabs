using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Added_Notifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Reports",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Posts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Hashtags",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Follows",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Comments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Bookmarks",
                newName: "CreatedAt");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Reports",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Posts",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Hashtags",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Follows",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Comments",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Bookmarks",
                newName: "DateCreated");
        }
    }
}
