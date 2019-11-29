using System.Linq;
using System.Threading.Tasks;
using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace EMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly SignInManager<Admin> signInManager;
        private readonly UserManager<Admin> userManager;

        public AccountController(ILogger<AccountController> logger, SignInManager<Admin> signInManager, UserManager<Admin> userManager)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                SignInResult result = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

                if (result.Succeeded)
                {
                    
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ModelState.AddModelError("", "Failed to login");

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet("/createUser")]
        public async Task<IActionResult> TestAsync()
        {
            Admin user = new Admin
            {
                UserName = "Urim",
                FirstName = "Urim"
            };
            IdentityResult result = await userManager.CreateAsync(user, "PWpw6969!!");
            return Ok("test");
        }

        public async Task<IActionResult> TestLoginAsync()
        {
            Admin user = await userManager.GetUserAsync(User);

            return Ok("test");
        }
    }
}