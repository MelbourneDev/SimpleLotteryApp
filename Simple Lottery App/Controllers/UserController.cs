using Microsoft.AspNetCore.Mvc;
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

            // If a user is found, redirect them based o their role
            return RedirectToAction(user.IsAdmin ? "AdminPage" : "UserPage", "User");

        }

    }
}
