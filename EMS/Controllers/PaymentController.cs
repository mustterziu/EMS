using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
            try
            {
                Employee emp = context.Employee
                .Include(e => e.Payments)
                .First(emp => emp.Id == id);

                List<Attendance> days = context.Attendance.Where(att => att.Emp == emp && att.EndTime != null && att.Payment == null)
                    .OrderBy(att => att.StartTime)
                    .ToList();

                emp.Attendance = days;

                if (days.Count > 0)
                {
                    ViewData["maxDate"] = days.Last().EndTime.GetValueOrDefault();
                }
                else
                {
                    ViewData["maxDate"] = null;
                }

                if (emp.Payments.Count > 0)
                {
                    ViewData["LastPayment"] = emp.Payments.Last();
                }
                else
                {
                    ViewData["LastPayment"] = null;
                }

                ViewData["employee"] = emp;
                return View();
            }
            catch(ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception)
            {                
                return BadRequest();
            }
            
        }

        [HttpGet]
        public IActionResult PagesatEperfunduar(int id)
        {
            try
            {
                Employee emp = context.Employee.Include(emp => emp.Payments).First(e => e.Id == id);
                return View(emp);
            }catch(ArgumentNullException)
            {
                return NotFound();
            }catch(Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult Fatura(int id)
        {
            try
            {
                Payment payment = context.Payment.Include(p => p.employee).First(p => p.Id == id);
                ViewData["payment"] = payment;
                return View();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }catch(Exception)
            {
                return BadRequest();
            }            
        }

        [HttpPost]
        public IActionResult Payment(int id, string time)
        {
            try
            {
                int minimumPayment = 80; 
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
                        if (atd.EndTime != null)
                        {
                            timeSpan = timeSpan.Add(atd.EndTime.Value.Subtract(atd.StartTime));
                        }
                    });
                }
                else
                {
                    TempData["msg"] = "Nuk ka ditë të pa paguara në këtë periudhë.";
                    return RedirectToAction("Fatura", new { id = employee.Id });
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
                    TempData["msg"] = "Për arsye ligjore ju nuk mund ti tërhiqni me pak se 80 euro.";
                    return RedirectToAction("Payment", new { id = employee.Id});
                }
            }
            catch(ArgumentNullException)
            {
                return NotFound();
            }catch(Exception)
            {
                return BadRequest();
            }            
        }

        [HttpPost]
        public IActionResult ConfirmPayment(int id)
        {
            try
            {
                Payment payment = context.Payment.Find(id);
                payment.paid = true;
                payment.paymentDate = DateTime.Now;
                context.SaveChanges();

                return RedirectToAction("PagesatEperfunduar", new { id = payment.employeeId });
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }catch(Exception e)
            {
                return BadRequest(e);
            }            
        }

        [HttpPost]
        public IActionResult CancelPayment(int id)
        {
            try
            {
                Payment payment = context.Payment
                .Include(p => p.days)
                .Include(p => p.employee)
                .First(p => p.Id == id);
                foreach (Attendance atd in payment.days)
                {
                    atd.PaymentId = null;
                }

                context.Remove(payment);
                context.SaveChanges();

                TempData["msg"] = $"Pagesa për punëtorin {payment.employee.FirstName} {payment.employee.LastName} u anullua!";
                return RedirectToAction("Payment", new { id = payment.employeeId });
            }
            catch(ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e);
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