
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simple_Lottery_App.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LotteryId",
                table: "LotteryEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Lottery",
                columns: table => new
                {
                    LotteryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lottery", x => x.LotteryId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotteryEntries_LotteryId",
                table: "LotteryEntries",
                column: "LotteryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LotteryEntries_Lottery_LotteryId",
                table: "LotteryEntries",
                column: "LotteryId",
                principalTable: "Lottery",
                principalColumn: "LotteryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LotteryEntries_Lottery_LotteryId",
                table: "LotteryEntries");

            migrationBuilder.DropTable(
                name: "Lottery");

            migrationBuilder.DropIndex(
                name: "IX_LotteryEntries_LotteryId",
                table: "LotteryEntries");

            migrationBuilder.DropColumn(
                name: "LotteryId",
                table: "LotteryEntries");
        }
    }
}
