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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // for try-catch
        public async Task<ActionResult<PlanForDetailDTO>> CreatePlan([FromBody] PlanForCreateDTO planForCreateDTO)
        {
            try
            {
                // at least one item
                if (planForCreateDTO.Items == null || !planForCreateDTO.Items.Any())
                {
                    return BadRequest("At least 1 class must be selected for the plan.");
                }

                // payment method needs to exist
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

                // process each selected class
                foreach (var itemDTO in planForCreateDTO.Items)
                {
                    var classToEnroll = await _context.Classes.FindAsync(itemDTO.ClassId);

                    if (classToEnroll == null)
                    {
                        return NotFound($"Class with ID {itemDTO.ClassId} does not exist.");
                    }

                    // validate capacity
                    var currentEnrollments = await _context.PlanItems.CountAsync(pi => pi.ClassId == classToEnroll.Id);
                    if (currentEnrollments >= classToEnroll.Capacity)
                    {
                        return Conflict($"Class '{classToEnroll.Name}' does not have enough capacity. Please modify your selection.");
                    }

                    // add price each class
                    totalPrice += classToEnroll.Price;

                    // create PlanItem entity
                    var planItem = new PlanItem
                    {
                        ClassId = classToEnroll.Id,
                        Goal = itemDTO.Goal, 
                        Price = classToEnroll.Price // storing price at the time of purchase
                    };

                    plan.PlanItems.Add(planItem);
                }

                // total price calculation
                plan.TotalPrice = totalPrice;

                // saving to db
                _context.Plans.Add(plan);
                await _context.SaveChangesAsync();

                // response preparation
                // reload the plan with all required navigation properties
                var savedPlan = await _context.Plans
                    .Include(p => p.PaymentMethod)
                        .ThenInclude(pm => pm.User) // for UserFullName
                    .Include(p => p.PlanItems)
                        .ThenInclude(pi => pi.Class)
                            .ThenInclude(c => c.TypeItems) // for class type names
                    .FirstOrDefaultAsync(p => p.Id == plan.Id);

                if (savedPlan == null)
                {
                    // should not happen, but it's a critical server error if it does
                    _logger.LogError("The plan was saved (ID: {PlanId}) but could not be reloaded from the DB.", plan.Id);
                    return StatusCode(500, "Error processing the plan after saving.");
                }

                // Map the entity to the PlanForDetailDTO
                var planDetailDTO = new PlanForDetailDTO
                {
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
                        ClassId = pi.ClassId,
                        Name = pi.Class.Name,
                        Type = pi.Class.TypeItems.Select(t => t.Name).ToList(),
                        Price = pi.Price, // using the price stored in PlanItem
                        Date = pi.Class.Date,
                        Goal = pi.Goal
                    }).ToList()
                };

                // returning 201 created with the plan details
                return CreatedAtAction(nameof(GetPlan), new { id = plan.Id }, planDetailDTO);
            }
            // database-specific save errors
            catch (DbUpdateException ex)
            {
                string error = "An error occurred while saving to the database.";

                // specific SQL error if available
                if (ex.InnerException != null)
                {
                    error = ex.InnerException.Message;
                }

                _logger.LogError(ex, "DbUpdateException in CreatePlan: {ErrorMessage}", error);

                // returning 500 here
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error while saving: {error}");
            }
            // all other unexpected errors
            catch (Exception ex)
            {
                string error = "An unexpected error occurred on the server.";
                _logger.LogError(ex, "Unexpected error in CreatePlan: {ErrorMessage}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, error);
            }
        }

        /*
         * test json for the post method
         * {
              "paymentMethodId": 1,
              "name": "test plan1",
              "description": "Mi plan con varias clases",
              "weeks": 4,
              "healthIssues": "Ninguno",
              "items": [
                {
                  "classId": 1,
                  "goal": "Probar Yoga"
                },
                {
                  "classId": 2,
                  "goal": "Mejorar cardio en Spinning"
                }
              ]
            }
         */











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
