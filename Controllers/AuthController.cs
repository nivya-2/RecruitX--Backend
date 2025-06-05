using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RecruitX.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;


        public AuthController(AppDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action("LoginCallback", "Auth");
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Authorize]
        [HttpGet("LoginCallback")]
        public IActionResult LoginCallback()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                     ?? User.FindFirst("preferred_username")?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value
                     ?? User.FindFirst("name")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Email not found in Azure claims.");
                return Unauthorized("Email not found");
            }

            // Lookup user in your DB
            var user = _context.Users
                .Include(u => u.Employee)
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                Console.WriteLine($"User not found in DB: {email}");
                return Unauthorized("User not registered.");
            }

            var role = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId)?.RoleName ?? "Unknown";

            Console.WriteLine($"Login success: {name} ({email}), Role: {role}");

            // Redirect based on role
            return role switch
            {
                "Recruiter Head" => Redirect("http://localhost:4200/recruiter-head/jrs/assign-jr"),
                "Recruiter Lead" => Redirect("http://localhost:4200/recruiter-lead/jrs/assign-jr"),
                "Recruiter" => Redirect("http://localhost:4200/recruiter/my-jd/pendingjdgeneration"),
                "Admin" => Redirect("http://localhost:4200/admin/add-jr"),     
                _ => Redirect("http://localhost:4200/unauthorized")
            };
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                     ?? User.FindFirst("preferred_username")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("No email found.");
            }

            var user = _context.Users
                .Include(u => u.Employee)
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var role = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId)?.RoleName ?? "Unknown";
            Console.WriteLine($"Login success: {user.Username} ({email}), Role: {role}");


            return Ok(new
            {
                name = user.Username,
                email = user.Email,
                role
            });
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            var userIdentifier = User.Identity?.IsAuthenticated == true
                ? (User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity.Name)
                : "Anonymous";
            _logger.LogInformation($"Logout initiated for user: {userIdentifier}.");

            HttpContext.Session.Clear();
            var postLogoutRedirectUri = "http://localhost:4200"; // Adjust to your Angular app's desired post-logout page

            var properties = new AuthenticationProperties { RedirectUri = postLogoutRedirectUri };

            return SignOut(properties,
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }


    }
}
