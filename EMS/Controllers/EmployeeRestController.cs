using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    [Route("api/Employee")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpGet("{id}")]
        public ActionResult<Employee> Get(int id)
        {
            Employee employee = context.Employee.Find(id);
            if (employee != null)
            {
                return Ok(employee);
            }
            else
            {
                return NotFound();
            }
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