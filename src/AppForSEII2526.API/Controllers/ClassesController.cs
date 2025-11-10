using AppForSEII2526.API.DTOs.ClassDTOs;
using AppForSEII2526.API.DTOs.PlanDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;

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




        [HttpPost("plans")]
        [ProducesResponseType(typeof(PlanForDetailDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PlanForDetailDTO>> CreatePlan([FromBody] PlanForCreateDTO planForCreateDTO)
        {
            //alt flow 4 at least 1 item
            if (planForCreateDTO.Items == null || !planForCreateDTO.Items.Any())
            {
                return BadRequest("At least 1 class must be selected for the plan.");
            }

            // payment method existence check
            var paymentMethod = await _context.PaymentMethods.FindAsync(planForCreateDTO.PaymentMethodId);
            if (paymentMethod == null)
            {
                return NotFound($"Payment method with ID {planForCreateDTO.PaymentMethodId} was not found.");
            }

            // creating plan entity
            var plan = new Plan
            {
                Name = planForCreateDTO.Name,
                Description = planForCreateDTO.Description,
                Weeks = planForCreateDTO.Weeks,
                HealthIssues = planForCreateDTO.HealthIssues,
                CreatedDate = DateTime.UtcNow, 
                PaymentMethod = paymentMethod, 
                PlanItems = new List<PlanItem>() 
            };

            decimal totalPrice = 0;

            // processing each selected class
            foreach (var itemDTO in planForCreateDTO.Items)
            {
                var classToEnroll = await _context.Classes.FindAsync(itemDTO.ClassId);

                if (classToEnroll == null)
                {
                    return NotFound($"Class with ID {itemDTO.ClassId} does not exist.");
                }

                // checking capaticy
                var currentEnrollments = await _context.PlanItems.CountAsync(pi => pi.ClassId == classToEnroll.Id);
                if (currentEnrollments >= classToEnroll.Capacity)
                {
                    return Conflict($"Class '{classToEnroll.Name}' does not have enough capacity. Choose another one.");
                }

                // price update
                totalPrice += classToEnroll.Price;

                // planitem entity creation
                var planItem = new PlanItem
                {
                    ClassId = classToEnroll.Id,
                    Goal = itemDTO.Goal,
                    Price = classToEnroll.Price
                };

                plan.PlanItems.Add(planItem);
            }

            // total price
            plan.TotalPrice = totalPrice;

            // ddbb save
            _context.Plans.Add(plan);
            await _context.SaveChangesAsync();

            // response preparation
            // loading the plan again
            var savedPlan = await _context.Plans
                // including payment method and user to get user name and surname later on
                .Include(p => p.PaymentMethod)
                    .ThenInclude(pm => pm.User)
                // planitems and classes
                .Include(p => p.PlanItems)
                    .ThenInclude(pi => pi.Class)
                        // typeitems for class type names
                        .ThenInclude(c => c.TypeItems)
                .FirstOrDefaultAsync(p => p.Id == plan.Id);

            if (savedPlan == null)
            {
                return StatusCode(500, "Error saving the plan.");
            }

            // mapping plan to PlanForDetailDTO to get details in response
            var planDetailDTO = new PlanForDetailDTO
            {
                // planfordetaildto properties
                Id = savedPlan.Id,
                Name = savedPlan.Name,
                Description = savedPlan.Description,
                Weeks = savedPlan.Weeks,
                HealthIssues = savedPlan.HealthIssues,
                TotalPrice = savedPlan.TotalPrice,
                CreatedDate = savedPlan.CreatedDate,

                UserFullName = $"{savedPlan.PaymentMethod?.User?.Name} {savedPlan.PaymentMethod?.User?.Surname}".Trim(),

                PlanItems = savedPlan.PlanItems.Select(pi => new PlanItemForDetailDTO
                {
                    // planitemfordetaildto properties
                    ClassId = pi.ClassId,
                    Name = pi.Class.Name,

                    
                    Type = pi.Class.TypeItems.Select(t => t.Name).ToList(),

                    
                    Price = pi.Price,

                    
                    Date = pi.Class.Date,

                    Goal = pi.Goal

                }).ToList()
            };

            // response, 201, and details of the created plan
            return CreatedAtAction(nameof(GetPlan), new { id = plan.Id }, planDetailDTO);
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
                _logger.LogWarning($"Warning: Plan with id {id} was not found.");
                return NotFound();
            }

            // returning plan
            return Ok(plan);
        }




    }
}
