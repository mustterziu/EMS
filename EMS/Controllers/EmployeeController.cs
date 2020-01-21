using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using static Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions;

namespace EMS.Controllers
{
    [Authorize]
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
                LogException(e);
                logger.LogError("Error showing AllEmployees", e);
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult ActiveEmployees()
        {
            try
            {
                logger.LogDebug("Showing ActiveEmployees()");
                List<Employee> employees = context.Employee.Where(emp => emp.Status == true).ToList();
                ViewData["employees"] = employees;
                return View();
            }
            catch (Exception e)
            {
                LogException(e);

                logger.LogError("Error showing ActiveEmployees", e);
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult OrariEmployees()
        {
            try
            {
                logger.LogDebug("Showing ActiveEmployees()");
                List<Employee> employees = context.Employee.Where(emp => emp.Status == true).ToList();
                ViewData["employees"] = employees;
                return View();
            }
            catch (Exception e)
            {
                LogException(e);

                logger.LogError("Error showing ActiveEmployees", e);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public IActionResult Regjistro(Employee employee)
        {
            try
            {
                logger.LogDebug("Regjistro()");
                if (ModelState.IsValid)
                {
                    employee.Status = true;

                    context.Entry(employee).Property("CreatedBy").IsModified = false;
                    context.Entry(employee).Property("DateCreated").IsModified = false;

                    context.Employee.Add(employee);
                    context.SaveChanges();
                    logger.LogDebug("New Employee was created by " + User.Identity.Name);
                    TempData["registered"] = true;
  
                    context.Logs.Add(new Logs { 
                        mesazhi = $"Employee {employee.FirstName} {employee.LastName} was added with id: {employee.Id}", 
                        createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value, 
                        createdAt = DateTime.Now });
                    context.SaveChanges();

                    return RedirectToAction("Allemployees");
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                LogException(e);

                logger.LogError("Error creating new Employee", e);
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult ShfaqKontraten() {
            return View();          
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
                LogException(e);

                logger.LogError("Error creating new Employee", e);
                return RedirectToAction("Error", "Home");
            }
        }

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
                LogException(e);

                logger.LogError("Error getting details for {id}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult ChangeOrari(int id)
        {
            Employee employee = context.Employee.Find(id);
            if(employee.Schedule == "Pasdite")
            {
                employee.Schedule = "Paradite";
            }
            else
            {
                employee.Schedule = "Pasdite";
            }

            context.Logs.Add(new Logs
            {
                mesazhi = $"Orari u ndryshua ne {employee.Schedule} per punetorin {employee.FirstName} me id {employee.Id}",
                createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                createdAt = DateTime.Now
            });
            context.SaveChanges();

            return RedirectToAction("OrariEmployees");
        }

        public IActionResult Update(int id)
        {
            try
            {
                logger.LogDebug("ShowEmployee({id})", id);
                Employee employee = context.Employee.Find(id);
                if (employee == null)
                {
                    return NotFound();
                }

                ViewData["updated"] = false;
                return View(employee);
            }
            catch (Exception e)
            {
                LogException(e);

                logger.LogError("Error getting details for {id}", id);
                return RedirectToAction("Error", "Home");
            }
        }
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

                    context.Logs.Add(new Logs
                    {
                        mesazhi = $"Te dhenat u ndryshuan per punetorin {employee.FirstName} me id {employee.Id}",
                        createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                        createdAt = DateTime.Now
                    });

                    context.SaveChanges();

                    logger.LogDebug("Emplyee with id: {id} was updated by: {name}", employee.Id, User.Identity.Name);
                    ViewData["updated"] = true;
                    return View(employee);   
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                LogException(e);

                logger.LogError("Error updating Employee with id: {id}", employee.Id, e);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                Employee emp = context.Employee.Find(id);
                if (emp == null)
                {
                    return BadRequest();
                }
                context.Employee.Remove(emp);
                context.SaveChanges();

                logger.LogInformation("Employee with id: {id} was fired by {name}", emp.Id, User.Identity.Name);

                return RedirectToAction("AllEmployees");
            }catch(Exception e)
            {
                LogException(e);

                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult KtheNePune(int id)
        {
            try
            {
                logger.LogDebug("KtheNePune()");
                Employee employee = context.Employee.Find(id);
                employee.Status = true;

                context.Entry(employee).Property("CreatedBy").IsModified = false;
                context.Entry(employee).Property("DateCreated").IsModified = false;

                context.Logs.Add(new Logs
                {
                    mesazhi = $"Statusi per punetorin {employee.FirstName} u ndryshua",
                    createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    createdAt = DateTime.Now
                });

                context.SaveChanges();

                logger.LogInformation("Employee with id: {id} was employed again by {name}", employee.Id, User.Identity.Name);

                return RedirectToAction("Allemployees");
            }
            catch (Exception e)
            {
                LogException(e);

                logger.LogError("Error employing again Employee with id: {id}");
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Pusho(int id)
        {
            try
            {
                logger.LogDebug("Pusho()");
                Employee employee = context.Employee.Find(id);
                employee.Status = false;

                context.Entry(employee).Property("CreatedBy").IsModified = false;
                context.Entry(employee).Property("DateCreated").IsModified = false;
              
                context.SaveChanges();

                context.Logs.Add(new Logs
                {
                    mesazhi = $"Statusi per punetorin {employee.FirstName} u ndryshua",
                    createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    createdAt = DateTime.Now
                });

                context.SaveChanges();

                logger.LogInformation("Employee with id: {id} was fired by {name}", employee.Id, User.Identity.Name);
                return RedirectToAction("Allemployees");
            }
            catch (Exception e)
            {
                LogException(e);

                logger.LogError("Error firing Employee with id: {id}");
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult ShowReports()
        {
            List<Employee> employees = null;
            ViewData["employees"] = employees;
            ViewBag.Active = "Reports";
            return View();
        }

        public IActionResult Logs()
        {
            List<Logs> logs = context.Logs.ToList(); ;
            ViewData["logs"] = logs;
            ViewBag.Active = "Logs";
            return View();
        }

        [HttpPost]
        public IActionResult ShowReports(string periudha, string renditja, string orderby , string excel)
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

            if(excel == "Po")
            {
                try
                {
                    DataTable Dt = new DataTable();
                    Dt.Columns.Add("ID", typeof(string));
                    Dt.Columns.Add("Emri", typeof(string));
                    Dt.Columns.Add("Mbiemri", typeof(string));
                    Dt.Columns.Add("Pozita", typeof(string));
                    Dt.Columns.Add("Rroga", typeof(string));

                    foreach (var emp in employees.ToList())
                    {
                        DataRow row = Dt.NewRow();
                        row[0] = emp.Id;
                        row[1] = emp.FirstName;
                        row[2] = emp.LastName;
                        row[3] = emp.Position;
                        row[4] = emp.Paga;
                        Dt.Rows.Add(row);
                    }

                    var memoryStream = new MemoryStream();

                    using (var excelPackage = new ExcelPackage(memoryStream))
                    {
                        var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                        worksheet.Cells["A1"].LoadFromDataTable(Dt, true, TableStyles.None);
                        worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                        worksheet.DefaultRowHeight = 18;

                        worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.DefaultColWidth = 20;

                        worksheet.Column(2).AutoFit();
                        byte[] data = excelPackage.GetAsByteArray();
                        return File(data, "application/octet-m", "Raportet.xlsx");
                    }
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    throw;
                }
            }
            ViewData["employees"] = employees.ToList();
            ViewBag.Active = "Reports";
            return View();
        }

        [HttpPost]
        public IActionResult DownloadReports()
        {

            List<Employee> employees = context.Employee.ToList();

            try
            {
                DataTable Dt = new DataTable();
                Dt.Columns.Add("ID", typeof(string));
                Dt.Columns.Add("Emri", typeof(string));
                Dt.Columns.Add("Mbiemri", typeof(string));
                Dt.Columns.Add("Pozita", typeof(string));
                Dt.Columns.Add("Rroga", typeof(string));

                foreach(var emp in employees)
                {
                    DataRow row = Dt.NewRow();
                    row[0] = emp.Id;
                    row[1] = emp.FirstName;
                    row[2] = emp.LastName;
                    row[3] = emp.NrBankes;
                    row[4] = emp.PaymentPerHour;
                    Dt.Rows.Add(row);
                }

                var memoryStream = new MemoryStream();

                using (var excelPackage = new ExcelPackage(memoryStream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells["A1"].LoadFromDataTable(Dt, true, TableStyles.None);
                    worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                    worksheet.DefaultRowHeight = 18;

                    worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    worksheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.DefaultColWidth = 20;

                    worksheet.Column(2).AutoFit();
                    byte[] data = excelPackage.GetAsByteArray();
                    return File(data, "application/octet-m", "Raportet.xlsx");
                }
            }
            catch(Exception ex)
            {
                LogException(ex);
                throw;
            }         
        }

        private void LogException(Exception e)
        {
            context.Logs.Add(new Logs
            {
                mesazhi = e.Message,
                createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                createdAt = DateTime.Now
            });
            context.SaveChanges();
        }
    }
}