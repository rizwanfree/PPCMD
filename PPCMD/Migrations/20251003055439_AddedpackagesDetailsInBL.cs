using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPCMD.Migrations
{
    /// <inheritdoc />
    public partial class AddedpackagesDetailsInBL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Containers",
                table: "BLs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Packages",
                table: "BLs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "BLs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Containers",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "Packages",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "BLs");
        }
    }
}
