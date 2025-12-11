using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.PlanDTOs;

namespace AppForSEII2526.UT.PlansController_test 
{
    public class GetPlan_test : AppForSEII25264SqliteUT
    {
        // seeded entities to be used in tests
        private readonly Plan _seededPlan;
        private readonly Class _seededClass1;
        private readonly Class _seededClass2;
        private readonly ApplicationUser _seededUser;
        private readonly PlanItem _seededPlanItem1;

        // constructor to seed data before each test
        public GetPlan_test()
        {
            // first the typeitems
            var typeItemMat = new TypeItem("Mat");
            var typeItemGloves = new TypeItem("Gloves");
            _context.TypeItems.AddRange(typeItemMat, typeItemGloves);
            _context.SaveChanges();

            // classes
            _seededClass1 = new Class("Morning Yoga", DateTime.Now.AddDays(3), 10, 24.99m, new List<TypeItem> { typeItemMat });
            _seededClass2 = new Class("Evening Boxing", DateTime.Now.AddDays(4), 5, 39.99m, new List<TypeItem> { typeItemGloves });
            _context.Classes.AddRange(_seededClass1, _seededClass2);
            _context.SaveChanges(); // Class ids 1 and 2

            // user
            _seededUser = new ApplicationUser("Test", "User")
            {
                Id = "1",
                Email = "getplan@test.com",
                UserName = "getplanuser",
                PaymentMethods = new List<PaymentMethod>()
            };
            _context.Users.Add(_seededUser);
            _context.SaveChanges();

            // paymentmethod (again using creditcard because pm is abstract)
            var paymentMethod = new CreditCard
            {
                User = _seededUser,
                CreditCardNumber = 9876543210987654,
                ExpirationDate = DateTime.UtcNow.AddYears(2),
                Plans = new List<Plan>(),
                Purchases = new List<Purchase>()
            };
            _context.PaymentMethods.Add(paymentMethod);
            _context.SaveChanges(); // pm id 1

            // plan
            _seededPlan = new Plan("My Summer Plan", "Plan for getting fit", DateTime.UtcNow, "None", 64.99m, 4, paymentMethod)
            {
                PlanItems = new List<PlanItem>()
            };
            _context.Plans.Add(_seededPlan);
            _context.SaveChanges(); // plan id 1

            // planitems
            // controller reads from pi.Price so we store price in planitem
            _seededPlanItem1 = new PlanItem(_seededPlan, _seededClass1, "Get flexible", 24.99m);
            var planItem2 = new PlanItem(_seededPlan, _seededClass2, "Get strong", 39.99m);
            _context.PlanItems.AddRange(_seededPlanItem1, planItem2);
            _context.SaveChanges();
        }

        // --- CASE 1: SUCCESS ---
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetPlan_OK_ShouldReturn200OK_And_CorrectDTO()
        {
            // ARRANGE
            // CHANGED: Mock<ILogger<PlansController>> instead of ClassesController
            var logger = new Mock<ILogger<PlansController>>().Object;

            // CHANGED: Instantiating PlansController
            var controller = new PlansController(_context, logger);

            // ACT
            // requesting id of the seeded plan
            var result = await controller.GetPlan(_seededPlan.Id);

            // ASSERT
            // checking 200
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // ActionResult<T> wraps the result in .Result

            

            var resultObject = result.Result as OkObjectResult;
            Assert.NotNull(resultObject);
            Assert.Equal(200, resultObject.StatusCode);

            // checking good dto
            var dto = Assert.IsType<PlanForDetailDTO>(resultObject.Value);

            // checking all dto properties are fine
            Assert.Equal(_seededPlan.Id, dto.Id);
            Assert.Equal(_seededPlan.Name, dto.Name);
            Assert.Equal(_seededPlan.TotalPrice, dto.TotalPrice);
            Assert.Equal("Test User", dto.UserFullName.Trim());
            Assert.Equal(_seededPlan.Weeks, dto.Weeks);

            // checking planitems list is good
            Assert.Equal(2, dto.PlanItems.Count); // added 2 items

            // checking 1st item (Yoga)
            // using .First() here assuming order is preserved
            var yogaItemDTO = dto.PlanItems.FirstOrDefault(p => p.ClassId == _seededClass1.Id);
            Assert.NotNull(yogaItemDTO);
            Assert.Equal(_seededClass1.Name, yogaItemDTO.Name);
            Assert.Equal(_seededPlanItem1.Goal, yogaItemDTO.Goal);
            Assert.Equal(_seededClass1.Date, yogaItemDTO.Date);
            Assert.Equal(24.99m, yogaItemDTO.Price); // 
            Assert.Single(yogaItemDTO.Type); // there should be only 1 type
            Assert.Equal("Mat", yogaItemDTO.Type[0]);

            // checking 2nd item (Boxing)
            var boxingItemDTO = dto.PlanItems.FirstOrDefault(p => p.ClassId == _seededClass2.Id);
            Assert.NotNull(boxingItemDTO);
            Assert.Equal(_seededClass2.Name, boxingItemDTO.Name);
            Assert.Equal("Get strong", boxingItemDTO.Goal);
            Assert.Equal(39.99m, boxingItemDTO.Price);
            Assert.Single(boxingItemDTO.Type);
            Assert.Equal("Gloves", boxingItemDTO.Type[0]);
        }


        // --- CASE 2: FAIL - NOT FOUND ---
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetPlan_Fail_NotFound_ShouldReturn404NotFound()
        {
            // ARRANGE
            var logger = new Mock<ILogger<PlansController>>().Object;
            var controller = new PlansController(_context, logger);
            var idToRequest = 999; // obv will not exist

            // ACT
            var result = await controller.GetPlan(idToRequest);

            // ASSERT
            // checking 404
            // controller returns NotFoundResult wrapped in ActionResult
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}