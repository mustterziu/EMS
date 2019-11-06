using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class EmployeeController : Controller
    {

        [HttpGet("/AllEmployees")]
        public IActionResult Allemployees(EMSContext context)
        {
            var employees = context.Employee.ToList<Employee>();
            ViewData["employees"] = employees;
            return View();
        }

        [HttpPost("/regjistro")]
        public IActionResult Regjistro(EMSContext context)
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

            context.Employee.Add(emp);
            context.SaveChanges();

            return Redirect("/");
        }

        [HttpGet("/Kontrollo/{id}")]
        public IActionResult Kontrollo(int id, EMSContext context)
        {
            var employee = context.Employee.Find(id);
            ViewData["employee"] = employee;
            return View("Views/Home/Kontrollo.cshtml");
        }


        [HttpGet("/employee/{id}")]
        public IActionResult ShowEmployee(int id, EMSContext context)
        {
            var employee = context.Employee.Find(id);
            ViewData["employee"] = employee;
            return View();
        }

        [HttpPost("/employee/delete")]
        public IActionResult DeleteEmployee(EMSContext context)
        {
            int id = int.Parse(HttpContext.Request.Form["id"]);
            Employee employee = context.Employee.Find(id);
            context.Employee.Remove(employee);
            context.SaveChanges();

            return Redirect("/");
        }
    }
}