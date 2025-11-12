using AppForSEII2526.API.DTOs.ItemDTOs;
using AppForSEII2526.API.DTOs.PurchaseDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private ApplicationDbContext _context;
        private ILogger<ItemsController> _logger;

        public PurchasesController(ApplicationDbContext context, ILogger<ItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(PurchaseDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetPurchase(int id)
        {
            if (_context.Purchases == null)
            {
                _logger.LogError("Error: Purchases table does not exist.");
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Where(p => p.Id == id)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Item)
                        .ThenInclude(i => i.Brand)
                .Include(p => p.PaymentMethod)
                .Select(p => new PurchaseDetailDTO(
                    p.Id,
                    new PaymentMethodForPurchaseDetailDTO(
                        p.PaymentMethod.Id,
                        p.PaymentMethod.GetType().Name
                    ),
                    p.Street,
                    p.City,
                    p.Country,
                    p.Description,
                    p.PurchaseItems
                        .Select(pi => new ItemForPurchaseDetailDTO(
                            pi.Item.Name,
                            pi.Item.Brand.Name,
                            pi.Amount_bought,
                            pi.Price
                        ))
                        .ToList(),
                    p.Total_price
                ))
                .FirstOrDefaultAsync();

            if (purchase == null)
            {
                _logger.LogError($"Error: Purchase with id {id} does not exist.");
                return NotFound();
            }

            return Ok(purchase);
        }


    }
}
