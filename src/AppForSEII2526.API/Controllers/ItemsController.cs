using AppForSEII2526.API.DTOs.ItemDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

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

        //[HttpGet]
        //[Route("[action]")]
        //[ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        //public async Task<ActionResult> ComputeDivision(decimal op1, decimal op2)
        //{
        //    if (op2 == 0)
        //    {
        //        string error = "Op2 cannot be zero.";
        //        _logger.LogError(DateTime.Now + "Error: " + error);
        //        return BadRequest(error);
        //    }
        //    decimal result = op1 / op2;
        //    return Ok(result);
        //}

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<ItemForPurchaseDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetItemsForPurchasing(string? itemName, string? itemBrandName){
            IList<ItemForPurchaseDTO> itemsDTOS = await _context.Items
                .Include(i=>i.Brand) //para reuniones con otras tablas. ThenInclude
                .Where(i=>(
                (i.Brand.Name.Contains(itemBrandName) && i.Name == null && i.Brand.Name != null) //Right now, only this applies (both must be added)
                || (i.Name.Contains(itemName) && i.Brand.Name == null && i.Name != null)
                || (i.Name.Contains(itemName) && i.Brand.Name.Contains(itemBrandName) && i.Name != null && i.Brand.Name != null)
                )
                ) 
                .OrderBy(i=>i.Name) //OrderByDescending, ThenBy, ThenByDescending are also available
                .Select(i=>new ItemForPurchaseDTO(i.Id, i.Name, i.Brand.Name, i.Description, i.PurchasePrice, i.QuantityAvailableForPurchase))
                .ToListAsync();
            return Ok(itemsDTOS);
        }
    }
}
