using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppForSEII2526.API.DTOs.PurchaseDTOs;
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

        [HttpPost]
        [Route("[action]")]

        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(PurchaseDetailDTO), (int)HttpStatusCode.Created)]

        public async Task<ActionResult> CreatePurchase(PurchaseForCreateDTO purchaseForCreate)
        {
            //any validation defined in PurchaseForCreate is checked before running the method so they don't have to be checked again
            if (string.IsNullOrWhiteSpace(purchaseForCreate.Street))
                ModelState.AddModelError("Street", "Error! You must specify a street.");

            if (string.IsNullOrWhiteSpace(purchaseForCreate.City))
                ModelState.AddModelError("City", "Error! You must specify a city.");

            if (string.IsNullOrWhiteSpace(purchaseForCreate.Country))
                ModelState.AddModelError("Country", "Error! You must specify a country.");

            if (purchaseForCreate.PurchaseItems == null || !purchaseForCreate.PurchaseItems.Any())
                ModelState.AddModelError("PurchaseItems", "Error! You must include at least one item to be purchased.");


            //we must relate the Purchase to the User TODO
            //var user = await _context.Users.FirstOrDefaultAsync(au => au.UserName == rentalForCreate.UserNameCustomer);
            //if (user == null)
            //    ModelState.AddModelError("RentalApplicationUser", "Error! UserName is not registered");

            //we must check that all the items to be purchased exist in the database
            var itemNames = purchaseForCreate.PurchaseItems.Select(pi => pi.Item.Name).ToList<string>();

            var items = await _context.Items
                .Where(i => itemNames.Contains(i.Name))
                .ToListAsync();

            //we must provide purchase with the info to be saved in the database
            Purchase purchase = new Purchase(purchaseForCreate.Id, purchaseForCreate.City,
                purchaseForCreate.Country, DateTime.Now, purchaseForCreate.Description, purchaseForCreate.Street,
                new List<PurchaseItem>(), 0, purchaseForCreate.PaymentMethod); //THE 0 IS TEMPORARY!!




            foreach (var reqItem in purchaseForCreate.PurchaseItems)
            {
                var dbItem = items.FirstOrDefault(i => i.Name == reqItem.Item.Name);
                //we must check that there is enough quantity to be rented in the database
                if (dbItem == null)
                {
                    ModelState.AddModelError("PurchaseItems", $"Error! Item '{reqItem.Item.Name}' does not exist.");
                }

                if (reqItem.Quantity > dbItem.QuantityAvailableForPurchase)
                {
                    ModelState.AddModelError("PurchaseItems",
                        $"Error! You requested {reqItem.Quantity} of '{dbItem.Name}', but only {dbItem.QuantityAvailableForPurchase} are available.");
                }

                else //TODO
                {
                    // Add valid item to purchase
                    var purchaseItem = new PurchaseItem(
                        dbItem.Id,
                        dbItem,
                        reqItem.Quantity,
                        reqItem.Quantity,
                        dbItem.PurchasePrice,
                        purchase,
                        0 //temporary ID??
                    );
                    purchase.PurchaseItems.Add(purchaseItem);

                    // Update item stock
                    dbItem.QuantityAvailableForPurchase -= reqItem.Quantity;
                    purchase.Total_price = purchase.PurchaseItems.Sum(pi => pi.Price * pi.Quantity);
                }
                // Validation and save
                if (ModelState.ErrorCount > 0)
                    return BadRequest(new ValidationProblemDetails(ModelState));

                _context.Add(purchase);
                await _context.SaveChangesAsync();

                // Prepare return DTO
                var purchaseDetail = new PurchaseDetailDTO(purchase.Id, purchase.PaymentMethod,
                purchase.Street, purchase.City, purchase.Country, purchase.Description,
                purchase.PurchaseItems, purchase.Total_price);

                return CreatedAtAction("GetPurchase", new { id = purchase.Id }, purchaseDetail);
            }
        }
    }
}
