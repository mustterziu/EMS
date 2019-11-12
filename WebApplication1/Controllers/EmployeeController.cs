using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;

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
            return View();
        }

        [HttpPost("/regjistro")]
        public IActionResult Regjistro()
        {
            Employee emp = new Employee();

            emp.FirstName = HttpContext.Request.Form["firstName"];
            emp.LastName = HttpContext.Request.Form["lastName"];
            emp.PhoneNumber = int.Parse(HttpContext.Request.Form["phoneNumber"]);
            emp.Gender = HttpContext.Request.Form["gender"];
            emp.City = HttpContext.Request.Form["city"];
            emp.State = HttpContext.Request.Form["state"];
            emp.Position = HttpContext.Request.Form["position"];
            emp.Schedule = HttpContext.Request.Form["schedule"];

            _context.Employee.Add(emp);
            _context.SaveChanges();

            return Redirect("/");
        }

        [HttpGet("/Kontrollo/{id}")]
        public IActionResult Kontrollo(int id)
        {
            var employee = _context.Employee.Find(id);
            ViewData["employee"] = employee;
            return View("Views/Home/Kontrollo.cshtml");
        }


        [HttpGet("/employee/{id}")]
        public IActionResult ShowEmployee(int id)
        {
            var employee = _context.Employee.Find(id);
            ViewData["employee"] = employee;
            return View();
        }

        [HttpPost("/employee/delete")]
        public IActionResult DeleteEmployee()
        {
            int id = int.Parse(HttpContext.Request.Form["id"]);
            Employee employee = _context.Employee.Find(id);
            _context.Employee.Remove(employee);
            _context.SaveChanges();
            return Redirect("/");
        }

        public IActionResult ShowReports()
        {
            return View();
        }
    }
}