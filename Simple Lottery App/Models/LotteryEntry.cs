namespace Simple_Lottery_App.Models
{
    public class LotteryEntry
    {
        public int LotteryEntryId { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key to User
        public int LotteryId { get; set; } // Foreign Key to Lottery

        public User User { get; set; } // Navigation property to User
        public Lottery Lottery { get; set; } // Navigation property to Lottery
    }

}
