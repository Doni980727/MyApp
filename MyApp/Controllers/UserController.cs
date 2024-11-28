using MyApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string username, string password, string email)
        {
            // Kontrollera om användarnamnet redan finns
            if (_context.Users.Any(u => u.Username == username))
            {
                ViewBag.ErrorMessage = "Username already exists.";
                return View();
            }

            // Skapa en ny användare och spara i databasen
            var newUser = new User
            {
                Username = username,
                Password = password, // OBS! Hasha lösenord i verkliga applikationer
                Email = email
            };

            _context.Users.Add(newUser);
            _context.SaveChanges(); // Spara ändringarna till databasen

            TempData["SuccessMessage"] = "Registration successful. You can now log in.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Kontrollera om användaren finns i databasen
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                TempData["SuccessMessage"] = $"Welcome, {username}!";
                return RedirectToAction("Index", "Budget");
            }

            ViewBag.ErrorMessage = "Invalid username or password.";
            return View();
        }
    }
}
