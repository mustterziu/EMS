using System;
using System.Linq;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    [Route("api/[Controller]/[Action]")]
    public class OrariController : Controller
    {
        private readonly EMSContext context;

        public OrariController(EMSContext context)
        {
            this.context = context;
        }

        public IActionResult Test()
        {
            return Ok("Test");
        }

        [HttpPost]
        public IActionResult CheckIn([FromBody] Params input)
        {
            try
            {
                Attendance attCheck = context.Attendance.Where(att => 
                    att.EmpId == input.id && att.StartTime > DateTime.Today && att.EndTime == null)
                    .ToList().First();

                if (attCheck.EndTime == null)
                {
                    return BadRequest("Already checked in today.");
                }
            }
            catch (InvalidOperationException e)
            {
                Attendance attendance = new Attendance
                {
                    StartTime = DateTime.Now
                };
                try
                {
                    context.Employee.Find(input.id).Attendance.Add(attendance);
                    context.DefaultSaveChanges();

                    return Ok("Checked In");
                }
                catch(NullReferenceException ex)
                {
                    return NotFound($"Employee with id: {input.id} was not found");
                }
            }
            return BadRequest("Something went wrong");
        }

        [HttpPost]
        public IActionResult CheckOut([FromBody]Params input)
        {
            Employee employee = context.Employee.Find(input.id);
            if (employee != null)
            {
                try
                {
                    Attendance attendance = context.Attendance.Where(att => 
                        att.EmpId == input.id && att.StartTime > DateTime.Today && att.EndTime == null)
                        .ToList().First();

                    attendance.EndTime = DateTime.Now;
                    TimeSpan test = (TimeSpan)(attendance.EndTime - attendance.StartTime);
                    double hours = test.TotalHours;
                    attendance.payment = Math.Round((hours * employee.PaymentPerHour), 3);
                    context.Attendance.Update(attendance);
                    context.DefaultSaveChanges();

                    return Ok(new CheckOutResponse
                    {
                        message = "Checked out sucessfuly",
                        startTime = attendance.StartTime.ToShortTimeString(),
                        endTime = attendance.EndTime?.ToShortTimeString(),
                        payment = attendance.payment.GetValueOrDefault(),
                    });
                }catch(InvalidOperationException e)
                {
                    return BadRequest("You must check in first");
                }
            }
            else
            {
                return NotFound($"Employee with id: \"{input.id}\" was not found");
            }
        }

        public class CheckOutResponse
        {
            public string message { get; set; }
            public string startTime { get; set; }
            public string endTime { get; set; }
            public double payment { get; set; }
        }

        public class Params
        {
            public int id { get; set; }
        }
    }    
}