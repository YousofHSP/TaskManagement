using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Customers_ParentId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ParentId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ParentId",
                table: "Customers",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Customers_ParentId",
                table: "Customers",
                column: "ParentId",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
