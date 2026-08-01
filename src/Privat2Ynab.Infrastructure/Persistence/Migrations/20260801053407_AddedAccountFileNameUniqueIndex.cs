using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Privat2Ynab.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedAccountFileNameUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Accounts_FileName",
                table: "Accounts",
                column: "FileName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Accounts_FileName",
                table: "Accounts");
        }
    }
}
