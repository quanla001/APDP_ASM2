using APDP_ASM2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace APDP_ASM2.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            if (string.IsNullOrEmpty(user.UserName))
            {
                ViewBag.error = "Username cannot be empty!";
                return View("Login");
            }

            if (string.IsNullOrEmpty(user.Pass))
            {
                ViewBag.error = "Password cannot be empty!";
                return View("Login");
            }

            List<User> users = LoadUsersFromFile("users.json");
            var result = users.Find(u => u.UserName == user.UserName);

            if (result != null && result.Pass == user.Pass)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.UserName),
                    new Claim(ClaimTypes.Role, result.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToRoleDashboard(result.Role);
            }
            else
            {
                ViewBag.error = "Invalid username or password!";
                return View("Login");
            }
        }

        private IActionResult RedirectToRoleDashboard(string role)
        {
            return role switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "Teacher" => RedirectToAction("Index", "Teacher"),
                "Student" => RedirectToAction("Index", "Student"),
                _ => RedirectToAction("Index", "Home"),
            };
        }

        [HttpGet] // Chuyển từ HttpPost sang HttpGet
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public List<User>? LoadUsersFromFile(string fileName)
        {
            string readText = System.IO.File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<List<User>>(readText);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (user.Pass != user.ConfirmPass)
                {
                    ModelState.AddModelError("ConfirmPass", "Passwords do not match");
                    return View("Register", user);
                }

                List<User> users = LoadUsersFromFile("users.json");

                if (users.Any(u => u.UserName == user.UserName))
                {
                    ModelState.AddModelError("UserName", "Username already exists");
                    return View("Register", user);
                }

                users.Add(user);

                string jsonString = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText("users.json", jsonString);

                return RedirectToAction("Login");
            }

            return View("Register", user);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}