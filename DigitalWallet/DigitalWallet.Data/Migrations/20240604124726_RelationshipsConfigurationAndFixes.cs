using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalWallet.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipsConfigurationAndFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_AspNetUsers_ClientId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_ClientId",
                table: "Wallets");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "Wallets",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalCustomer",
                table: "Transactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "End",
                table: "Transactions",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_ClientId",
                table: "Wallets",
                column: "ClientId",
                unique: true,
                filter: "[ClientId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SenderId",
                table: "Transactions",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Wallets_SenderId",
                table: "Transactions",
                column: "SenderId",
                principalTable: "Wallets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_AspNetUsers_ClientId",
                table: "Wallets",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Wallets_SenderId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_AspNetUsers_ClientId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_ClientId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SenderId",
                table: "Transactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "Wallets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExternalCustomer",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "End",
                table: "Transactions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_ClientId",
                table: "Wallets",
                column: "ClientId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_AspNetUsers_ClientId",
                table: "Wallets",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
