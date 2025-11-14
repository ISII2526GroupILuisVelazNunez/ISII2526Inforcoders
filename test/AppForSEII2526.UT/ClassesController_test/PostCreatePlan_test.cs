using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ClassDTOs;
using AppForSEII2526.API.DTOs.PlanDTOs;
namespace AppForSEII2526.UT.ClassesController_test
{
    public class PostCreatePlan_test : AppForSEII25264SqliteUT 
    {
        // creating seeded entities
        private readonly Class classWithCapacity; // id 1
        private readonly Class classFull;         // id 2
        private readonly PaymentMethod paymentMethod; // id 1 
        private readonly ApplicationUser user;

        // constructor will seed data before each test
        public PostCreatePlan_test()
        {
            // typeitems creation
            var typeItems = new List<TypeItem>
            {
                new TypeItem("Mat"),         // index 0
                new TypeItem("Spinning Bike")// index 1
            };
            _context.TypeItems.AddRange(typeItems);
            _context.SaveChanges();

            // creating classes with typeitem constructor
            classWithCapacity = new Class("Yoga", DateTime.Now.AddDays(5), 10, 14.99m, new List<TypeItem> { typeItems[0] });
            classFull = new Class("Spinning", DateTime.Now.AddDays(3), 1, 19.99m, new List<TypeItem> { typeItems[1] });
            _context.Classes.AddRange(classWithCapacity, classFull);
            _context.SaveChanges(); // Yoga id 1, Spinning id 2

            // create user
            user = new ApplicationUser("Test", "User")
            {
                Id = "1", // using string id for ApplicationUser
                Email = "test@user.com",
                UserName = "testuser",
                PaymentMethods = new List<PaymentMethod>() // initializing list to be filled later
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // creating payment method
            // abstract :( so need use a derived class 
            // credit card it is
            paymentMethod = new CreditCard
            {
                // Id = 1 auto-generated
                User = user,
                CreditCardNumber = 1234567890123456,
                ExpirationDate = DateTime.UtcNow.AddYears(3),
                Plans = new List<Plan>(), // will fill later
                Purchases = new List<Purchase>() // same
            };
            _context.PaymentMethods.Add(paymentMethod);
            _context.SaveChanges(); // payment method id 1


            // filling classFull class with a plan and planitem to reach capacity of 1
            // simulating a person already enrolled in the class using old plan and planitem
            // plan constructor with no planitems so i can fill later
            var oldPlan = new Plan("Old Plan", "Old", DateTime.UtcNow, "None", 50.0m, 1, paymentMethod)
            {
                PlanItems = new List<PlanItem>()
            };
            _context.Plans.Add(oldPlan);
            _context.SaveChanges(); // oldPlan.Id is now 1

            // now i create planitem
            var oldPlanItem = new PlanItem(oldPlan, classFull, "Old goal", 50.0m);
            _context.PlanItems.Add(oldPlanItem);
            _context.SaveChanges(); // this will make class with id 2 full (capacity 1 reached)
        }

        // --- CASE 1: SUCCESS ---
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePlan_OK_ShouldReturn201Created()
        {
            // ARRANGE
            var logger = new Mock<ILogger<ClassesController>>().Object; // mock logger
            var controller = new ClassesController(_context, logger);

            // dto client would send
            var createDTO = new PlanForCreateDTO
            {
                Name = "My Yoga Plan",
                Description = "Test description",
                Weeks = 4,
                HealthIssues = "None",
                PaymentMethodId = paymentMethod.Id, // Use the seeded PM (ID 1)
                Items = new List<PlanItemForCreateDTO> // items would be the classes selected
                {
                    // Use the ID of the class WITH capacity (ID 1)
                    new PlanItemForCreateDTO { ClassId = classWithCapacity.Id, Goal = "My Yoga Goal" }
                }
            };

            // ACT
            var result = await controller.CreatePlan(createDTO);

            // ASSERT
            // checking 201 created response
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(201, createdResult.StatusCode);

            // checking returned DTO content is correct
            var dtoResult = Assert.IsType<PlanForDetailDTO>(createdResult.Value);
            Assert.Equal("My Yoga Plan", dtoResult.Name);
            Assert.Equal(14.99m, dtoResult.TotalPrice); // price of the Yoga Class
            Assert.Equal("Test User", dtoResult.UserFullName.Trim());
            Assert.Single(dtoResult.PlanItems); // should have 1 item
            Assert.Equal("My Yoga Goal", dtoResult.PlanItems[0].Goal);
            Assert.Equal(classWithCapacity.Id, dtoResult.PlanItems[0].ClassId);

            // checking its saved to db
            var planInDb = await _context.Plans.FindAsync(dtoResult.Id);
            Assert.NotNull(planInDb);
            Assert.Equal(14.99m, planInDb.TotalPrice);
        }

        // --- CASE 2: FAIL - NO ITEMS ---
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePlan_Fail_NoItems_ShouldReturn400BadRequest()
        {
            // ARRANGE
            var logger = new Mock<ILogger<ClassesController>>().Object;
            var controller = new ClassesController(_context, logger);

            var createDTO = new PlanForCreateDTO
            {
                Name = "Plan with no items",
                PaymentMethodId = paymentMethod.Id,
                Items = new List<PlanItemForCreateDTO>()
            };

            // ACT
            var result = await controller.CreatePlan(createDTO);

            // ASSERT
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("At least 1 class must be selected for the plan.", badRequestResult.Value);
        }

        // --- CASE 3: FAIL - PAYMENT METHOD NOT FOUND ---
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePlan_Fail_PaymentMethodNotFound_ShouldReturn404NotFound()
        {
            // ARRANGE
            var logger = new Mock<ILogger<ClassesController>>().Object;
            var controller = new ClassesController(_context, logger);

            var createDTO = new PlanForCreateDTO
            {
                Name = "Plan with invalid payment method",
                PaymentMethodId = 999, // id that does not exist
                Items = new List<PlanItemForCreateDTO>
                {
                    new PlanItemForCreateDTO { ClassId = classWithCapacity.Id, Goal = "My goal" }
                }
            };

            // ACT
            var result = await controller.CreatePlan(createDTO);

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal($"Payment method with ID 999 was not found.", notFoundResult.Value);
        }

        // --- CASE 4: FAIL - CLASS NOT FOUND ---
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePlan_Fail_ClassNotFound_ShouldReturn404NotFound()
        {
            // ARRANGE
            var logger = new Mock<ILogger<ClassesController>>().Object;
            var controller = new ClassesController(_context, logger);

            var createDTO = new PlanForCreateDTO
            {
                Name = "Plan with invalid class",
                PaymentMethodId = paymentMethod.Id,
                Items = new List<PlanItemForCreateDTO>
                {
                    new PlanItemForCreateDTO { ClassId = 999, Goal = "Ghost class" } // id that does not exist
                }
            };

            // ACT
            var result = await controller.CreatePlan(createDTO);

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal($"Class with ID 999 does not exist.", notFoundResult.Value);
        }

        // --- CASE 5: FAIL - CLASS HAS NO CAPACITY ---
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePlan_Fail_NoCapacity_ShouldReturn409Conflict()
        {
            // ARRANGE
            var logger = new Mock<ILogger<ClassesController>>().Object;
            var controller = new ClassesController(_context, logger);

            var createDTO = new PlanForCreateDTO
            {
                Name = "Plan for full class",
                PaymentMethodId = paymentMethod.Id,
                Items = new List<PlanItemForCreateDTO>
                {
                    // id 2 = classFull, which has capacity 1 and was filled in the constructor
                    new PlanItemForCreateDTO { ClassId = classFull.Id, Goal = "Trying to get in" }
                }
            };

            // ACT
            var result = await controller.CreatePlan(createDTO);

            // ASSERT
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal(409, conflictResult.StatusCode);
            // Check the exact error message from your controller
            Assert.Equal($"Class '{classFull.Name}' does not have enough capacity. Please modify your selection.", conflictResult.Value);
        }
    }
}