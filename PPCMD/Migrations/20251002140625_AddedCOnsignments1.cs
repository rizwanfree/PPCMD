using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPCMD.Migrations
{
    /// <inheritdoc />
    public partial class AddedCOnsignments1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Companies_CompanyId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_DutyTypes_Companies_CompanyId",
                table: "DutyTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemDuties_Companies_CompanyId",
                table: "ItemDuties");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Companies_CompanyId",
                table: "Items");

            migrationBuilder.CreateTable(
                name: "IGMs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Vessel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PortId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IGMs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IGMs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IGMs_Ports_PortId",
                        column: x => x.PortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LCs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LCNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeclaredValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssessedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LandingCharges = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LCs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LCs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PendingBLs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    BLNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssignedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IndexNumber = table.Column<int>(type: "int", nullable: false),
                    JobNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IGMId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IGMId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingBLs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingBLs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PendingBLs_IGMs_IGMId",
                        column: x => x.IGMId,
                        principalTable: "IGMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PendingBLs_IGMs_IGMId1",
                        column: x => x.IGMId1,
                        principalTable: "IGMs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BLItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PendingBLId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImportValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InsuranceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FreightCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BLItems_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BLItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BLItems_PendingBLs_PendingBLId",
                        column: x => x.PendingBLId,
                        principalTable: "PendingBLs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BLs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BLDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CashRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CashDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GDRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GDDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LCId = table.Column<int>(type: "int", nullable: false),
                    PendingBLId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BLs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BLs_LCs_LCId",
                        column: x => x.LCId,
                        principalTable: "LCs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BLs_PendingBLs_PendingBLId",
                        column: x => x.PendingBLId,
                        principalTable: "PendingBLs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DutyCharges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BLId = table.Column<int>(type: "int", nullable: false),
                    ImportValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InsuranceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LadingCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DutyTypeId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    IsPercentage = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    BLId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DutyCharges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DutyCharges_BLs_BLId",
                        column: x => x.BLId,
                        principalTable: "BLs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DutyCharges_BLs_BLId1",
                        column: x => x.BLId1,
                        principalTable: "BLs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DutyCharges_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DutyCharges_DutyTypes_DutyTypeId",
                        column: x => x.DutyTypeId,
                        principalTable: "DutyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HCs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BLId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HCs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HCs_BLs_BLId",
                        column: x => x.BLId,
                        principalTable: "BLs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HCs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BLItems_CompanyId",
                table: "BLItems",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BLItems_ItemId",
                table: "BLItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BLItems_PendingBLId",
                table: "BLItems",
                column: "PendingBLId");

            migrationBuilder.CreateIndex(
                name: "IX_BLs_CompanyId",
                table: "BLs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BLs_LCId",
                table: "BLs",
                column: "LCId");

            migrationBuilder.CreateIndex(
                name: "IX_BLs_PendingBLId",
                table: "BLs",
                column: "PendingBLId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DutyCharges_BLId",
                table: "DutyCharges",
                column: "BLId");

            migrationBuilder.CreateIndex(
                name: "IX_DutyCharges_BLId1",
                table: "DutyCharges",
                column: "BLId1");

            migrationBuilder.CreateIndex(
                name: "IX_DutyCharges_CompanyId",
                table: "DutyCharges",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DutyCharges_DutyTypeId",
                table: "DutyCharges",
                column: "DutyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HCs_BLId",
                table: "HCs",
                column: "BLId");

            migrationBuilder.CreateIndex(
                name: "IX_HCs_CompanyId",
                table: "HCs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IGMs_CompanyId",
                table: "IGMs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IGMs_PortId",
                table: "IGMs",
                column: "PortId");

            migrationBuilder.CreateIndex(
                name: "IX_LCs_CompanyId",
                table: "LCs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingBLs_CompanyId",
                table: "PendingBLs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingBLs_IGMId",
                table: "PendingBLs",
                column: "IGMId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingBLs_IGMId1",
                table: "PendingBLs",
                column: "IGMId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Companies_CompanyId",
                table: "Clients",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DutyTypes_Companies_CompanyId",
                table: "DutyTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDuties_Companies_CompanyId",
                table: "ItemDuties",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Companies_CompanyId",
                table: "Items",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Companies_CompanyId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_DutyTypes_Companies_CompanyId",
                table: "DutyTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemDuties_Companies_CompanyId",
                table: "ItemDuties");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Companies_CompanyId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "BLItems");

            migrationBuilder.DropTable(
                name: "DutyCharges");

            migrationBuilder.DropTable(
                name: "HCs");

            migrationBuilder.DropTable(
                name: "BLs");

            migrationBuilder.DropTable(
                name: "LCs");

            migrationBuilder.DropTable(
                name: "PendingBLs");

            migrationBuilder.DropTable(
                name: "IGMs");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Companies_CompanyId",
                table: "Clients",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DutyTypes_Companies_CompanyId",
                table: "DutyTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDuties_Companies_CompanyId",
                table: "ItemDuties",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Companies_CompanyId",
                table: "Items",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
