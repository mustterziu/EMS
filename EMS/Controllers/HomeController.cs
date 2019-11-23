using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EMSContext _context;

        public HomeController(ILogger<HomeController> logger, EMSContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Active = "Kryefaqja";
            int nrPunetorve = _context.Employee.Count();
            ViewData["nrPunetoreve"] = nrPunetorve;
            List<Attendance> attendance = _context.Attendance.Where(a => a.StartTime >= DateTime.Parse("2019-10-25 00:00:00.000") && a.StartTime <= DateTime.Parse("2019-10-25 23:59:59.000")).Include(a => a.Emp).ToList();
            ViewData["attendance"] = attendance;

            return View();
        }

        [HttpGet("/GjeneroNew")]
        [Route("Home/GjeneroKontraten")]
        public IActionResult GjeneroKontraten()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
