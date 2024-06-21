using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Repository.Data.Migrations
{
    public partial class editorderitemproductidtoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Product_ProductId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Product_ProductId",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
