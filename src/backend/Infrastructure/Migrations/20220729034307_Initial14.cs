using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContractName",
                schema: "Chain",
                table: "CrossChainOperation",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "Chain",
                table: "CrossChainOperation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RawTxData",
                schema: "Chain",
                table: "CrossChainOperation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractName",
                schema: "Chain",
                table: "CrossChainOperation");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "Chain",
                table: "CrossChainOperation");

            migrationBuilder.DropColumn(
                name: "RawTxData",
                schema: "Chain",
                table: "CrossChainOperation");
        }
    }
}
