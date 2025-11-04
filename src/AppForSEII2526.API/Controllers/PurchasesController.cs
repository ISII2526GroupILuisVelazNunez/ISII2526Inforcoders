using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(typeof(PurchaseForDetailDTO), (int)HttpStatusCode.Created)]

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
            var itemNames = purchaseForCreate.PurchaseItems.Select(pi => pi.Name).ToList<string>();

            var items = _context.Items.Include(i => i.PurchaseItems)
                .ThenInclude(pi => pi.Purchase)

                //we must check that all the items to be purchased exist in the database
                .Where(i => itemNames.Contains(i.Name))

                //we use an anonymous type https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types
                .Select(i => new {
                    i.Id,
                    i.Name,
                    i.QuantityAvailableForPurchase,
                    i.PurchasePrice
                })
                .ToList();

            //we must provide purchase with the info to be saved in the database
            Purchase purchase = new Purchase(purchaseForCreate.Id, purchaseForCreate.City,
                purchaseForCreate.Country, DateTime.Now, purchaseForCreate.Description, purchaseForCreate.Street,
                new List<PurchaseItem>(), 0, purchaseForCreate.PaymentMethod); //THE 0 IS TEMPORARY!!




            foreach (var reqItem in purchaseForCreate.PurchaseItems)
            {
                var dbItem = items.FirstOrDefault(i => i.Name == reqItem.Name);
                //we must check that there is enough quantity to be rented in the database
                if (dbItem == null)
                {
                    ModelState.AddModelError("PurchaseItems", $"Error! Item '{reqItem.Name}' does not exist.");
                }

                if (reqItem.Quantity > dbItem.QuantityAvailableForPurchase)
                {
                    ModelState.AddModelError("PurchaseItems",
                        $"Error! You requested {reqItem.Quantity} of '{dbItem.Name}', but only {dbItem.QuantityAvailableForPurchase} are available.");
                }

                else //TODO
                {
                    // purchase does not exist in the database yet and does not have a valid Id, so we must relate purchaseitem to the object purchase
                    
                }
            }
        }
    }
}
