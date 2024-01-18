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

        public IActionResult Logout()
        {   // Clears the current session
            HttpContext.Session.Clear();
            // Redirects user to the Login page
            return RedirectToAction("Login"); 
        }


        public IActionResult AdminPage()
        {
            var latestLottery = _context.Lottery
                                        .Include(l => l.Entries)
                                        .ThenInclude(e => e.User)
                                        .OrderByDescending(l => l.LotteryId)
                                        .FirstOrDefault();

            ViewBag.LatestLottery = latestLottery?.IsActive == true ? latestLottery : null;

            if (TempData["WinnerUserId"] != null)
            {
                ViewBag.WinnerUserId = TempData["WinnerUserId"];
                ViewBag.WinnerUserName = TempData["WinnerUserName"];
                ViewBag.ParticipantUserIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(TempData["ParticipantUserIds"].ToString());
            }

            return View();
        }





        public IActionResult StartLotto()
        {
            var newLottery = new Lottery
            {
                Entries = new List<LotteryEntry>(),
                IsActive = true // Set active status to true
            };

            _context.Lottery.Add(newLottery);
            _context.SaveChanges();

            return RedirectToAction("AdminPage");
        }



        public IActionResult EndLotto()
        {
            var currentLottery = _context.Lottery
                                       .Include(l => l.Entries)
                                       .ThenInclude(e => e.User)
                                       .OrderByDescending(l => l.LotteryId)
                                       .FirstOrDefault();

            if (currentLottery != null && currentLottery.IsActive)
            {
                currentLottery.IsActive = false;
                var participantIds = currentLottery.Entries.Select(e => e.UserId).ToList();
                var winnerEntry = currentLottery.Entries
                                                .OrderBy(r => Guid.NewGuid())
                                                .FirstOrDefault();

                if (winnerEntry != null)
                {
                    var pastLottery = new PastLottery
                    {
                        LotteryId = currentLottery.LotteryId,
                        WinnerUserId = winnerEntry.UserId, 
                        ParticipantUserIds = currentLottery.Entries.Select(e => e.UserId).ToList()
                    };
                    _context.PastLotteries.Add(pastLottery);

                    // Removing entries from the current lottery
                    _context.LotteryEntries.RemoveRange(currentLottery.Entries);
                    _context.SaveChanges();

                    TempData["WinnerUserId"] = winnerEntry.UserId;
                    TempData["WinnerUserName"] = winnerEntry.User.UserName;
                    TempData["ParticipantUserIds"] = Newtonsoft.Json.JsonConvert.SerializeObject(participantIds);
                    ViewBag.LatestLottery = null;
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
                return RedirectToAction("Login");
            }

            var currentLottery = _context.Lottery
                                         .Include(l => l.Entries)
                                         .OrderByDescending(l => l.LotteryId)
                                         .FirstOrDefault();

            bool isCurrentLotteryActive = currentLottery != null && !_context.PastLotteries.Any(p => p.LotteryId == currentLottery.LotteryId);


            var isUserEntered = currentLottery?.Entries.Any(e => e.UserId == currentUserId.Value) ?? false;

            ViewBag.CurrentLotteryActive = isCurrentLotteryActive;
            ViewBag.CurrentLottery = currentLottery;
            ViewBag.IsUserEntered = isUserEntered;

            var pastLottery = _context.PastLotteries
                              .OrderByDescending(p => p.PastLotteryId)
                              .FirstOrDefault();

            ViewBag.UserWon = pastLottery != null && pastLottery.WinnerUserId == currentUserId.Value;
            ViewBag.UserParticipated = pastLottery != null && pastLottery.ParticipantUserIds.Contains(currentUserId.Value);
            ViewBag.PastLotteryId = pastLottery?.LotteryId;

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
