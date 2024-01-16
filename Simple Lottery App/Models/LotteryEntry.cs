namespace Simple_Lottery_App.Models
{
    public class LotteryEntry
    {
        public int LotteryEntryId { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key

        public User User { get; set; } // Navigation property to User
    }
}
