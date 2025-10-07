using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPCMD.Migrations
{
    /// <inheritdoc />
    public partial class modifyLCBL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LCs_Lolos_LoloId",
                table: "LCs");

            migrationBuilder.DropForeignKey(
                name: "FK_LCs_ShippingLines_ShippingLineId",
                table: "LCs");

            migrationBuilder.DropForeignKey(
                name: "FK_LCs_Terminals_TerminalId",
                table: "LCs");

            migrationBuilder.DropIndex(
                name: "IX_LCs_LoloId",
                table: "LCs");

            migrationBuilder.DropIndex(
                name: "IX_LCs_ShippingLineId",
                table: "LCs");

            migrationBuilder.DropIndex(
                name: "IX_LCs_TerminalId",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "AssessedValue",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "DeclaredValue",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "EntryType",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "LoloId",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "ShippingLineId",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "TerminalId",
                table: "LCs");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "LCs",
                newName: "TotalQuantity");

            migrationBuilder.AddColumn<decimal>(
                name: "AssessedValue",
                table: "BLs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BLQuantity",
                table: "BLs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeclaredValue",
                table: "BLs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "BLs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "LoloId",
                table: "BLs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingLineId",
                table: "BLs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TerminalId",
                table: "BLs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BLs_LoloId",
                table: "BLs",
                column: "LoloId");

            migrationBuilder.CreateIndex(
                name: "IX_BLs_ShippingLineId",
                table: "BLs",
                column: "ShippingLineId");

            migrationBuilder.CreateIndex(
                name: "IX_BLs_TerminalId",
                table: "BLs",
                column: "TerminalId");

            migrationBuilder.AddForeignKey(
                name: "FK_BLs_Lolos_LoloId",
                table: "BLs",
                column: "LoloId",
                principalTable: "Lolos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BLs_ShippingLines_ShippingLineId",
                table: "BLs",
                column: "ShippingLineId",
                principalTable: "ShippingLines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BLs_Terminals_TerminalId",
                table: "BLs",
                column: "TerminalId",
                principalTable: "Terminals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BLs_Lolos_LoloId",
                table: "BLs");

            migrationBuilder.DropForeignKey(
                name: "FK_BLs_ShippingLines_ShippingLineId",
                table: "BLs");

            migrationBuilder.DropForeignKey(
                name: "FK_BLs_Terminals_TerminalId",
                table: "BLs");

            migrationBuilder.DropIndex(
                name: "IX_BLs_LoloId",
                table: "BLs");

            migrationBuilder.DropIndex(
                name: "IX_BLs_ShippingLineId",
                table: "BLs");

            migrationBuilder.DropIndex(
                name: "IX_BLs_TerminalId",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "AssessedValue",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "BLQuantity",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "DeclaredValue",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "LoloId",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "ShippingLineId",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "TerminalId",
                table: "BLs");

            migrationBuilder.RenameColumn(
                name: "TotalQuantity",
                table: "LCs",
                newName: "Quantity");

            migrationBuilder.AddColumn<decimal>(
                name: "AssessedValue",
                table: "LCs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeclaredValue",
                table: "LCs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "EntryType",
                table: "LCs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "LCs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "LoloId",
                table: "LCs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingLineId",
                table: "LCs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TerminalId",
                table: "LCs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LCs_LoloId",
                table: "LCs",
                column: "LoloId");

            migrationBuilder.CreateIndex(
                name: "IX_LCs_ShippingLineId",
                table: "LCs",
                column: "ShippingLineId");

            migrationBuilder.CreateIndex(
                name: "IX_LCs_TerminalId",
                table: "LCs",
                column: "TerminalId");

            migrationBuilder.AddForeignKey(
                name: "FK_LCs_Lolos_LoloId",
                table: "LCs",
                column: "LoloId",
                principalTable: "Lolos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LCs_ShippingLines_ShippingLineId",
                table: "LCs",
                column: "ShippingLineId",
                principalTable: "ShippingLines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LCs_Terminals_TerminalId",
                table: "LCs",
                column: "TerminalId",
                principalTable: "Terminals",
                principalColumn: "Id");
        }
    }
}
