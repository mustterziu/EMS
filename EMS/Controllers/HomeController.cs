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
            ViewBag.Active = "Kryefaqja";
            int nrPunetorve = context.Employee.Count();
            ViewData["nrPunetoreve"] = nrPunetorve;
            List<Attendance> attendance = context.Attendance.Where(a => a.StartTime >= DateTime.Parse("2019-10-25 00:00:00.000") && a.StartTime <= DateTime.Parse("2019-10-25 23:59:59.000")).Include(a => a.Emp).ToList();
            ViewData["attendance"] = attendance;

            return View();
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
