using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusServicePortal_TicUnicorns.Migrations
{
    public partial class AddCanteenFullFlowFinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // MEAL SUBSCRIPTIONS - PAYMENT FIELDS
            // =====================================================

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "MealSubscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "MealSubscriptions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "MealSubscriptions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");


            // =====================================================
            // MEAL PACKAGES - CANTEEN / PLAN FIELDS
            // =====================================================

            migrationBuilder.AddColumn<string>(
                name: "BillingPeriod",
                table: "MealPackages",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CanteenId",
                table: "MealPackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanType",
                table: "MealPackages",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);


            // =====================================================
            // CREATE CANTEENS TABLE
            // =====================================================

            migrationBuilder.CreateTable(
                name: "Canteens",
                columns: table => new
                {
                    CanteenId = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    HostelId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    CanteenName = table.Column<string>(
                        type: "nvarchar(120)",
                        maxLength: 120,
                        nullable: false),

                    Description = table.Column<string>(
                        type: "nvarchar(300)",
                        maxLength: 300,
                        nullable: true),

                    IsActive = table.Column<bool>(
                        type: "bit",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Canteens",
                        x => x.CanteenId);

                    table.ForeignKey(
                        name: "FK_Canteens_Hostels_HostelId",
                        column: x => x.HostelId,
                        principalTable: "Hostels",
                        principalColumn: "HostelId",
                        onDelete: ReferentialAction.Restrict);
                });


            // =====================================================
            // CREATE CANTEEN MENU TABLE
            // =====================================================

            migrationBuilder.CreateTable(
                name: "CanteenMenu",
                columns: table => new
                {
                    MenuItemId = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    CanteenId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    ItemName = table.Column<string>(
                        type: "nvarchar(120)",
                        maxLength: 120,
                        nullable: false),

                    Description = table.Column<string>(
                        type: "nvarchar(300)",
                        maxLength: 300,
                        nullable: true),

                    MealType = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: false),

                    Price = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false),

                    IsAvailable = table.Column<bool>(
                        type: "bit",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_CanteenMenu",
                        x => x.MenuItemId);

                    table.ForeignKey(
                        name: "FK_CanteenMenu_Canteens_CanteenId",
                        column: x => x.CanteenId,
                        principalTable: "Canteens",
                        principalColumn: "CanteenId",
                        onDelete: ReferentialAction.Restrict);
                });


            // =====================================================
            // MEAL PACKAGE INDEX
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_MealPackages_CanteenId_PlanType_BillingPeriod",
                table: "MealPackages",
                columns: new[]
                {
                    "CanteenId",
                    "PlanType",
                    "BillingPeriod"
                },
                unique: true,
                filter:
                    "[CanteenId] IS NOT NULL " +
                    "AND [PlanType] IS NOT NULL " +
                    "AND [BillingPeriod] IS NOT NULL");


            // =====================================================
            // CANTEEN INDEXES
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_CanteenMenu_CanteenId_MealType_ItemName",
                table: "CanteenMenu",
                columns: new[]
                {
                    "CanteenId",
                    "MealType",
                    "ItemName"
                });

            migrationBuilder.CreateIndex(
                name: "IX_Canteens_HostelId_CanteenName",
                table: "Canteens",
                columns: new[]
                {
                    "HostelId",
                    "CanteenName"
                },
                unique: true);


            // =====================================================
            // HOSTEL ROOM HOLD INDEXES
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_HostelRoomHolds_ApplicationId",
                table: "HostelRoomHolds",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelRoomHolds_RoomBedId",
                table: "HostelRoomHolds",
                column: "RoomBedId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelRoomHolds_StudentId",
                table: "HostelRoomHolds",
                column: "StudentId");


            // =====================================================
            // HOSTEL APPLICATION INDEXES
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_HostelApplications_HostelId",
                table: "HostelApplications",
                column: "HostelId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelApplications_StudentId",
                table: "HostelApplications",
                column: "StudentId");


            // =====================================================
            // HOSTEL ALLOCATION INDEXES
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocations_ApplicationId",
                table: "HostelAllocations",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocations_BedId",
                table: "HostelAllocations",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllocations_StudentId",
                table: "HostelAllocations",
                column: "StudentId");


            // =====================================================
            // HOSTEL ALLOCATION FOREIGN KEYS
            // =====================================================

            migrationBuilder.AddForeignKey(
                name:
                    "FK_HostelAllocations_HostelApplications_ApplicationId",
                table: "HostelAllocations",
                column: "ApplicationId",
                principalTable: "HostelApplications",
                principalColumn: "HostelApplicationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name:
                    "FK_HostelAllocations_RoomBeds_BedId",
                table: "HostelAllocations",
                column: "BedId",
                principalTable: "RoomBeds",
                principalColumn: "BedId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name:
                    "FK_HostelAllocations_Student_StudentId",
                table: "HostelAllocations",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);


            // =====================================================
            // HOSTEL APPLICATION FOREIGN KEYS
            // =====================================================

            migrationBuilder.AddForeignKey(
                name:
                    "FK_HostelApplications_Hostels_HostelId",
                table: "HostelApplications",
                column: "HostelId",
                principalTable: "Hostels",
                principalColumn: "HostelId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name:
                    "FK_HostelApplications_Student_StudentId",
                table: "HostelApplications",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);


            // =====================================================
            // HOSTEL ROOM HOLD FOREIGN KEYS
            // =====================================================

            migrationBuilder.AddForeignKey(
                name:
                    "FK_HostelRoomHolds_HostelApplications_ApplicationId",
                table: "HostelRoomHolds",
                column: "ApplicationId",
                principalTable: "HostelApplications",
                principalColumn: "HostelApplicationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name:
                    "FK_HostelRoomHolds_RoomBeds_RoomBedId",
                table: "HostelRoomHolds",
                column: "RoomBedId",
                principalTable: "RoomBeds",
                principalColumn: "BedId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name:
                    "FK_HostelRoomHolds_Student_StudentId",
                table: "HostelRoomHolds",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);


            // =====================================================
            // MEAL PACKAGE → CANTEEN FOREIGN KEY
            // =====================================================

            migrationBuilder.AddForeignKey(
                name:
                    "FK_MealPackages_Canteens_CanteenId",
                table: "MealPackages",
                column: "CanteenId",
                principalTable: "Canteens",
                principalColumn: "CanteenId",
                onDelete: ReferentialAction.Restrict);
        }


        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // REMOVE FOREIGN KEYS
            // =====================================================

            migrationBuilder.DropForeignKey(
                name:
                    "FK_MealPackages_Canteens_CanteenId",
                table: "MealPackages");

            migrationBuilder.DropForeignKey(
                name:
                    "FK_HostelAllocations_HostelApplications_ApplicationId",
                table: "HostelAllocations");

            migrationBuilder.DropForeignKey(
                name:
                    "FK_HostelAllocations_RoomBeds_BedId",
                table: "HostelAllocations");

            migrationBuilder.DropForeignKey(
                name:
                    "FK_HostelAllocations_Student_StudentId",
                table: "HostelAllocations");

            migrationBuilder.DropForeignKey(
                name:
                    "FK_HostelApplications_Hostels_HostelId",
                table: "HostelApplications");

            migrationBuilder.DropForeignKey(
                name:
                    "FK_HostelApplications_Student_StudentId",
                table: "HostelApplications");

            migrationBuilder.DropForeignKey(
                name:
                    "FK_HostelRoomHolds_HostelApplications_ApplicationId",
                table: "HostelRoomHolds");

            migrationBuilder.DropForeignKey(
                name:
                    "FK_HostelRoomHolds_RoomBeds_RoomBedId",
                table: "HostelRoomHolds");

            migrationBuilder.DropForeignKey(
                name:
                    "FK_HostelRoomHolds_Student_StudentId",
                table: "HostelRoomHolds");


            // =====================================================
            // DROP CANTEEN MENU
            // =====================================================

            migrationBuilder.DropTable(
                name: "CanteenMenu");


            // =====================================================
            // DROP HOSTEL INDEXES
            // =====================================================

            migrationBuilder.DropIndex(
                name: "IX_HostelRoomHolds_ApplicationId",
                table: "HostelRoomHolds");

            migrationBuilder.DropIndex(
                name: "IX_HostelRoomHolds_RoomBedId",
                table: "HostelRoomHolds");

            migrationBuilder.DropIndex(
                name: "IX_HostelRoomHolds_StudentId",
                table: "HostelRoomHolds");

            migrationBuilder.DropIndex(
                name: "IX_HostelApplications_HostelId",
                table: "HostelApplications");

            migrationBuilder.DropIndex(
                name: "IX_HostelApplications_StudentId",
                table: "HostelApplications");

            migrationBuilder.DropIndex(
                name: "IX_HostelAllocations_ApplicationId",
                table: "HostelAllocations");

            migrationBuilder.DropIndex(
                name: "IX_HostelAllocations_BedId",
                table: "HostelAllocations");

            migrationBuilder.DropIndex(
                name: "IX_HostelAllocations_StudentId",
                table: "HostelAllocations");


            // =====================================================
            // DROP MEAL PACKAGE INDEX
            // =====================================================

            migrationBuilder.DropIndex(
                name:
                    "IX_MealPackages_CanteenId_PlanType_BillingPeriod",
                table: "MealPackages");


            // =====================================================
            // DROP CANTEENS TABLE
            // =====================================================

            migrationBuilder.DropTable(
                name: "Canteens");


            // =====================================================
            // REMOVE MEAL SUBSCRIPTION COLUMNS
            // =====================================================

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "MealSubscriptions");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "MealSubscriptions");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "MealSubscriptions");


            // =====================================================
            // REMOVE MEAL PACKAGE COLUMNS
            // =====================================================

            migrationBuilder.DropColumn(
                name: "BillingPeriod",
                table: "MealPackages");

            migrationBuilder.DropColumn(
                name: "CanteenId",
                table: "MealPackages");

            migrationBuilder.DropColumn(
                name: "PlanType",
                table: "MealPackages");
        }
    }
}