using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HXSail.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "annualFeeStructure",
                columns: table => new
                {
                    year = table.Column<int>(nullable: false),
                    annualFee = table.Column<double>(nullable: true),
                    earlyDiscountedFee = table.Column<double>(nullable: true),
                    earlyDiscountEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    renewDeadlineDate = table.Column<DateTime>(type: "date", nullable: true),
                    taskExemptionFee = table.Column<double>(nullable: true),
                    secondBoatFee = table.Column<double>(nullable: true),
                    thirdBoatFee = table.Column<double>(nullable: true),
                    forthAndSubsequentBoatFee = table.Column<double>(nullable: true),
                    nonSailFee = table.Column<double>(nullable: true),
                    newMember25DiscountDate = table.Column<DateTime>(type: "date", nullable: true),
                    newMember50DiscountDate = table.Column<DateTime>(type: "date", nullable: true),
                    newMember75DiscountDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_annualFeeStructure", x => x.year)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "boatType",
                columns: table => new
                {
                    boatTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    description = table.Column<string>(unicode: false, nullable: true),
                    chargeable = table.Column<bool>(nullable: false),
                    sail = table.Column<bool>(nullable: false),
                    image = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boatType", x => x.boatTypeId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "country",
                columns: table => new
                {
                    countryCode = table.Column<string>(unicode: false, maxLength: 2, nullable: false),
                    name = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    postalPattern = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    phonePattern = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    federalSalesTax = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_country", x => x.countryCode);
                });

            migrationBuilder.CreateTable(
                name: "membershipType",
                columns: table => new
                {
                    membershipTypeName = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    description = table.Column<string>(unicode: false, nullable: true),
                    ratioToFull = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membershipType", x => x.membershipTypeName)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "surcharge",
                columns: table => new
                {
                    surchargeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    startYear = table.Column<int>(nullable: true),
                    endYear = table.Column<int>(nullable: true),
                    amount = table.Column<double>(nullable: true),
                    name = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    description = table.Column<string>(unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_surcharge", x => x.surchargeId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    taskId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    description = table.Column<string>(unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.taskId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "town",
                columns: table => new
                {
                    townName = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    provinceCode = table.Column<string>(unicode: false, maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_town", x => x.townName);
                });

            migrationBuilder.CreateTable(
                name: "parking",
                columns: table => new
                {
                    parkingCode = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    boatTypeId = table.Column<int>(nullable: true),
                    actualBoatId = table.Column<string>(unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parking", x => x.parkingCode)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_parking_boatType",
                        column: x => x.boatTypeId,
                        principalTable: "boatType",
                        principalColumn: "boatTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "province",
                columns: table => new
                {
                    provinceCode = table.Column<string>(unicode: false, maxLength: 2, nullable: false),
                    name = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    countryCode = table.Column<string>(unicode: false, maxLength: 2, nullable: false),
                    taxCode = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    taxRate = table.Column<double>(nullable: false),
                    capital = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    includesFerderalTax = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_province", x => x.provinceCode);
                    table.ForeignKey(
                        name: "FK_province_country",
                        column: x => x.countryCode,
                        principalTable: "country",
                        principalColumn: "countryCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "member",
                columns: table => new
                {
                    memberId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    fullName = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    firstName = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    lastName = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    spouseFirstName = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    spouseLastName = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    street = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    city = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    provinceCode = table.Column<string>(unicode: false, maxLength: 2, nullable: true),
                    postalCode = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    homePhone = table.Column<string>(unicode: false, maxLength: 14, nullable: true),
                    email = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    yearJoined = table.Column<int>(nullable: true),
                    comment = table.Column<string>(unicode: false, nullable: true),
                    taskExempt = table.Column<bool>(nullable: false),
                    useCanadaPost = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member", x => x.memberId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_member_province",
                        column: x => x.provinceCode,
                        principalTable: "province",
                        principalColumn: "provinceCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "boat",
                columns: table => new
                {
                    boatId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    memberId = table.Column<int>(nullable: true),
                    boatClass = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    hullColour = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    sailNumber = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    hullLength = table.Column<double>(nullable: true),
                    boatTypeId = table.Column<int>(nullable: true),
                    parkingCode = table.Column<string>(unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boat", x => x.boatId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_boat_boatType",
                        column: x => x.boatTypeId,
                        principalTable: "boatType",
                        principalColumn: "boatTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_boat_member",
                        column: x => x.memberId,
                        principalTable: "member",
                        principalColumn: "memberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_boat_parking",
                        column: x => x.parkingCode,
                        principalTable: "parking",
                        principalColumn: "parkingCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "membership",
                columns: table => new
                {
                    membershipId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    memberId = table.Column<int>(nullable: false),
                    year = table.Column<int>(nullable: false),
                    membershipTypeName = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    fee = table.Column<double>(nullable: false),
                    comments = table.Column<string>(unicode: false, nullable: true),
                    paid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membership", x => x.membershipId);
                    table.ForeignKey(
                        name: "FK_membership_member",
                        column: x => x.memberId,
                        principalTable: "member",
                        principalColumn: "memberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_membership_membershipType",
                        column: x => x.membershipTypeName,
                        principalTable: "membershipType",
                        principalColumn: "membershipTypeName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_membership_annualFeeStructure_year",
                        column: x => x.year,
                        principalTable: "annualFeeStructure",
                        principalColumn: "year",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "memberTask",
                columns: table => new
                {
                    memberTaskId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    wednesdayDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    taskId = table.Column<int>(nullable: true),
                    memberId = table.Column<int>(nullable: true),
                    comment = table.Column<string>(unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memberTask", x => x.memberTaskId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_memberTask_member",
                        column: x => x.memberId,
                        principalTable: "member",
                        principalColumn: "memberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_memberTask_tasks",
                        column: x => x.taskId,
                        principalTable: "tasks",
                        principalColumn: "taskId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_boat_boatTypeId",
                table: "boat",
                column: "boatTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_boat_memberId",
                table: "boat",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_boat_parkingCode",
                table: "boat",
                column: "parkingCode");

            migrationBuilder.CreateIndex(
                name: "IX_member_provinceCode",
                table: "member",
                column: "provinceCode");

            migrationBuilder.CreateIndex(
                name: "IX_membership_memberId",
                table: "membership",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_membership_membershipTypeName",
                table: "membership",
                column: "membershipTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_membership_year",
                table: "membership",
                column: "year",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_memberTask_memberId",
                table: "memberTask",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_memberTask_taskId",
                table: "memberTask",
                column: "taskId");

            migrationBuilder.CreateIndex(
                name: "IX_parking_boatTypeId",
                table: "parking",
                column: "boatTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_province_countryCode",
                table: "province",
                column: "countryCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "boat");

            migrationBuilder.DropTable(
                name: "membership");

            migrationBuilder.DropTable(
                name: "memberTask");

            migrationBuilder.DropTable(
                name: "surcharge");

            migrationBuilder.DropTable(
                name: "town");

            migrationBuilder.DropTable(
                name: "parking");

            migrationBuilder.DropTable(
                name: "membershipType");

            migrationBuilder.DropTable(
                name: "annualFeeStructure");

            migrationBuilder.DropTable(
                name: "member");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "boatType");

            migrationBuilder.DropTable(
                name: "province");

            migrationBuilder.DropTable(
                name: "country");
        }
    }
}
