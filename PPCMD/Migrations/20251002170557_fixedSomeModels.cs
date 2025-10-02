using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPCMD.Migrations
{
    /// <inheritdoc />
    public partial class fixedSomeModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ItemID",
                table: "Items",
                newName: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PendingBLs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LCs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LCs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IGMs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IGMs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "HCs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "HCs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DutyTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DutyCharges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DutyCharges",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BLs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BLs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PendingBLs_ClientId",
                table: "PendingBLs",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingBLs_Clients_ClientId",
                table: "PendingBLs",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingBLs_Clients_ClientId",
                table: "PendingBLs");

            migrationBuilder.DropIndex(
                name: "IX_PendingBLs_ClientId",
                table: "PendingBLs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PendingBLs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IGMs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IGMs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "HCs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "HCs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DutyCharges");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DutyCharges");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BLs");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Items",
                newName: "ItemID");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DutyTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
