using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Wallet");

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "Wallet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChainIdBase58 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    WalletAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TokenName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TokenSymbol = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TokenDecimals = table.Column<int>(type: "int", nullable: false),
                    TokenSupply = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TokenTotalSupply = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "Wallet");
        }
    }
}
