using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(EMSContext context)
        {

            
            var attendance = context.Attendance.Where(a => a.StartTime >= DateTime.Parse("2019-10-25 00:00:00.000") && a.StartTime <= DateTime.Parse("2019-10-25 23:59:59.000")).Include(a => a.Emp).ToList();
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
