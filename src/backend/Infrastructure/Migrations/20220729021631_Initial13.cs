using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrossChainOperation",
                schema: "Chain",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    From = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ChainOperation = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ChainBlockNumber = table.Column<long>(type: "bigint", nullable: false),
                    ChainBlockHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ChainId = table.Column<int>(type: "int", nullable: false),
                    IssueChainId = table.Column<int>(type: "int", nullable: false),
                    IssueChainOperation = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrossChainOperation", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrossChainOperation",
                schema: "Chain");
        }
    }
}
