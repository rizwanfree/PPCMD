using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPCMD.Migrations
{
    /// <inheritdoc />
    public partial class modifiedLCBL2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DutyCharges_BLs_BLId",
                table: "DutyCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_DutyCharges_BLs_BLId1",
                table: "DutyCharges");

            migrationBuilder.DropIndex(
                name: "IX_DutyCharges_BLId1",
                table: "DutyCharges");

            migrationBuilder.DropColumn(
                name: "BLId1",
                table: "DutyCharges");

            migrationBuilder.DropColumn(
                name: "ImportValue",
                table: "DutyCharges");

            migrationBuilder.DropColumn(
                name: "InsuranceValue",
                table: "DutyCharges");

            migrationBuilder.DropColumn(
                name: "LadingCharges",
                table: "DutyCharges");

            migrationBuilder.AlterColumn<int>(
                name: "BLId",
                table: "DutyCharges",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BLItemId",
                table: "DutyCharges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DutyCharges_BLItemId",
                table: "DutyCharges",
                column: "BLItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_DutyCharges_BLItems_BLItemId",
                table: "DutyCharges",
                column: "BLItemId",
                principalTable: "BLItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DutyCharges_BLs_BLId",
                table: "DutyCharges",
                column: "BLId",
                principalTable: "BLs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DutyCharges_BLItems_BLItemId",
                table: "DutyCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_DutyCharges_BLs_BLId",
                table: "DutyCharges");

            migrationBuilder.DropIndex(
                name: "IX_DutyCharges_BLItemId",
                table: "DutyCharges");

            migrationBuilder.DropColumn(
                name: "BLItemId",
                table: "DutyCharges");

            migrationBuilder.AlterColumn<int>(
                name: "BLId",
                table: "DutyCharges",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BLId1",
                table: "DutyCharges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ImportValue",
                table: "DutyCharges",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InsuranceValue",
                table: "DutyCharges",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LadingCharges",
                table: "DutyCharges",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_DutyCharges_BLId1",
                table: "DutyCharges",
                column: "BLId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DutyCharges_BLs_BLId",
                table: "DutyCharges",
                column: "BLId",
                principalTable: "BLs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DutyCharges_BLs_BLId1",
                table: "DutyCharges",
                column: "BLId1",
                principalTable: "BLs",
                principalColumn: "Id");
        }
    }
}
