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
            IList<ItemForReportingDTO> IFEs = await _context.ItemsForExercise
                .Include(ife=>ife.Item)
                    .ThenInclude(i=>i.TypeItem)
                .Where(ife=>(ife.Location.Contains(itemLocation)) || itemLocation == null)
                    .Where(ife=>(ife.Item.Name.Contains(itemName)) || itemName == null)
                .OrderBy(ife=>ife.Item.Name)
                    .ThenBy(ife=>ife.Location)
                .Select(ife=>new ItemForReportingDTO(ife.Id, ife.Item.Name, ife.Location, ife.Item.Description, ife.Item.TypeItem.Name))
                .ToListAsync();
            return Ok(IFEs);
        }

    }
}
