using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using static Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions;

namespace WebApplication1.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EMSContext _context;

        public EmployeeController(ILogger<HomeController> logger, EMSContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("/AllEmployees")]
        public IActionResult Allemployees()
        {
            var employees = _context.Employee.ToList<Employee>();
            ViewData["employees"] = employees;
//            ViewData["registered"] = false;
            return View();
        }

        [HttpPost("/regjistro")]
        public IActionResult Regjistro(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employee.Add(employee);
                _context.SaveChanges();
                TempData["registered"] = true;
                return RedirectToAction("Allemployees");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("/Kontrollo/{id}")]
        public IActionResult Kontrollo(int id)
        {
            var employee = _context.Employee.Include(e => e.Attendance).First(e => e.Id == id);
            ViewData["employee"] = employee;
            return View();
        }

        [HttpGet("/employee/{id}")]
        public IActionResult ShowEmployee(int id)
        {
            var employee = _context.Employee.Find(id);
            ViewData["employee"] = employee;
            ViewData["updated"] = false;
            return View();
        }

        [HttpPost("/employee/delete")]
        public IActionResult DeleteEmployee()
        {
            int id = int.Parse(HttpContext.Request.Form["id"]);
            Employee employee = _context.Employee.Find(id);
            _context.Employee.Remove(employee);
            _context.SaveChanges();
            return RedirectToAction("Allemployees");
        }

        public IActionResult ShowReports()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Update(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employee.Update(employee);
                _context.SaveChanges();
                ViewData["employee"] = employee;
                ViewData["updated"] = true;
                return View("showEmployee");   
            }
            return RedirectToAction("Index", "Home");
        }
    }
}