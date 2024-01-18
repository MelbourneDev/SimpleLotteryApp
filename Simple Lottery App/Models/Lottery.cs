namespace Simple_Lottery_App.Models
{
    public class Lottery
    {
        public int LotteryId { get; set; } // Primary Key
                                           
        public ICollection<LotteryEntry> Entries { get; set; } // Users participating in this lottery

        public bool IsActive { get; set; } // property to indicate if the lottery is active
    }

}
