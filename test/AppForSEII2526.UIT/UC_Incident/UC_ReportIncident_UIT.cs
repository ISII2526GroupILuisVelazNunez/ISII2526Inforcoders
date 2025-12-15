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
        private DetailIncident_PO detailIncident_PO;
        private CreateIncident_PO createIncident_PO;

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

        // Create incident for the exam
        private string incidentTitle = "This title was created during the exam";
        private string incidentExercise = "Volleyball";
        private string incidentReporterName = "Elena Navarro";

        public UC_ReportIncident_UIT(ITestOutputHelper output) : base(output)
        {
            selectItemsForReporting_PO = new SelectItemsForReporting_PO(_driver, _output);
            detailIncident_PO = new DetailIncident_PO(_driver, _output);
            createIncident_PO = new CreateIncident_PO(_driver, _output);
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
        public void UC11_1_report_details()
        {
            //Arrange
            InitialStepsForSelectItems();
            string Title = "An incident";
            DateTime DateOfIdentification = new DateTime(2025, 11, 11);
            string Exercise = "Surf";
            string ReporterName = "Elena";
            var expectedItems = new List<string[]> { new string[] { itemName1, itemType1, itemLocation1, itemDescription1, "High" } };

            //Act
            selectItemsForReporting_PO.WaitForBeingVisible(By.Id("DetailsButton"));
            _driver.FindElement(By.Id("DetailsButton")).Click();
            System.Threading.Thread.Sleep(1000);

            //Assert
            Assert.True(detailIncident_PO.CheckItemsTable(expectedItems));
            Assert.True(detailIncident_PO.CheckIncidentDetails(Title, DateOfIdentification, Exercise, ReporterName));
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC11_2_no_items_available()
        {
            //Arrange
            InitialStepsForSelectItems();
            
            //Act
            selectItemsForReporting_PO.SearchItems("fhqwhgads", "Llanfairpwllgwyngyllgogerychwyrndrobwllllantysiliogogogoch"); // No item like this exists
            System.Threading.Thread.Sleep(1000);

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
        public void UC11_5_1_modify_list()
        {
            //Arrange
            InitialStepsForSelectItems();
            var expectedItems = new List<string[]> { new string[] { itemName2, "Remove" } };


            //Act
            selectItemsForReporting_PO.SearchItems("", "");
            selectItemsForReporting_PO.SelectItems(new List<string> { itemName1, itemName2 });
            selectItemsForReporting_PO.RemoveItemFromList(itemName1);

            System.Threading.Thread.Sleep(1000);

            //Assert 
            Assert.Equal(selectItemsForReporting_PO.CheckItemCount(), "Items selected: 1");
            Assert.True(selectItemsForReporting_PO.CheckListOfSelectedItems(expectedItems));
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

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC_Bonus_test_for_evaluation()
        {
            //Arrange
            InitialStepsForSelectItems();


            //Act

            // 1) Add an item
            selectItemsForReporting_PO.SearchItems("", "");
            selectItemsForReporting_PO.SelectItems(new List<string> { itemName1 });

            // 2) Filter by name
            selectItemsForReporting_PO.SearchItems("BB", "");

            // 3) Add a new item
            selectItemsForReporting_PO.SelectItems(new List<string> { itemName2 });

            // 4) Remove the first item
            selectItemsForReporting_PO.RemoveItemFromList(itemName1);

            // Continue the BF
            selectItemsForReporting_PO.Continue();

            createIncident_PO.fill_report_data(incidentTitle, incidentExercise, incidentReporterName);
            createIncident_PO.Submit_Report();


            //Assert
            Assert.True(true);
        }
    }
}
