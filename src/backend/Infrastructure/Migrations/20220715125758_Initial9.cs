using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLockingFeatureSupported",
                schema: "Chain",
                table: "ChainInfo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTokenCreationFeatureSupported",
                schema: "Chain",
                table: "ChainInfo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVestingFeatureSupported",
                schema: "Chain",
                table: "ChainInfo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLockingFeatureSupported",
                schema: "Chain",
                table: "ChainInfo");

            migrationBuilder.DropColumn(
                name: "IsTokenCreationFeatureSupported",
                schema: "Chain",
                table: "ChainInfo");

            migrationBuilder.DropColumn(
                name: "IsVestingFeatureSupported",
                schema: "Chain",
                table: "ChainInfo");
        }
    }
}
