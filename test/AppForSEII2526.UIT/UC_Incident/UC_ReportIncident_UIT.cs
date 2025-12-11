using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.UC_Incident
{
    public class UC_ReportIncident_UIT : UC_UIT
    {
        private SelectItemsForReporting_PO selectItemsForReporting_PO;

        private const int itemId1 = 1;

        private const string itemName1 = "AAAA";
        private const string itemType1 = "AAA";
        private const string itemLocation1 = "Albacete";
        private const string itemDescription1 = "An item";

        private const int itemId2 = 2;

        private const string itemName2 = "BBBB";
        private const string itemType2 = "BBB";
        private const string itemLocation2 = "Maine";
        private const string itemDescription2 = "Bn btem";

        public UC_ReportIncident_UIT(ITestOutputHelper output) : base(output)
        {
            selectItemsForReporting_PO = new SelectItemsForReporting_PO(_driver, _output);
        }

        private void Precondition_perform_login()
        {
            Perform_login("elena@uclm.es", "Password1234%");
        }

        private void InitialStepsForSelectItems()
        {
            Precondition_perform_login();
            selectItemsForReporting_PO.WaitForBeingVisible(By.Id("incidentsPage"));
            _driver.FindElement(By.Id("incidentsPage")).Click();
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC11_2_no_items_available()
        {
            //Arrange
            InitialStepsForSelectItems();
            
            //Act
            selectItemsForReporting_PO.SearchItems("fhqwhgads", "Llanfairpwllgwyngyllgogerychwyrndrobwllllantysiliogogogoch"); // No item like this exists

            //Assert
            Assert.False(selectItemsForReporting_PO.isTableFound());
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC11_3_1_filtering_by_name()
        {
            //Arrange
            InitialStepsForSelectItems();
            var expectedItems = new List<string[]> { new string[] { itemName1, itemType1, itemLocation1, itemDescription1, "Add" } };

            //Act
            selectItemsForReporting_PO.SearchItems("AA", "");

            //Assert
            Assert.True(selectItemsForReporting_PO.CheckListOfItems(expectedItems));
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC11_3_2_filtering_by_location()
        {
            //Arrange
            InitialStepsForSelectItems();
            var expectedItems = new List<string[]> { new string[] { itemName2, itemType2, itemLocation2, itemDescription2, "Add" } };

            //Act
            selectItemsForReporting_PO.SearchItems("", "Maine");

            //Assert
            Assert.True(selectItemsForReporting_PO.CheckListOfItems(expectedItems));
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC11_4_1_no_items_selected()
        {
            //Arrange
            InitialStepsForSelectItems();

            //Act
            selectItemsForReporting_PO.SearchItems("", "");
            selectItemsForReporting_PO.SelectItems(new List<string> { itemName1 });
            selectItemsForReporting_PO.RemoveItemFromList(itemName1);

            //Assert
            Assert.True(selectItemsForReporting_PO.isReportButtonDisabled());
        }
    }
}
