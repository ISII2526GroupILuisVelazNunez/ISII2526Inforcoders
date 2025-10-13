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


    }
}
