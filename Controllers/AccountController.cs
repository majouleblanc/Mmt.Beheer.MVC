using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mmt.Beheer.MVC.Models;
using Mmt.Beheer.MVC.Services;

namespace Mmt.Beheer.MVC.Controllers
{
    [AllowAnonymous]
    [Route("[Controller]/[Action]")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IAuthServices _AuthServices;

        public AccountController(IConfiguration configuration, IAuthServices authServices)
        {
            _Configuration = configuration;
            _AuthServices = authServices;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var LoginResult = await _AuthServices.LogInUserAsync(model.Email, model.Password);
            if (!LoginResult.IsSucces)
            {
                ViewData["ErrorMessage"] = "Login attempt Faild";
                return View();
            }
            var token = LoginResult.Result;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, model.Email),
                new Claim("token", token)
            };
            foreach (var role in LoginResult.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTime.UtcNow.AddDays(1),
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
