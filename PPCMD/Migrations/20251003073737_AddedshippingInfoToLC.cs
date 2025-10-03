using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPCMD.Migrations
{
    /// <inheritdoc />
    public partial class AddedshippingInfoToLC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "LoloId",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "ShippingLineId",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "TerminalId",
                table: "LCs");
        }
    }
}
