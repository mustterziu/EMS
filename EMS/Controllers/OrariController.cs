using System;
using System.Linq;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    public class OrariController : Controller
    {
        private readonly EMSContext _context;

        public OrariController(EMSContext context)
        {
            _context = context;
        }

        [HttpPost("/checkin")]
        public IActionResult CheckIn(int id)
        {
            Attendance attendance = new Attendance();
            attendance.StartTime = DateTime.Now;
            _context.Employee.Find(id).Attendance.Add(attendance);
            _context.SaveChanges();

            return Ok("Checked In");
        }
        
        [HttpPost("/checkout")]
        public IActionResult CheckOut(int id)
        {
            Employee employee = _context.Employee.Find(id);
            
            Attendance attendance = _context.Attendance.Where(att => att.EmpId == id && att.StartTime > DateTime.Today).ToList().First();
            attendance.EndTime = DateTime.Now;
            TimeSpan test = (TimeSpan) (attendance.EndTime - attendance.StartTime);
            double hours = test.TotalHours;
            attendance.Payment = Math.Round((hours * employee.PaymentPerHour), 3);
            _context.Attendance.Update(attendance);
            _context.SaveChanges();
            
            return Ok("Checked Out");
        }
    }
}