using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomForms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);
        }
    }
}
