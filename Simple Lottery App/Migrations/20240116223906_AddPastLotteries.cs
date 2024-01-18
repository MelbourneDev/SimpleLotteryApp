using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simple_Lottery_App.Migrations
{
    /// <inheritdoc />
    public partial class AddPastLotteries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PastLotteries",
                columns: table => new
                {
                    PastLotteryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LotteryId = table.Column<int>(type: "INTEGER", nullable: false),
                    WinnerUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParticipantUserIds = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PastLotteries", x => x.PastLotteryId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PastLotteries");
        }
    }
}
