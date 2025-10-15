using AppForSEII2526.API.DTOs.ItemForExerciseDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private ApplicationDbContext _context;
        private ILogger<ItemsController> _logger;

        public ItemsController(ApplicationDbContext context, ILogger<ItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /*
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(decimal), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ComputeDivision(decimal op1, decimal op2) 
        {
            if (op2 == 0)
            {
                string error = "Error: Division by 0!";
                _logger.LogError(DateTime.Now + " - " + error);
                return BadRequest(error);
            }
            decimal result = op1 / op2;
            return Ok(result);
        }
        */

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<ItemForReportingDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetItemsForReporting(string? itemName, string? itemLocation)
        {
            IList<ItemForReportingDTO> items = await _context.ItemsForExercise
                .Where(i=>i.Location.Contains(itemLocation) 
                    || itemLocation == null)
                .OrderBy(i=>i.Id)
                    .ThenBy(i=>i.Location)
                .Select(i=>new ItemForReportingDTO(i.Id, i.Location, i.Item.Name))
                .ToListAsync();
            return Ok(items);
        }

    }
}
