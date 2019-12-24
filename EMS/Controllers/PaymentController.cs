using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly EMSContext context;

        public PaymentController(EMSContext context) {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Payment(int id)
        {
            Employee emp = context.Employee.First(e => e.Id == id);
            return View();
        }

        [HttpGet]
        public IActionResult PagesatEperfunduar(int id)
        {
            Employee emp = context.Employee.Include(emp => emp.Payments).First(e => e.Id == id);
            return View(emp);
        }

        [HttpGet]
        public IActionResult Fatura(int id)
        {
            Payment payment = context.Payment.Find(id);
            ViewData["payment"] = payment;
            return View();
        }

        [HttpPost]
        public IActionResult Payment(int id, string time)
        {   
            int minimumPayment = 80; //default minimum payment 
            DateTime endTime = DateTime.Parse(time);

            Employee employee = context.Employee.Include(employee => employee.Attendance).First(emp => emp.Id == id);
            
            List<Attendance> list = employee.Attendance.Where(atd => atd.PaymentId == null && atd.StartTime.Date <= endTime.Date && atd.EndTime != null).OrderBy(atd => atd.StartTime).ToList();

            //TimeSpan timeSpan = list.LastOrDefault().EndTime.GetValueOrDefault().Subtract(list.FirstOrDefault().StartTime);
            TimeSpan timeSpan = new TimeSpan();

            double totalPayment = 0.0;
            list.ForEach(atd =>
            {
                totalPayment += atd.payment.GetValueOrDefault();
            });

            if (list.Count != 0)
            {
                list.ForEach(atd =>
                {
                    if(atd.EndTime != null)
                    {
                        timeSpan = timeSpan.Add(atd.EndTime.Value.Subtract(atd.StartTime));
                    }
                });
            }
            else
            {
                TempData["msg"] = "Nuk ka ditë të pa paguara në këtë periudhë.";
                return View();
            }

            if (!(totalPayment <= minimumPayment))
            {
                Payment payment = new Payment
                {
                    days = list,
                    employee = employee,
                    startDate = list.FirstOrDefault().StartTime,
                    endDate = list.LastOrDefault().EndTime.GetValueOrDefault(),
                    paid = false //true after ticket is printed
                };

                

                payment.paymentBruto = (float)totalPayment;
                payment.paymentNeto = (float)(totalPayment - (totalPayment * 0.30));//-30% 
                
                employee.Payments.Add(payment);

                context.SaveChanges();

                return RedirectToAction("Fatura", new { id = payment.Id });
            }
            else
            {
                TempData["msg"] = "Par arsye ligjore ju nuk mund ti tërhiqni me pak se 80 euro.";
                return View();
            }
        }

        //Dev only method
        public IActionResult DeletePayment(int id)
        {
            Payment payment = context.Payment.Include(p => p.days).First(p => p.Id == id);
            foreach (Attendance atd in payment.days)
            {
                atd.PaymentId = null;
            }

            context.Remove(payment);

            context.SaveChanges();
            return Ok(id);
        }
    }
}