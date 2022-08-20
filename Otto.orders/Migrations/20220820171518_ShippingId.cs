using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otto.orders.Migrations
{
    public partial class ShippingId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MShippingId",
                table: "Orders",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MShippingId",
                table: "Orders");
        }
    }
}
