using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPCMD.Migrations
{
    /// <inheritdoc />
    public partial class modifiedLCBL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGeneral",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "LandingCharges",
                table: "LCs");

            migrationBuilder.DropColumn(
                name: "AssessedValue",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "BLDate",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "BLQuantity",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "DeclaredValue",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "FreightCharges",
                table: "BLItems");

            migrationBuilder.DropColumn(
                name: "ImportValue",
                table: "BLItems");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "PendingBLs",
                newName: "BLQuantity");

            migrationBuilder.RenameColumn(
                name: "InsuranceValue",
                table: "BLItems",
                newName: "DeclaredValue");

            migrationBuilder.AddColumn<decimal>(
                name: "AssessableValue",
                table: "BLItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AssessedValue",
                table: "BLItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InsurancePKR",
                table: "BLItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InsuranceUSD",
                table: "BLItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InvoiceValue",
                table: "BLItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InvoiceValuePKR",
                table: "BLItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LandingCharges",
                table: "BLItems",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessableValue",
                table: "BLItems");

            migrationBuilder.DropColumn(
                name: "AssessedValue",
                table: "BLItems");

            migrationBuilder.DropColumn(
                name: "InsurancePKR",
                table: "BLItems");

            migrationBuilder.DropColumn(
                name: "InsuranceUSD",
                table: "BLItems");

            migrationBuilder.DropColumn(
                name: "InvoiceValue",
                table: "BLItems");

            migrationBuilder.DropColumn(
                name: "InvoiceValuePKR",
                table: "BLItems");

            migrationBuilder.DropColumn(
                name: "LandingCharges",
                table: "BLItems");

            migrationBuilder.RenameColumn(
                name: "BLQuantity",
                table: "PendingBLs",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "DeclaredValue",
                table: "BLItems",
                newName: "InsuranceValue");

            migrationBuilder.AddColumn<bool>(
                name: "IsGeneral",
                table: "LCs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LandingCharges",
                table: "LCs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AssessedValue",
                table: "BLs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "BLDate",
                table: "BLs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                name: "FreightCharges",
                table: "BLItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ImportValue",
                table: "BLItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
