using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPCMD.Migrations
{
    /// <inheritdoc />
    public partial class MovedDutyToItemDuty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPercentage",
                table: "DutyTypes");

            migrationBuilder.AddColumn<bool>(
                name: "IsPercentage",
                table: "ItemDuties",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPercentage",
                table: "ItemDuties");

            migrationBuilder.AddColumn<bool>(
                name: "IsPercentage",
                table: "DutyTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
