namespace Simple_Lottery_App.Models
{
    public class User
    {

        public int UserId { get; set; } // Primary Key
        public string UserName { get; set; } // User's name
        public bool IsAdmin { get; set; } // Indictaes if the user is an admin
    }
}
