using AppForSEII2526.API.DTOs.ClassDTOs;
using AppForSEII2526.API.DTOs.PlanDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Security.Claims;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private ApplicationDbContext _context;
        private ILogger<ClassesController> _logger;

        public ClassesController(ApplicationDbContext context, ILogger<ClassesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //[HttpGet]
        //[Route("[action]")]
        //[ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        //public async Task<ActionResult> ComputeDivision(decimal op1, decimal op2)
        //{
        //    if (op2 == 0)
        //    {
        //        string error = "Division by zero is not allowed.";
        //        _logger.LogError(DateTime.Now + "Error: " + error);
        //        return BadRequest(error);
        //    }
        //    decimal result = op1 / op2;
        //    return Ok(result);
        //}

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<ClassForPlanDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetClassesForPlan(string? name, DateTime? date) {
            DateTime referenceDate = date ?? DateTime.Now;
            IList<ClassForPlanDTO> classesDTOS = await _context.Classes
                .Include(i => i.TypeItems)
                .Where(i =>
                    (string.IsNullOrEmpty(name) || i.Name.Contains(name)) &&
                    i.Date >= referenceDate
                )
                .OrderBy(i => i.Id)
                .Select(i => new ClassForPlanDTO(
                    i.Id,
                    i.Name,
                    i.Price,
                    i.TypeItems.Select(t => t.Name).ToList(),
                    i.Date
                ))
                .ToListAsync();
            return Ok(classesDTOS);
        }




        [HttpGet]
        [Route("[action]")] // http url looks like this: api/Classes/GetPlan?id=5
        [ProducesResponseType(typeof(PlanForDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetPlan(int id)
        {  
            PlanForDetailDTO? plan = await _context.Plans

                .Where(p => p.Id == id) 
                .Include(p => p.PlanItems)
                    .ThenInclude(pi => pi.Class)
                        .ThenInclude(c => c.TypeItems) // names of the types
                .Include(p => p.PaymentMethod.User) // getting name and surname
                .Select(p => new PlanForDetailDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Weeks = p.Weeks,
                    HealthIssues = p.HealthIssues,
                    TotalPrice = p.TotalPrice,
                    CreatedDate = p.CreatedDate,
                    UserFullName = p.PaymentMethod.User.Name + " " + p.PaymentMethod.User.Surname,
                    PlanItems = p.PlanItems.Select(pi => new PlanItemForDetailDTO
                    {
                        ClassId = pi.ClassId,
                        Name = pi.Class.Name,
                        Type = pi.Class.TypeItems.Select(t => t.Name).ToList(),
                        Price = pi.Price, // using the price stored in planitem
                        Date = pi.Class.Date,
                        Goal = pi.Goal
                    }).ToList()
                })
                .FirstOrDefaultAsync(); // getting the first or null


            if (plan == null)
            {
                // Mensaje de log igual que en GetRental
                _logger.LogWarning($"Warning: Plan with id {id} was not found.");
                return NotFound();
            }

            // Devolver el plan
            return Ok(plan);
        }




    }
}
