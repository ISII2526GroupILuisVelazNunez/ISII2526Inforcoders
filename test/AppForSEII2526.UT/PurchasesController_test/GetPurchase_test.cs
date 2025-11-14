using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.PurchaseDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AppForSEII2526.UT.PurchasesController_test
{
    public class GetPurchase_test : AppForSEII25264SqliteUT
    {
        public GetPurchase_test()
        {
            //brand
            var brand = new Brand("Nike");
            _context.Brands.Add(brand);

            //typeItem
            var type = new TypeItem("Material");
            _context.TypeItems.Add(type);

            //item
            var item = new Item("Test description", "Dummy Item", 5, 20, 10, type, brand, 8m);
            _context.Items.Add(item);

            //user
            var user = new ApplicationUser(
                "Test",
                "User"
            )
            {
                UserName = "test@uclm.es",
                Email = "test@uclm.es",
                EmailConfirmed = true
            };
            _context.Users.Add(user);

            //payment method
            var pm = new CreditCard
            {
                CreditCardNumber = 4111222233334444,
                ExpirationDate = new DateTime(2027, 10, 10),
                User = user
            };
            _context.PaymentMethods.Add(pm);

            //purchase
            var purchase = new Purchase("Albacete", "Spain", DateTime.Now, "Test purchase", "Juan Sebastián Elcano")
            {
                PaymentMethod = pm,
                Total_price = 5 * 3  // 3 units × price(5)
            };
            _context.Purchases.Add(purchase);

            //purchaseItem
            var purchaseItem = new PurchaseItem(item.Id,item,20,3,5,purchase,purchase.Id);
            purchase.PurchaseItems.Add(purchaseItem);

            _context.SaveChanges();
        }

        // Test 1 — Purchase not found
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetPurchase_invalid_id_returns_NotFound()
        {
            var logger = new Mock<ILogger<ItemsController>>().Object;
            var controller = new PurchasesController(_context, logger);

            var result = await controller.GetPurchase(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // Test 2 — Purchase exists, correct
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetPurchase_valid_id_returns_purchase_details()
        {
            var logger = new Mock<ILogger<ItemsController>>().Object;
            var controller = new PurchasesController(_context, logger);

            //purchase with id 1 created in constructor
            var result = await controller.GetPurchase(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PurchaseDetailDTO>(okResult.Value);

            //check all fields
            Assert.Equal(1, dto.Id);
            Assert.Equal("Juan Sebastián Elcano", dto.Street);
            Assert.Equal("Albacete", dto.City);
            Assert.Equal("Spain", dto.Country);
            Assert.Equal("Test purchase", dto.Description);
            Assert.Equal("CreditCard", dto.PaymentMethod.Type);
            Assert.Equal(1, dto.PaymentMethod.Id);

            //items
            Assert.Single(dto.PurchaseItems);
            var itemDto = dto.PurchaseItems[0];
            Assert.Equal("Dummy Item", itemDto.Name);
            Assert.Equal("Nike", itemDto.Brand);
            Assert.Equal(3, itemDto.QuantityPurchased);
            Assert.Equal(5, itemDto.Price);
            Assert.Equal(15, dto.Total_price); 
        }
    }
}
