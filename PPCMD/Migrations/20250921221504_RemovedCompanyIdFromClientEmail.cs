using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPCMD.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCompanyIdFromClientEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientEmails_Companies_CompanyId",
                table: "ClientEmails");

            migrationBuilder.DropIndex(
                name: "IX_ClientEmails_CompanyId",
                table: "ClientEmails");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ClientEmails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "ClientEmails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ClientEmails_CompanyId",
                table: "ClientEmails",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientEmails_Companies_CompanyId",
                table: "ClientEmails",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
