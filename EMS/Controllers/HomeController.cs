using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly EMSContext context;
        private readonly UserManager<Admin> userManager;

        public HomeController(ILogger<HomeController> logger, EMSContext context, UserManager<Admin> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            try
            {
                ViewBag.Active = "Kryefaqja";
                int nrPunetorve = context.Employee.Count();
                int punetoretAktiv = context.Employee.Where(emp => emp.Status == true).Count();
                ViewData["nrPunetoreve"] = nrPunetorve;
                ViewData["punetoretAktiv"] = punetoretAktiv;
                List<Attendance> attendance = context.Attendance.Where(a => a.StartTime.Date == DateTime.Now.Date).Include(a => a.Emp).ToList();
                ViewData["attendance"] = attendance;

                return View();
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
                return BadRequest(e);
            }
        }

        [Authorize]
        public IActionResult GjeneroKontraten()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
