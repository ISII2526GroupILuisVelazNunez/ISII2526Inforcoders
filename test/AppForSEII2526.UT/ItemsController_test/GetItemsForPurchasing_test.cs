using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ItemDTOs;

namespace AppForSEII2526.UT.ItemsController_test
{
    public class GetItemsForPurchasing_test  : AppForSEII25264SqliteUT
    {
        public GetItemsForPurchasing_test()
        {
            var TypeItems = new List<TypeItem>()
            {
                new TypeItem("Material"),
                new TypeItem("Clothing"),
            };

            var Brands = new List<Brand>()
            {
                new Brand("Nike"),
                new Brand("Adidas"),
            };

            var Items = new List<Item>()
            {
                new Item("Shaker bottle for protein shakes", "Shaker Bottle S", 5, 20, 30, TypeItems[0], Brands[0], 15),
                new Item("A shirt which is ideal for exercise", "Breathable Shirt S", 20, 10, 40, TypeItems[1], Brands[1], 30),
            };

            _context.AddRange(TypeItems);
            _context.AddRange(Brands);
            _context.AddRange(Items);

            _context.SaveChanges();
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_GetItemsForPurchasing))]
        public async Task GetItemsForPurchasing_filtertest(string? itemName, string? itemBrand,
            List<ItemForPurchaseDTO> ExpectedIFPs)
        {
            var controller = new ItemsController(_context, null);

            var result = await controller.GetItemsForPurchasing(itemName, itemBrand);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var ActualIFPsDTOs = Assert.IsType<List<ItemForPurchaseDTO>>(okResult.Value);
            Assert.Equal(ExpectedIFPs, ActualIFPsDTOs);
        }

        public static IEnumerable<object[]> TestCasesFor_GetItemsForPurchasing()
        {
            var IFPs = new List<ItemForPurchaseDTO>()
            {
                new ItemForPurchaseDTO(1, "Shaker Bottle S", "Nike", "Shaker bottle for protein shakes", 5, 20),
                new ItemForPurchaseDTO(2, "Breathable Shirt S", "Adidas", "A shirt which is ideal for exercise", 20, 10)
            };

            //both filters are null so we return all items
            var TC1_both_are_null = new List<ItemForPurchaseDTO> { IFPs[1], IFPs[0] }; //Changed 1 and 0 around, as it is ordered by name (Breathable before Shaker)
            //a name filter is filled and a matching item is returned
            var TC2_name_is_filled = new List<ItemForPurchaseDTO> { IFPs[0]};
            //a brand filter is filled and a matching item is returned
            var TC3_brand_is_filled = new List<ItemForPurchaseDTO> { IFPs[1] };
            //a filter is used and more than one item matches the condition
            var TC4_filter_many = new List<ItemForPurchaseDTO> { IFPs[1], IFPs[0] };
            //a filter matches both, but another does not
            var TC5_specific = new List<ItemForPurchaseDTO> { IFPs[0] };

            var allTests = new List<object[]>
            {
                new object[]{null, null, TC1_both_are_null},
                new object[]{"Shaker", null, TC2_name_is_filled},
                new object[]{null, "Adidas", TC3_brand_is_filled},
                new object[]{"S", null, TC4_filter_many},
                new object[]{"S", "Nike", TC5_specific},
            };

            return allTests;
        }
    }
}
