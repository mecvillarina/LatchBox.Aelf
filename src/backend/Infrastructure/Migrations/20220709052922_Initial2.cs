using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChainIdInt",
                schema: "Chain",
                table: "ChainInfo");

            migrationBuilder.AlterColumn<int>(
                name: "ChainId",
                schema: "Chain",
                table: "ChainInfo",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "ChainIdBase58",
                schema: "Chain",
                table: "ChainInfo",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChainIdBase58",
                schema: "Chain",
                table: "ChainInfo");

            migrationBuilder.AlterColumn<string>(
                name: "ChainId",
                schema: "Chain",
                table: "ChainInfo",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ChainIdInt",
                schema: "Chain",
                table: "ChainInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
