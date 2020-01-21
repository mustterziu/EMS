using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EMS.Models;
using EMS.Models.ViewModels;
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
        private readonly IMapper mapper;

        public EmployeeRestController(EMSContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<Employee>> Get()
        {
            try
            {
                List<Employee> employees = context.Employee.ToList();
                return Ok(mapper.Map<List<EmployeeResponse>>(employees));
            }
            catch (Exception e)
            {
                context.Logs.Add(new Logs { mesazhi = e.Message, createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value, createdAt = DateTime.Now });
                context.SaveChanges();
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Employee> Get(int id)
        {
            try
            {
                Employee employee = context.Employee.Find(id);
                if (employee != null)
                {
                    return Ok(mapper.Map<EmployeeResponse>(employee));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                context.Logs.Add(new Logs { mesazhi = e.Message, createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value, createdAt = DateTime.Now });
                context.SaveChanges();
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody]Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Employee emp = context.Employee.Add(employee).Entity;
                    emp.Status = true;
                    context.SaveChanges();
                    context.Logs.Add(new Logs { mesazhi = $"Employee {emp.FirstName} {emp.LastName} was created with id: {emp.Id}", createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value, createdAt = DateTime.Now });
                    context.SaveChanges();
                    return Ok("employee with id: " + emp.Id + " was added");
                }
                else
                {
                    return BadRequest("Ju lutem plotsoni fushat ne menyren e sakte!");
                }
            }
            catch (Exception e)
            {
                context.Logs.Add(new Logs { mesazhi = e.Message, createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value, createdAt = DateTime.Now });
                context.SaveChanges();
                return BadRequest();
            }
            
        }

        [HttpPut]
        public ActionResult Put([FromBody]Employee employee)
        {
            try
            {
                var emp = context.Update(employee).Entity;
                context.SaveChanges();
                return Ok(mapper.Map<EmployeeResponse>(emp));
            }
            catch (Exception e)
            {
                context.Logs.Add(new Logs { mesazhi = e.Message, createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value, createdAt = DateTime.Now });
                context.SaveChanges();
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                Employee employee = context.Employee.Find(id);
                if (employee == null)
                {
                    return NotFound();
                }
                context.Employee.Remove(employee);
                context.SaveChanges();
                return Ok(new { msg = "Employee was deleted sucessfuly", employee = mapper.Map<EmployeeResponse>(employee) });
            }
            catch (Exception e)
            {
                context.Logs.Add(new Logs { mesazhi = e.Message, createdAt = DateTime.Now, createdBy = User.FindFirst(ClaimTypes.NameIdentifier).Value });
                context.SaveChanges();
                return BadRequest();
            }            
        }
    }
}