using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ItemForExerciseDTOs;

namespace AppForSEII2526.UT.ItemsController_test
{
    public class GetItemsForReporting_test  : AppForSEII25264SqliteUT
    {
        public GetItemsForReporting_test()
        {
            var TypeItems = new List<TypeItem>()
            {
                new TypeItem("Type A"),
                new TypeItem("Type B"),
            };

            var Brands = new List<Brand>()
            {
                new Brand("Albacete brand"),
                new Brand("Barcelona brand"),
            };

            var Items = new List<Item>()
            {
                new Item("A description", "AAAAA", 10, 5, 20, TypeItems[0], Brands[0], 15),
                new Item("Basically another description", "BBBBB", 20, 3, 14, TypeItems[1], Brands[1], 24),
            };

            var IFEs = new List<ItemForExercise>()
            {
                new ItemForExercise("Albacete", Items[0]),
                new ItemForExercise("Barcelona", Items[1]),
            };

            /*
            _context.Add(TypeItems);
            _context.Add(Brands);
            _context.Add(Items);
            */
            _context.AddRange(IFEs);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetItemsForReporting_null_name_location()
        {
            //arrange
            var ExpectedIFEs = new List<ItemForReportingDTO>()
            {
                new ItemForReportingDTO(1, "AAAAA", "Albacete", "A description", "Type A"),
                new ItemForReportingDTO(2, "BBBBB", "Barcelona", "Basically another description", "Type B"),
            };

            var controller = new ItemsController(_context, null);

            //act
            var result = await controller.GetItemsForReporting(null, null);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var IFEsDTOsActual = Assert.IsType<List<ItemForReportingDTO>>(okResult.Value);
            Assert.Equal(ExpectedIFEs, IFEsDTOsActual);
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_GetItemsForReporting))]
        public async Task GetItemsForReporting_filtertest(string itemName, string itemLocation,
            List<ItemForReportingDTO> ExpectedIFEs)
        {
            var controller = new ItemsController(_context, null);

            var result = await controller.GetItemsForReporting(itemName, itemLocation);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var IFEsDTOsActual = Assert.IsType<List<ItemForReportingDTO>>(okResult.Value);
            Assert.Equal(ExpectedIFEs, IFEsDTOsActual);
        }

        public static IEnumerable<object[]> TestCasesFor_GetItemsForReporting()
        {
            var IFEs = new List<ItemForReportingDTO>()
            {
                new ItemForReportingDTO(1, "AAAAA", "Albacete", "A description", "Type A"),
                new ItemForReportingDTO(2, "BBBBB", "Barcelona", "Basically another description", "Type B"),
            };

            var TC1_null_null = new List<ItemForReportingDTO> { IFEs[0], IFEs[1] };
            var TC2_with_name = new List<ItemForReportingDTO> { IFEs[0]};
            var TC3_with_location = new List<ItemForReportingDTO> { IFEs[1] };
            var TC4_test_contains_condition = new List<ItemForReportingDTO> { IFEs[0], IFEs[1] };

            var allTests = new List<object[]>
            {
                new object[]{null, null, TC1_null_null},
                new object[]{"AAAAA", null, TC2_with_name},
                new object[]{null, "Barcelona", TC3_with_location},
                new object[]{null, "a", TC4_test_contains_condition},
            };

            return allTests;
        }
    }
}
