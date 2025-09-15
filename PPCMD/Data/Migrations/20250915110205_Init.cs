using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPCMD.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    License = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    NTN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    STN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProprietorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CNIC = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Landline1 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Landline2 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CNIC",
                table: "Companies",
                column: "CNIC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CompanyName",
                table: "Companies",
                column: "CompanyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Email",
                table: "Companies",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_License",
                table: "Companies",
                column: "License",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_NTN",
                table: "Companies",
                column: "NTN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_STN",
                table: "Companies",
                column: "STN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Website",
                table: "Companies",
                column: "Website",
                unique: true,
                filter: "[Website] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AspNetUsers");
        }
    }
}
