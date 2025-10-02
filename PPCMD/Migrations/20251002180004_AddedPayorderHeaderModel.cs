using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPCMD.Migrations
{
    /// <inheritdoc />
    public partial class AddedPayorderHeaderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payorder_BLs_BLId",
                table: "Payorder");

            migrationBuilder.DropForeignKey(
                name: "FK_Payorder_Companies_CompanyId",
                table: "Payorder");

            migrationBuilder.DropForeignKey(
                name: "FK_Payorder_LCs_LCId",
                table: "Payorder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payorder",
                table: "Payorder");

            migrationBuilder.RenameTable(
                name: "Payorder",
                newName: "Payorders");

            migrationBuilder.RenameIndex(
                name: "IX_Payorder_LCId",
                table: "Payorders",
                newName: "IX_Payorders_LCId");

            migrationBuilder.RenameIndex(
                name: "IX_Payorder_CompanyId",
                table: "Payorders",
                newName: "IX_Payorders_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Payorder_BLId",
                table: "Payorders",
                newName: "IX_Payorders_BLId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payorders",
                table: "Payorders",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PayorderHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayorderHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayorderHeaders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayorderHeaders_CompanyId",
                table: "PayorderHeaders",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payorders_BLs_BLId",
                table: "Payorders",
                column: "BLId",
                principalTable: "BLs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payorders_Companies_CompanyId",
                table: "Payorders",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payorders_LCs_LCId",
                table: "Payorders",
                column: "LCId",
                principalTable: "LCs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payorders_BLs_BLId",
                table: "Payorders");

            migrationBuilder.DropForeignKey(
                name: "FK_Payorders_Companies_CompanyId",
                table: "Payorders");

            migrationBuilder.DropForeignKey(
                name: "FK_Payorders_LCs_LCId",
                table: "Payorders");

            migrationBuilder.DropTable(
                name: "PayorderHeaders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payorders",
                table: "Payorders");

            migrationBuilder.RenameTable(
                name: "Payorders",
                newName: "Payorder");

            migrationBuilder.RenameIndex(
                name: "IX_Payorders_LCId",
                table: "Payorder",
                newName: "IX_Payorder_LCId");

            migrationBuilder.RenameIndex(
                name: "IX_Payorders_CompanyId",
                table: "Payorder",
                newName: "IX_Payorder_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Payorders_BLId",
                table: "Payorder",
                newName: "IX_Payorder_BLId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payorder",
                table: "Payorder",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payorder_BLs_BLId",
                table: "Payorder",
                column: "BLId",
                principalTable: "BLs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payorder_Companies_CompanyId",
                table: "Payorder",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payorder_LCs_LCId",
                table: "Payorder",
                column: "LCId",
                principalTable: "LCs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
