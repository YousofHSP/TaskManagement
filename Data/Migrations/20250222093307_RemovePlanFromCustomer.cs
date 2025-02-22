using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlanFromCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Plans_PlanId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Plans_PlanId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_PlanId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Customers_PlanId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_PlanId",
                table: "Jobs",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PlanId",
                table: "Customers",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Plans_PlanId",
                table: "Customers",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Plans_PlanId",
                table: "Jobs",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id");
        }
    }
}
