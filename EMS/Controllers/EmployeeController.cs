using System;
using System.Collections.Generic;
using System.Linq;
using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions;

namespace EMS.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly EMSContext context;
        private readonly UserManager<Admin> userManager;

        public EmployeeController(ILogger<HomeController> logger, EMSContext context, UserManager<Admin> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
        }

        [Authorize]
        [HttpGet("/AllEmployees")]
        public IActionResult Allemployees()
        {
            try
            {
                logger.LogDebug("Showing Allemployees()");
                List<Employee> employees = context.Employee.ToList();
                ViewData["employees"] = employees;
                return View();
            }
            catch (Exception e)
            {
                logger.LogError("Error showing AllEmployees", e);
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize]
        [HttpPost("/regjistro")]
        public IActionResult Regjistro(Employee employee)
        {
            try
            {
                logger.LogDebug("Regjistro()");
                if (ModelState.IsValid)
                {
                    context.Employee.Add(employee);
                    context.SaveChanges();
                    logger.LogDebug("New Employee was created by " + User.Identity.Name);
                    TempData["registered"] = true;
                    return RedirectToAction("Allemployees");
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                logger.LogError("Error creating new Employee");
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpPost]
        public IActionResult ShfaqKontraten(Employee employee)
        {
            try
            {
                logger.LogDebug("ShfaqKontraten()");
                if (ModelState.IsValid)
                {
                   
                    return View(employee);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                logger.LogError("Error creating new Employee");
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize]
        [HttpGet("/Kontrollo/{id}")]
        public IActionResult Kontrollo(int id)
        {
            try
            {
                logger.LogDebug("Kontrollo {id}", id);
                Employee employee = context.Employee.Include(e => e.Attendance).First(e => e.Id == id);
                ViewData["employee"] = employee;
                return View();
            }
            catch (Exception e)
            {
                logger.LogError("Error getting details for {id}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize]
        [HttpGet("/employee/{id}")]
        public IActionResult ShowEmployee(int id)
        {
            try
            {
                logger.LogDebug("ShowEmployee({id})", id);
                Employee employee = context.Employee.Find(id);
                ViewData["employee"] = employee;
                ViewData["updated"] = false;
                return View();
            }
            catch (Exception e)
            {
                logger.LogError("Error getting details for {id}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize]
        [HttpPost("/employee/delete")]
        public IActionResult DeleteEmployee()
        {
            try
            {
                logger.LogDebug("DeleteEmployee()");
                int id = int.Parse(HttpContext.Request.Form["id"]);
                Employee employee = context.Employee.Find(id);
                context.Employee.Remove(employee);
                context.SaveChanges();
                logger.LogInformation("Employee with id: {id} was deleted by {name}", employee.Id, User.Identity.Name);
                return RedirectToAction("Allemployees");
            }
            catch (Exception e)
            {
                logger.LogError("Error deleting Employee with id: {id}");
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize]
        public IActionResult ShowReports()
        {
            List<Employee> employees = null;
            ViewData["employees"] = employees;
            ViewBag.Active = "Reports";
            return View();
        }


        [Authorize]
        [HttpPost()]
        public IActionResult ShowReports(string periudha, string renditja, string orderby)
        {
            DateTime dt = DateTime.Now;
            DateTime mondayDate = DateTime.Today.AddDays(((int)(DateTime.Today.DayOfWeek) * -1) + 1);
            //var sundayDate = StartOfWeek.AddDays(-1);//DateTime.Now.Subtract(new TimeSpan((int)dt.DayOfWeek, 0, 0, 0));
            var sundayDate = mondayDate.AddDays(6);
            //sundayDate = (int)sundayDate.Day;
            var dataFillimit = "0";

            var dataPerfundimit = "0";

            switch (periudha)
            {
                case "Ditore":
                    dataFillimit = dt.Year+"-"+dt.Month+"-"+dt.Day+ " 00:00:00.0000000";

                    dataPerfundimit = dt.Year + "-" + dt.Month + "-" + dt.Day + " 23:59:59.0000000";
                    break;
                case "Javore":
              
                   
                    dataFillimit = dt.Year + "-" + dt.Month + "-" + mondayDate.Day + " 00:00:00.0000000";

                    dataPerfundimit = dt.Year + "-" + dt.Month + "-" +  sundayDate.Day + " 23:59:59.0000000";
                    break;
                case
                   "Mujore":
                    dataFillimit = dt.Year + "-" + dt.Month + "-01 00:00:00.0000000";

                    dataPerfundimit = dt.Year + "-" + dt.Month + "-30 23:59:59.0000000";
                    break;
                case
                   "Vjetore":
                    dataFillimit = dt.Year + "-01-01 00:00:00.0000000";

                    dataPerfundimit = dt.Year + "-12-31 23:59:59.0000000";
                    break;
            }

            var employees = context.EmployeeRroga.FromSqlRaw($"Select e.Id, e.FirstName , e.LastName , e.Position, sum(DATEDIFF(second , a.StartTime , a.EndTime ) / 3600 * a.Payment ) as Paga " +
                                                        "from dbo.Employee as e inner join dbo.Attendance as a on e.id = a.EmpId " +
                                                        "where a.StartTime > \'"+ dataFillimit+ "\' AND  a.EndTime < \'" + dataPerfundimit + "\' " +
                                                        " group by e.Id,  e.FirstName, e.LastName , e.Position"
                                                        ) ;
            if(orderby == "AZ")
            {
                switch (renditja)
                {
                    case "Emri":
                        employees = employees.OrderBy(e => e.FirstName);
                        break;
                    case "Mbiemri":
                        employees = employees.OrderBy(e => e.LastName);
                        break;
                    case "Pozita":
                        employees = employees.OrderBy(e => e.Position);
                        break;
                    case "Pagesa":
                        employees = employees.OrderBy(e => e.Paga);
                        break;
                }
            }else if(orderby == "ZA"){
                switch (renditja)
                {
                    case "Emri":
                        employees = employees.OrderByDescending(e => e.FirstName);
                        break;
                    case "Mbiemri":
                        employees = employees.OrderByDescending(e => e.LastName);
                        break;
                    case "Pozita":
                        employees = employees.OrderByDescending(e => e.Position);
                        break;
                    case "Pagesa":
                        employees = employees.OrderByDescending(e => e.Paga);
                        break;
                }
            }
            ViewData["employees"] = employees.ToList();
            ViewBag.Active = "Reports";
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Update(Employee employee)
        {
            try
            {
                logger.LogDebug("Update()");
                if (ModelState.IsValid)
                {
                    context.Employee.Update(employee);
                    context.Entry(employee).Property("CreatedBy").IsModified = false;
                    context.Entry(employee).Property("DateCreated").IsModified = false;
                    context.SaveChanges();
                    logger.LogDebug("Emplyee with id: {id} was updated by: {name}", employee.Id, User.Identity.Name);
                    ViewData["employee"] = employee;
                    ViewData["updated"] = true;
                    return View("showEmployee");   
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                logger.LogError("Error updating Employee with id: {id}", employee.Id);
                return RedirectToAction("Error", "Home");
            }
        }
        public IActionResult ShfaqKontraten() {
            
            return View();
          
        }
    }
}