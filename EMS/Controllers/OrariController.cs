using System;
using System.Linq;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    [Route("/api/[Controller]")]
    public class OrariController : Controller
    {
        private readonly EMSContext context;

        public OrariController(EMSContext context)
        {
            this.context = context;
        }

        //TODO Double checkin fix
        [HttpPost("{id:int}")]
        public IActionResult CheckIn(int id)
        {
            Attendance attendance = new Attendance();
            attendance.StartTime = DateTime.Now;
            context.Employee.Find(id).Attendance.Add(attendance);
            context.SaveChanges();

            return Ok("Checked In");
        }
        
        //TODO fix bugs
        public IActionResult CheckOut(int id)
        {
            Employee employee = context.Employee.Find(id);
            
            Attendance attendance = context.Attendance.Where(att => att.EmpId == id && att.StartTime > DateTime.Today).ToList().First();
            attendance.EndTime = DateTime.Now;
            TimeSpan test = (TimeSpan) (attendance.EndTime - attendance.StartTime);
            double hours = test.TotalHours;
            attendance.Payment = Math.Round((hours * employee.PaymentPerHour), 3);
            context.Attendance.Update(attendance);
            context.SaveChanges();
            
            return Ok("Checked Out");
        }
    }
}