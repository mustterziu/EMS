using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        private readonly EMSContext context;

        public AccountController(ILogger<AccountController> logger, SignInManager<Admin> signInManager, UserManager<Admin> userManager, EMSContext context)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.context = context;
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
        public async Task<IActionResult> Login(Login model)
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

        [Authorize]
        public IActionResult Security()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SecurityAsync(PasswordChange password)
        {
            if (ModelState.IsValid)
            {
                Admin user = await userManager.GetUserAsync(User);

                if (await userManager.CheckPasswordAsync(user, password.currentPassword))
                {
                    if (password.newPassword.Equals(password.confirmNewPassword))
                    {
                        IdentityResult result = await userManager.ChangePasswordAsync(user, password.currentPassword, password.newPassword);
                        if (result.Succeeded)
                        {
                            var claim = new Claim("PasswordChangeRequired", "true");
                            IdentityResult task = await userManager.RemoveClaimAsync(user, claim);
                            await signInManager.SignInAsync(user, false);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", result.Errors.ToList().FirstOrDefault().Description);
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Konfirmo mire fjalekalimin e ri");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Fjalekalimi eshte gabim");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Plotesoni te gjitha fushat.");
                return View();
            }
            return View();
        }

        public IActionResult EmployeeLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginEmployee(int empId)
        {
            Employee employee = context.Employee.FirstOrDefault(emp => emp.Id == empId);
            return View(employee);
        }

        [HttpGet("/createUser")]
        public async Task<IActionResult> TestAsync()
        {
            Admin user = new Admin
            {
                UserName = "UserTest",
                PasswordChangeRequired = false,
            };
            IdentityResult result = await userManager.CreateAsync(user, "UserTest123!!");
            return Ok("User: UserTest, Password: UserTest123!!");
        }
    }
}