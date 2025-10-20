using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore.Query.Internal;
using AppForSEII2526.API.DTOs.ClassDTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
            IList<ClassForPlanDTO> classesDTOS = await _context.Classes
                .Include(i => i.TypeItems)
                .Where(i =>
                    (string.IsNullOrEmpty(name) || i.Name.Contains(name)) &&
                    (
                        (date == null && i.Date.Date >= DateTime.Today) ||
                        (date != null && i.Date.Date == date.Value.Date)
                    )
                )
                .OrderByDescending(i => i.Name)
                .Select(i => new ClassForPlanDTO(i.Id, i.Name, i.Price, (IList<string>)i.TypeItems, i.Date))
                .ToListAsync();
            return Ok(classesDTOS);
        }
    }
}
