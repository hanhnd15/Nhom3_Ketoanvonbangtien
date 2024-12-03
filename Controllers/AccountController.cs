using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MoneyManagementApp.Controllers
{
    public class AccountController : Controller
    {
        private const string UserName = "admin";
        private const string Password = "123";

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username == UserName && password == Password)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username)
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Transaction");
            }
            else
            {
                // Lưu thông báo lỗi vào TempData và chuyển hướng lại về trang đăng nhập
                TempData["Error"] = "Sai tên đăng nhập hoặc mật khẩu";
                return RedirectToAction("Login");
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
