using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LockVaultContractAddress",
                schema: "Chain",
                table: "ChainInfo",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VestingVaultContractAddress",
                schema: "Chain",
                table: "ChainInfo",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockVaultContractAddress",
                schema: "Chain",
                table: "ChainInfo");

            migrationBuilder.DropColumn(
                name: "VestingVaultContractAddress",
                schema: "Chain",
                table: "ChainInfo");
        }
    }
}
