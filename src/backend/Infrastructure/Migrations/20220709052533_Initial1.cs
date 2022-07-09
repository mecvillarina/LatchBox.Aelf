using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Chain");

            migrationBuilder.CreateTable(
                name: "ChainInfo",
                schema: "Chain",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChainId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ChainIdInt = table.Column<int>(type: "int", nullable: false),
                    RpcApi = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    LongestChainHeight = table.Column<long>(type: "bigint", nullable: false),
                    LongestChainHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    GenesisBlockHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    GenesisContractAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastIrreversibleBlockHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastIrreversibleBlockHeight = table.Column<long>(type: "bigint", nullable: false),
                    BestChainHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    BestChainHeight = table.Column<long>(type: "bigint", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChainInfo", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChainInfo",
                schema: "Chain");
        }
    }
}
