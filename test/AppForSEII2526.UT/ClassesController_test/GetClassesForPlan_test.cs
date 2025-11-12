using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppForSEII2526.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ClassDTOs;
using AppForSEII2526.API.Models;
using Microsoft.VisualBasic;

namespace AppForSEII2526.UT.ClassesController_test
{
    public class GetClassesForPlan_test : AppForSEII25264SqliteUT
    {
        // test dates
        // we use datetime here because its what the controller uses to filter
        private static readonly DateTime datePast = DateTime.Now.AddDays(-1);
        private static readonly DateTime dateFuture = DateTime.Now.AddDays(2);
        private static readonly DateTime dateFarFuture = DateTime.Now.AddDays(7);

        // expected DTOs
        private static readonly ClassForPlanDTO yogaDTO = new ClassForPlanDTO(
            1, "Future Yoga", 19.99m, new List<string> { "Mat" }, dateFuture
        );

        private static readonly ClassForPlanDTO boxingDTO = new ClassForPlanDTO(
            3, "Boxing Next Week", 14.99m, new List<string> { "Punching Bag" }, dateFarFuture
        );

        // constructor seeds data before each test
        public GetClassesForPlan_test()
        {
            // creating the typeitems
            var typeItems = new List<TypeItem>
            {
                new TypeItem("Mat"),         // id 0
                new TypeItem("Dumbbell"),    // id 1
                new TypeItem("Punching Bag") // id 2
            };
            _context.TypeItems.AddRange(typeItems);
            _context.SaveChanges(); // save so ids can be assigned

            // class creation
            var classes = new List<Class>
            {
                // this one is in the close future (must appear)
                new Class("Future Yoga", dateFuture, 40, 19.99m, new List<TypeItem> {
                    typeItems[0]
                }), // id 1

                // past (will not appear)
                new Class("Past Cardio", datePast, 25, 9.99m, new List<TypeItem> {
                    typeItems[1]
                }), // id 2
            
                // far future (must appear)
                new Class("Boxing Next Week", dateFarFuture, 30, 14.99m, new List<TypeItem> {
                    typeItems[2]
                }) // id 3
            };
            _context.Classes.AddRange(classes);

            // save to the in-memory database
            _context.SaveChanges();
        }


        // test itself
        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_GetClassesForPlan))]
        public async Task GetClassesForPlan_Theory(string? name, DateTime? date, List<ClassForPlanDTO> expectedClasses)
        {
            // ARRANGE
            var mock = new Mock<ILogger<ClassesController>>();
            ILogger<ClassesController> logger = mock.Object;

            // using the context with seeded data
            var controller = new ClassesController(_context, logger);

            // ACT
            var result = await controller.GetClassesForPlan(name, date);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var classesActualResult = Assert.IsType<List<ClassForPlanDTO>>(okResult.Value);

            // comparing expected vs actual
            Assert.Equal(expectedClasses.Count, classesActualResult.Count);
            Assert.Equal(expectedClasses, classesActualResult);
        }

        // test cases
        public static IEnumerable<object[]> TestCasesFor_GetClassesForPlan()
        {
            // case 1: default search (name=null, date=null)
            // logic: date is turned into DateTime.Now
            // expected: all future classes (Future Yoga and Boxing Next Week)
            var expected_TC1 = new List<ClassForPlanDTO> { yogaDTO, boxingDTO };

            // case 2: filtering just name (name="Yoga", date=null)
            // logic: date is DateTime.Now, name contains "Yoga".
            // expected: only "Future Yoga".
            var expected_TC2 = new List<ClassForPlanDTO> { yogaDTO };

            // case 3: filtering date (name=null, date= far future - 5 min)
            // logic: filter is i.Date >= referenceDate. 
            // if referenceDate = far future - 5 min, only "Boxing Next Week" qualifies.
            var expected_TC3 = new List<ClassForPlanDTO> { boxingDTO };

            // case 4: filtering name that does not exist (name="Nonexistent", date=null)
            var expected_TC4 = new List<ClassForPlanDTO>();

            // returning all cases
            var allTests = new List<object[]>
            {
                //             name,              date,                             expectedClasses
                new object[] { null,              null,                             expected_TC1 },
                new object[] { "Yoga",            null,                             expected_TC2 },
                new object[] { null,              dateFarFuture.AddMinutes(-5),     expected_TC3 },
                new object[] { "Nonexistent",     null,                             expected_TC4 }
            };

            return allTests;
        }
    }
}
