namespace Simple_Lottery_App.Models
{
    public class PastLottery
    {
        public int PastLotteryId { get; set; }
        public int LotteryId { get; set; } // ID of the lottery
        public int WinnerUserId { get; set; } // ID of the winning user
        public List<int> ParticipantUserIds { get; set; } // List of participant user IDs

      
    }
}
