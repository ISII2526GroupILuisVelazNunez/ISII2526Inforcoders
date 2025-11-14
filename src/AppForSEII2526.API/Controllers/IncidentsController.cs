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

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(IncidentDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateIncident(IncidentForCreateDTO incidentForCreate)
        {
            if (incidentForCreate.IncidentItems.Count == 0)
                ModelState.AddModelError("IncidentItems","Error: you must select at least one item to report!");

            if (incidentForCreate.DateOfIdentification >= DateTime.Now)
                ModelState.AddModelError("DateOfIdentification", "Error: the date of identification can't be in the future!");

            var reporterNameSplit = incidentForCreate.ReporterName.Split(' '); // Separate the reporter's first and last name
            if (reporterNameSplit.Length != 2)
                ModelState.AddModelError("IncidentApplicationUser", "Error! Please write the reporter's full name using 2 words");

            var user = new ApplicationUser(reporterNameSplit[0], reporterNameSplit[1]);

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var ifeLocations = incidentForCreate.IncidentItems.Select(ii => ii.Location).ToList<string>();

            var IFEs = _context.ItemsForExercise.Include(ife => ife.IncidentItems)
                .ThenInclude(ii => ii.Incident)
                .Where(ife => ifeLocations.Contains(ife.Location))
                .Select(ife => new {
                    ife.Id, ife.Location, NumberOfItems = ife.IncidentItems.Count
                })

                .ToList();

            var incident = new Incident(incidentForCreate.Title, "", incidentForCreate.Exercise, 
                incidentForCreate.DateOfIdentification, new List<IncidentItem>(), incidentForCreate.IncidentState, user);

            foreach(var item in incidentForCreate.IncidentItems)
            {
                var IFE = IFEs.FirstOrDefault(ife => ife.Location == item.Location);
                incident.IncidentItems.Add(new IncidentItem(incident, 
                    new ItemForExercise(IFE.Location, 
                        new Item(item.Description))));
            }

            _context.Add(incident);

            try {
                await _context.SaveChangesAsync();
            } catch (Exception e) {
                _logger.LogError(e.Message);
                ModelState.AddModelError("Incident", "Error! There was a problem while recording the incident report... Try again later");
                return Conflict("Error" + e.Message);
            }

            var incidentDetail = new IncidentDetailDTO(incident.IncidentState, incident.Title,
                incident.DateOfIdentification, incident.Exercise, incidentForCreate.ReporterName,
                incidentForCreate.IncidentItems);

            return CreatedAtAction("GetIncident", new { id = incident.Id }, incidentDetail);
        }
    }
}
