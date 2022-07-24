using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLaunchpadFeatureSupported",
                schema: "Chain",
                table: "ChainInfo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LaunchpadContractAddress",
                schema: "Chain",
                table: "ChainInfo",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLaunchpadFeatureSupported",
                schema: "Chain",
                table: "ChainInfo");

            migrationBuilder.DropColumn(
                name: "LaunchpadContractAddress",
                schema: "Chain",
                table: "ChainInfo");
        }
    }
}
