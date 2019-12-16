using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    [Route("api/Employee")]
    [ApiController]
    public class EmployeeRestController : ControllerBase
    {
        private readonly EMSContext context;

        public EmployeeRestController(EMSContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult<List<Employee>> Get()
        {
            return Ok(context.Employee.ToList());
        }

        [HttpPost]
        public ActionResult Post([FromBody]Employee employee)
        {
            if (ModelState.IsValid)
            {
                Employee emp = context.Employee.Add(employee).Entity;
                context.DefaultSaveChanges();
                return Ok("employee with id: " + emp.Id + " was added");
            } 
            else
            {
                return BadRequest("Ju lutem plotsoni fushat ne menyren e sakte!");
            }
        }

        [HttpPut]
        public ActionResult Put([FromBody]Employee employee)
        {
            var emp = context.Update(employee).Entity;
            context.DefaultSaveChanges();
            return Ok(emp);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            Employee employee = context.Employee.Find(int.Parse(id));
            context.Employee.Remove(employee);
            context.DefaultSaveChanges();
            return Ok(employee);
        }
    }
}