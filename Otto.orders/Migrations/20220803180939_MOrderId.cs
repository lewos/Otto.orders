using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otto.orders.Migrations
{
    public partial class MOrderId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MOrderId",
                table: "Orders",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MOrderId",
                table: "Orders");
        }
    }
}
