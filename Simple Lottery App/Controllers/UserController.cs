using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simple_Lottery_App.Data;
using Simple_Lottery_App.Models;


namespace Simple_Lottery_App.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
                // find a user that has matching userId and userName
                var user = _context.Users.FirstOrDefault(u => u.UserId == model.UserId && u.UserName == model.UserName);
            // if user was found
            if (user == null)
            {
                //if no user found, reload the page as the credentials dont match
                ViewBag.LoginError = "Invalid UserId or Username.";
                return View(model); //reload the logjn page with the error message
            }

            // If a user is found, store user ID in session
            HttpContext.Session.SetInt32("UserId", user.UserId);

            // If a user is found, redirect them based o their role
            return RedirectToAction(user.IsAdmin ? "AdminPage" : "UserPage", "User");

        }
        public IActionResult AdminPage()
        {
            // Fetch the latest lottery information
            var latestLottery = _context.Lottery
                                        .Include(l => l.Entries)
                                        .ThenInclude(e => e.User)
                                        .OrderByDescending(l => l.LotteryId)
                                        .FirstOrDefault();

            ViewBag.LatestLottery = latestLottery;

            return View();
        }


        public IActionResult StartLotto()
        {
            // Create a new Lottery object
            var newLottery = new Lottery
            {
                // Initialize the Entries collection
                Entries = new List<LotteryEntry>()
            };

            // Add the new Lottery to the database and save changes
            _context.Lottery.Add(newLottery);
            _context.SaveChanges();

            // Redirect to the AdminPage
            return RedirectToAction("AdminPage");
        }
        public IActionResult EndLotto()
        {
            var currentLottery = _context.Lottery
                                         .Include(l => l.Entries)
                                         .ThenInclude(e => e.User)
                                         .OrderByDescending(l => l.LotteryId)
                                         .FirstOrDefault();

            if (currentLottery != null)
            {
                // Check if there are entries in the lottery
                if (currentLottery.Entries != null && currentLottery.Entries.Any())
                {
                    // Logic to randomly select a winner
                    var winnerEntry = currentLottery.Entries
                                          .OrderBy(r => Guid.NewGuid()) // Randomize entries
                                          .FirstOrDefault(); // Take the first entry as the winner

                    if (winnerEntry != null)
                    {
                        // You can now do something with the winner.
                        // For example, storing the winner's ID in the Lottery table
                        // currentLottery.WinnerId = winnerEntry.UserId;
                        // Add any other logic you need for when a winner is selected
                    }

                    // Remove all entries from this lottery
                    _context.LotteryEntries.RemoveRange(currentLottery.Entries);
                }

                _context.SaveChanges();
            }

            return RedirectToAction("AdminPage");
        }


        public IActionResult UserPage()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");

            if (!currentUserId.HasValue)
            {
                // If the user ID is not in the session, redirect to the login page
                return RedirectToAction("Login");
            }

            // Fetch the current lottery information
            var currentLottery = _context.Lottery
                                         .Include(l => l.Entries)
                                         .OrderByDescending(l => l.LotteryId)
                                         .FirstOrDefault();

            // You might want to pass this information to the view
            ViewBag.CurrentLottery = currentLottery;

            return View();
        }

        public IActionResult EnterLotto()
        {
            var currentUserIdNullable = this.HttpContext.Session.GetInt32("UserId"); // This returns an 'int?'

            if (!currentUserIdNullable.HasValue)
            {
                // Handle the case where the user ID is not available
                // Redirect to login page or show an error message
                return RedirectToAction("Login"); // Or any other appropriate action
            }

            // Now that we have checked for null, we can safely use the value
            var currentUserId = currentUserIdNullable.Value;

            var currentLottery = _context.Lottery.OrderByDescending(l => l.LotteryId).FirstOrDefault();

            if (currentLottery != null)
            {
                var lotteryEntry = new LotteryEntry
                {
                    UserId = currentUserId,
                    LotteryId = currentLottery.LotteryId
                };

                _context.LotteryEntries.Add(lotteryEntry);
                _context.SaveChanges();
            }

            return RedirectToAction("UserPage");
        }

        public IActionResult LeaveLotto()
        {
            var currentUserId = this.HttpContext.Session.GetInt32("UserId");

            // Find the user's entry in the latest lottery
            var currentLottery = _context.Lottery.OrderByDescending(l => l.LotteryId).FirstOrDefault();
            var entryToRemove = _context.LotteryEntries
                .FirstOrDefault(le => le.UserId == currentUserId && le.LotteryId == currentLottery.LotteryId);

            if (entryToRemove != null)
            {
                // Remove the entry from the database and save changes
                _context.LotteryEntries.Remove(entryToRemove);
                _context.SaveChanges();
            }

            // Redirect to the UserPage
            return RedirectToAction("UserPage");
        }


    }
}
