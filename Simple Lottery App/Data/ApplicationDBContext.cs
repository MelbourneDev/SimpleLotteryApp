using Microsoft.EntityFrameworkCore;
using Simple_Lottery_App.Models;

namespace Simple_Lottery_App.Data
    
{   

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options)
            :base(options){ }
        
        public DbSet<User> Users { get; set; }
        public DbSet<LotteryEntry> LotteryEntries { get; set;}
    }
}
