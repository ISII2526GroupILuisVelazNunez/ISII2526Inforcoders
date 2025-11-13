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
            // Validation and save
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            return Ok(purchase);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(PurchaseDetailDTO), (int)HttpStatusCode.Created)]
        public async Task<ActionResult> CreatePurchase(PurchaseForCreateDTO purchaseForCreate)
        {
            // --- Validate required fields ---
            if (string.IsNullOrWhiteSpace(purchaseForCreate.Street))
                ModelState.AddModelError("Street", "Error! You must specify a street.");

            if (string.IsNullOrWhiteSpace(purchaseForCreate.City))
                ModelState.AddModelError("City", "Error! You must specify a city.");

            if (string.IsNullOrWhiteSpace(purchaseForCreate.Country))
                ModelState.AddModelError("Country", "Error! You must specify a country.");

            if (purchaseForCreate.PurchaseItems == null || !purchaseForCreate.PurchaseItems.Any())
                ModelState.AddModelError("PurchaseItems", "Error! You must include at least one item to be purchased.");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // --- Get user's payment method ---
            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == purchaseForCreate.PaymentMethodId);
            if (paymentMethod == null)
            {
                ModelState.AddModelError("PaymentMethodId", "Error! Selected payment method does not exist.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // --- Fetch items from DB ---
            var itemIds = purchaseForCreate.PurchaseItems.Select(pi => pi.Id).ToList();
            var items = await _context.Items
                .Include(i => i.Brand)
                .Where(i => itemIds.Contains(i.Id))
                .ToListAsync();

            // --- Create the purchase entity ---
            var purchase = new Purchase(
                purchaseForCreate.City,
                purchaseForCreate.Country,
                DateTime.Now,
                purchaseForCreate.Description,
                purchaseForCreate.Street
            );

            purchase.PaymentMethod = paymentMethod;

            //process all of the selected items
            foreach (var selectedItem in purchaseForCreate.PurchaseItems)
            {
                var dbItem = items.FirstOrDefault(i => i.Id == selectedItem.Id);
                if (dbItem == null)
                {
                    ModelState.AddModelError("PurchaseItems", $"Item '{selectedItem.Name}' does not exist.");
                    continue;
                }

                if (selectedItem.QuantityToBuy > dbItem.QuantityAvailableForPurchase)
                {
                    ModelState.AddModelError("PurchaseItems",
                        $"Requested {selectedItem.QuantityToBuy} of '{dbItem.Name}', but only {dbItem.QuantityAvailableForPurchase} available.");
                    continue;
                }

                //create purchaseItem
                var purchaseItem = new PurchaseItem(
                    dbItem.Id,
                    selectedItem.QuantityAvailableForPurchase,
                    selectedItem.QuantityToBuy,
                    dbItem.PurchasePrice,
                    purchase.Id);

                purchase.PurchaseItems.Add(purchaseItem);

                // Reduce stock
                dbItem.QuantityAvailableForPurchase -= selectedItem.QuantityToBuy;
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            //total_price calculation
            purchase.Total_price = purchase.PurchaseItems.Sum(pi => pi.Price * pi.Quantity);

            //save
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            //the return DTO
            var purchaseDetail = new PurchaseDetailDTO(
                purchase.Id,
                new PaymentMethodForPurchaseDetailDTO(paymentMethod.Id, paymentMethod.GetType().Name),
                purchase.Street,
                purchase.City,
                purchase.Country,
                purchase.Description,
                purchase.PurchaseItems.Select(pi => new ItemForPurchaseDetailDTO(
                    pi.Item.Name,
                    pi.Item.Brand.Name,
                    pi.Amount_bought,
                    pi.Price
                )).ToList(),
                purchase.Total_price
            );

            return CreatedAtAction("GetPurchase", new { id = purchase.Id }, purchaseDetail);
        }




    }
}