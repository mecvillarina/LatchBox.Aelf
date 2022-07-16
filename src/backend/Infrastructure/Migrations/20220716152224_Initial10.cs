using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChainIdBase58",
                schema: "Wallet",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "Wallet",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "TokenName",
                schema: "Wallet",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "TokenSupply",
                schema: "Wallet",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "TokenSymbol",
                schema: "Wallet",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "Wallet",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "WalletAddress",
                schema: "Wallet",
                table: "Tokens");

            migrationBuilder.RenameTable(
                name: "Tokens",
                schema: "Wallet",
                newName: "Tokens",
                newSchema: "Chain");

            migrationBuilder.RenameColumn(
                name: "TokenTotalSupply",
                schema: "Chain",
                table: "Tokens",
                newName: "Symbol");

            migrationBuilder.RenameColumn(
                name: "TokenDecimals",
                schema: "Chain",
                table: "Tokens",
                newName: "ChainId");

            migrationBuilder.AddColumn<string>(
                name: "Issuer",
                schema: "Chain",
                table: "Tokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawExplorerData",
                schema: "Chain",
                table: "Tokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RawTxData",
                schema: "Chain",
                table: "Tokens",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Issuer",
                schema: "Chain",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "RawExplorerData",
                schema: "Chain",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "RawTxData",
                schema: "Chain",
                table: "Tokens");

            migrationBuilder.EnsureSchema(
                name: "Wallet");

            migrationBuilder.RenameTable(
                name: "Tokens",
                schema: "Chain",
                newName: "Tokens",
                newSchema: "Wallet");

            migrationBuilder.RenameColumn(
                name: "Symbol",
                schema: "Wallet",
                table: "Tokens",
                newName: "TokenTotalSupply");

            migrationBuilder.RenameColumn(
                name: "ChainId",
                schema: "Wallet",
                table: "Tokens",
                newName: "TokenDecimals");

            migrationBuilder.AddColumn<string>(
                name: "ChainIdBase58",
                schema: "Wallet",
                table: "Tokens",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "Wallet",
                table: "Tokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TokenName",
                schema: "Wallet",
                table: "Tokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TokenSupply",
                schema: "Wallet",
                table: "Tokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TokenSymbol",
                schema: "Wallet",
                table: "Tokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "Wallet",
                table: "Tokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress",
                schema: "Wallet",
                table: "Tokens",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
