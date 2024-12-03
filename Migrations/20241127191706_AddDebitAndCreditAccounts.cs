using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDongTien.Migrations
{
    /// <inheritdoc />
    public partial class AddDebitAndCreditAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreditAccount",
                table: "Transactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DebitAccount",
                table: "Transactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditAccount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DebitAccount",
                table: "Transactions");
        }
    }
}
