using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    public class PaymentController : Controller
    {
        private readonly EMSContext context;

        public PaymentController(EMSContext context) {
            this.context = context;
        }


        [Authorize]
        [HttpGet]
        public IActionResult Payment(int id)
        {
            Employee emp = context.Employee.First(e => e.Id == id);
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult PagesatEperfunduar(int id)
        {
            Employee emp = context.Employee.First(e => e.Id == id);
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Fatura()
        {
            return View();
        }
    }
}