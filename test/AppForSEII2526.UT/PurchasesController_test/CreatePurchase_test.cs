using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ItemDTOs;
using AppForSEII2526.API.DTOs.PlanDTOs;
using AppForSEII2526.API.DTOs.PurchaseDTOs;
using AppForSEII2526.API.Models;
using AppForSEII2526.UT;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.PurchasesController_test
{
    public class CreatePurchase_test : AppForSEII25264SqliteUT
    {
        public CreatePurchase_test()
        {
            // Create a dummy user
            var user = new ApplicationUser(
                "Test",
                "User"
            )
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                EmailConfirmed = true
            };
            _context.Users.Add(user);

            //brand ---
            var brand = new Brand("Nike");
            _context.Add(brand);

            //typeItem
            var typeItem = new TypeItem("Material");
            _context.Add(typeItem);

            //item
            var item = new Item("Test description", "Dummy Item", 10, 20, 5, typeItem, brand, 8);

            _context.Add(item);

            // payment method
            var pm = new CreditCard { CreditCardNumber = 4111222233334444, ExpirationDate = new DateTime(2027, 10, 10), User = user };

            _context.PaymentMethods.Add(pm);

            _context.SaveChanges();
        }


        // Test 1 - Street is missing
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_missing_street_returns_badrequest()
        {
            //arrange
            var logger = new Mock<ILogger<ItemsController>>().Object;
            var controller = new PurchasesController(_context, logger);

            var dto = new PurchaseForCreateDTO
            {
                Street = "",
                City = "Albacete",
                Country = "Spain",
                PaymentMethodId = 1,
                PurchaseItems = new List<ItemForPurchaseSelectionDTO>()
            };

            var result = await controller.CreatePurchase(dto);
            //assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);

            Assert.Equal("Error! You must specify a street.", problem.Errors["Street"][0]);
        }

        // Test 2 - City is missing
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_missing_city_returns_badrequest()
        {
            //arrange
            var logger = new Mock<ILogger<ItemsController>>().Object;
            var controller = new PurchasesController(_context, logger);

            var dto = new PurchaseForCreateDTO
            {
                Street = "Juan Sebastián Elcano",
                City = "",
                Country = "Spain",
                PaymentMethodId = 1,
                PurchaseItems = new List<ItemForPurchaseSelectionDTO>()
            };

            var result = await controller.CreatePurchase(dto);
            //assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);

            Assert.Equal("Error! You must specify a city.", problem.Errors["City"][0]);
        }

        // Test 3 - Country is missing
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_missing_country_returns_badrequest()
        {
            //arrange
            var logger = new Mock<ILogger<ItemsController>>().Object;
            var controller = new PurchasesController(_context, logger);

            var dto = new PurchaseForCreateDTO
            {
                Street = "Juan Sebastián Elcano",
                City = "Albacete",
                Country = "",
                PaymentMethodId = 1,
                PurchaseItems = new List<ItemForPurchaseSelectionDTO>()
            };

            var result = await controller.CreatePurchase(dto);
            //assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);

            Assert.Equal("Error! You must specify a country.", problem.Errors["Country"][0]);
        }


        // Test 4 - Payment method doesn't exist
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_invalid_payment_method_returns_badrequest()
        {
            //arrange
            var logger = new Mock<ILogger<ItemsController>>().Object; 
            var controller = new PurchasesController(_context, logger);

            var dto = new PurchaseForCreateDTO
            {
                Street = "Juan Sebastián Elcano",
                City = "Albacete",
                Country = "Spain",
                PaymentMethodId = 404,
                PurchaseItems = new List<ItemForPurchaseSelectionDTO>
                {
                    new ItemForPurchaseSelectionDTO
                    {
                        Id = 1,
                        Name = "Dummy Item",
                        QuantityToBuy = 1
                    }
                }
            };

            var result = await controller.CreatePurchase(dto);
            //assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);

            Assert.Equal("Error! Selected payment method does not exist.",
                         problem.Errors["PaymentMethodId"][0]);
        }

        // Test 5 - Item doesn't exist
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_item_not_found_returns_badrequest()
        {
            //arrange
            var logger = new Mock<ILogger<ItemsController>>().Object;
            var controller = new PurchasesController(_context, logger);

            var dto = new PurchaseForCreateDTO
            {
                Street = "Juan Sebastián Elcano",
                City = "Albacete",
                Country = "Spain",
                PaymentMethodId = 1,
                PurchaseItems = new List<ItemForPurchaseSelectionDTO>
                {
                    new ItemForPurchaseSelectionDTO
                    {
                        Id = 404, // invalid item id
                        Name = "Invalid Item",
                        QuantityToBuy = 1
                    }
                }
            };

            var result = await controller.CreatePurchase(dto);
            //assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);
            Assert.Equal("Item 'Invalid Item' does not exist.", problem.Errors["PurchaseItems"][0]);
        }


        // Test 6 - User tried to buy more than available
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_quantity_exceeds_available_returns_badrequest()
        {
            //arrange
            var logger = new Mock<ILogger<ItemsController>>().Object;
            var controller = new PurchasesController(_context, logger);

            var dto = new PurchaseForCreateDTO
            {
                Street = "Juan Sebastián Elcano",
                City = "Albacete",
                Country = "Spain",
                PaymentMethodId = 1,
                PurchaseItems = new List<ItemForPurchaseSelectionDTO>
                {
                    new ItemForPurchaseSelectionDTO
                    {
                        Id = 1,
                        Name = "Dummy Item",
                        QuantityToBuy = 99999 //too many, check
                    }
                }
            };

            var result = await controller.CreatePurchase(dto);
            //assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);

            Assert.Equal(
                "Requested 99999 of 'Dummy Item', but only 20 available.",
                problem.Errors["PurchaseItems"][0]
            );
        }

        // Test 8 - Description input does not start with "My purchase for"
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_bad_description_returns_badrequest()
        {
            //arrange
            var logger = new Mock<ILogger<ItemsController>>().Object;
            var controller = new PurchasesController(_context, logger);

            var dto = new PurchaseForCreateDTO
            {
                Street = "Juan Sebastián Elcano",
                City = "Albacete",
                Country = "Spain",
                PaymentMethodId = 1,
                Description = "Hola",
                PurchaseItems = new List<ItemForPurchaseSelectionDTO>()
            };

            var result = await controller.CreatePurchase(dto);
            //assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);

            Assert.Equal("Error! You must start the Description with My purchase for", problem.Errors["Description"][0]);
        }

        // Test 7 - Sucessfully purchased
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_valid_request_creates_purchase()
        {
            //arrange
            var logger = new Mock<ILogger<ItemsController>>().Object;
            var controller = new PurchasesController(_context, logger);


            var dto = new PurchaseForCreateDTO
            {
                Street = "Juan Sebastián Elcano",
                City = "Albacete",
                Country = "Spain",
                PaymentMethodId = 1,
                Description = "My purchase for",
                PurchaseItems = new List<ItemForPurchaseSelectionDTO>
                {
                    new ItemForPurchaseSelectionDTO
                    {
                        Id = 1,
                        Name = "Dummy Item",
                        QuantityToBuy = 1
                    }
                }
            };

            var result = await controller.CreatePurchase(dto);

            //assert

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualPurchaseDto = Assert.IsType<PurchaseDetailDTO>(createdResult.Value);

            //check for fields 
            Assert.Equal("Juan Sebastián Elcano", actualPurchaseDto.Street);
            Assert.Equal("Albacete", actualPurchaseDto.City);
            Assert.Equal("Spain", actualPurchaseDto.Country);
            Assert.Equal("My purchase for", actualPurchaseDto.Description);
            Assert.Equal(10m, actualPurchaseDto.Total_price);
            Assert.Equal(1, actualPurchaseDto.PaymentMethod.Id);

            //check each item
            Assert.Equal(1, actualPurchaseDto.PurchaseItems.Count); 
            for (int i = 0; i < 1; i++)
            {
                var expectedItem = new ItemForPurchaseDetailDTO("Dummy Item", "Nike", 1, 10m);
                var actualItem = actualPurchaseDto.PurchaseItems[i];

                Assert.Equal(expectedItem.Name, actualItem.Name);
                Assert.Equal(expectedItem.Brand, actualItem.Brand);
                Assert.Equal(expectedItem.QuantityPurchased, actualItem.QuantityPurchased);
                Assert.Equal(expectedItem.Price, actualItem.Price);
            }
        }
    }
}