using AppForSEII2526.API.Data; 
using AppForSEII2526.API.DTOs.ClassDTOs;
using AppForSEII2526.API.Models; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClassesController> _logger;

        public ClassesController(ApplicationDbContext context, ILogger<ClassesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<ClassForPlanDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetClassesForPlan(string? name, DateTime? date)
        {
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
    }
}