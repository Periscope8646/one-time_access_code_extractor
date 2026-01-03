using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace one_time_access_code_extractor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedRefreshTokenExpiredIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "GoogleToken",
                newName: "RefreshTokenExpiresAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "AccessTokenExpiresAt",
                table: "GoogleToken",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessTokenExpiresAt",
                table: "GoogleToken");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiresAt",
                table: "GoogleToken",
                newName: "ExpiresAt");
        }
    }
}
