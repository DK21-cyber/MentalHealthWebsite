using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PW.Models;
using PW.Models.ViewModels;

namespace PW.Controllers
{
    public class AccountController : Controller
    {

        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        //=========================================
        // LOGIN
        //=========================================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.Email == model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }

                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }

                HttpContext.Session.SetString("UserId", user.Id);

                HttpContext.Session.SetString("FullName", user.FullName);

                HttpContext.Session.SetString("Role", user.RoleName);

                TempData["Success"] = "Login successful.";

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                TempData["Error"] = "Login failed.";

                return View(model);
            }
        }

        //=========================================
        // REGISTER
        //=========================================

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                // Check email already exists
                bool exists = await _context.Users
                    .AnyAsync(x => x.Email == model.Email);

                if (exists)
                {
                    ModelState.AddModelError("", "This email address is already registered.");
                    return View(model);
                }

                ApplicationUser user = new ApplicationUser
                {
                    FullName = model.FullName,
                    Email = model.Email,

                    // using BCrypt:
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),

                    

                    RoleName = "Student",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Your account has been created successfully.";

                return RedirectToAction(nameof(Login));
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "An unexpected error occurred while creating your account.";

                return View(model);
            }
        }

        //=========================================
        // FORGOT PASSWORD
        //=========================================

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.Email == model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Email address was not found.");

                    return View(model);
                }

                // Nếu dùng BCrypt
                //user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

                // Nếu chưa dùng BCrypt
                user.PasswordHash = model.NewPassword;

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Password has been reset successfully.";

                return RedirectToAction(nameof(Login));
            }
            catch
            {
                TempData["Error"] =
                    "Unable to reset password.";

                return View(model);
            }
        }

        //=========================================
        // LOGOUT
        //=========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();

                TempData["Success"] =
                    "You have logged out successfully.";

                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "An unexpected error occurred while logging out.";

                return RedirectToAction("Index", "Home");
            }
        }

        //=========================================
        // ACCESS DENIED
        //=========================================

        [HttpGet]
        public IActionResult AccessDenied()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to load the requested page.";

                return RedirectToAction("Index", "Home");
            }
        }
    }
}