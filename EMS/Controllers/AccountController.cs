using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace EMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly SignInManager<Admin> signInManager;
        private readonly UserManager<Admin> userManager;
        private readonly EMSContext context;
        private readonly IConfiguration config;

        public AccountController(ILogger<AccountController> logger, SignInManager<Admin> signInManager, UserManager<Admin> userManager, EMSContext context, IConfiguration configuration)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.context = context;
            this.config = configuration;
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
            ModelState.AddModelError("", "Invalid Username or Password");
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

        public IActionResult Homepage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Profile(int empId)
        {
            try
            {
                Employee employee = context.Employee.Find(empId);
                if (employee == null)
                {
                    //TODO return a view
                    return NotFound();
                }
                return View(employee);
            }
            catch (Exception e)
            {
                context.Logs.Add(new Logs
                {
                    mesazhi = e.Message,
                    createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    createdAt = DateTime.Now
                });

                context.SaveChanges();

                return RedirectToAction("Error", "Home");
            }
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

        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody]Login login)
        {
            if (ModelState.IsValid)
            {
                Admin user = await userManager.FindByNameAsync(login.Username);
                SignInResult result = await signInManager.CheckPasswordSignInAsync(user, login.Password, false);
                
                if (result.Succeeded)
                {
                    Claim[] claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
                    };

                    SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"]));
                    SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    JwtSecurityToken token = new JwtSecurityToken(
                        config["Tokens:Issuer"],
                        config["Tokens:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(60),
                        signingCredentials: credentials
                        );

                    var results = new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    };
                    return Created("", results);
                }
            }

            return BadRequest();
        }
    }
}