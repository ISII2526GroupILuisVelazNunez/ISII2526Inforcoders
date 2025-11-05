using AppForSEII2526.API.DTOs.IncidentDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentsController : ControllerBase
    {
        private ApplicationDbContext _context;
        private ILogger<IncidentsController> _logger;

        public IncidentsController(ApplicationDbContext context, ILogger<IncidentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IncidentDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetIncident(int id)
        {
            if (_context.Incidents == null)
            {
                _logger.LogError("Error: The database has no table 'Incidents'.");
                return NotFound();
            }

            var Incident = await _context.Incidents
                .Where(i => i.Id == id)
                    .Include(i => i.User)
                    .Include(i => i.IncidentItems)
                        .ThenInclude(ii => ii.ItemForExercise)
                            .ThenInclude(ife => ife.Item)
                                .ThenInclude(item => item.TypeItem)
                .Select(i => new IncidentDetailDTO(i.Id, i.IncidentState, i.Title, 
                        i.DateOfIdentification, i.Exercise, i.User.Name,
                        i.IncidentItems
                            .Select(ii => new IncidentItemDTO(ii.ItemForExerciseId,ii.IncidentPriority,
                            ii.ItemForExercise.Location,ii.ItemForExercise.Item.Name,
                            ii.ItemForExercise.Item.Description, ii.ItemForExercise.Item.TypeItem.Name)
                            ).ToList<IncidentItemDTO>()
                        ))
                .FirstOrDefaultAsync();

            if (Incident == null)
            {
                _logger.LogError($"Error: Rental with id {id} does not exist");
                return NotFound();
            }


            return Ok(Incident);
        }
    }
}
