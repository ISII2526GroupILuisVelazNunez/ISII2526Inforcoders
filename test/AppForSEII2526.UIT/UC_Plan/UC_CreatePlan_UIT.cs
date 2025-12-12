using AppForSEII2526.UIT.Plan; 
namespace AppForSEII2526.UIT.UC_Plan
{
    public class UC_CreatePlan_UIT : UC_UIT
    {
        private CreatePlan_PO createPlan_PO;
        private SelectClassesForPlan_PO selectClasses_PO;

        // Test Data
        private const string validPlanName = "My Fitness Plan";
        private const string validDescription = "Getting fit for summer";
        private const int validWeeks = 4;
        private const string validHealth = "None";
        private const string validPaymentId = "1"; // CreditCard from SeedData

        // Pre-requisite data
        private const string classToSelect = "yoga";

        public UC_CreatePlan_UIT(ITestOutputHelper output) : base(output)
        {
            createPlan_PO = new CreatePlan_PO(_driver, _output);
            selectClasses_PO = new SelectClassesForPlan_PO(_driver, _output);
        }

        private void Precondition_perform_login()
        {
            Perform_login("elena@uclm.es", "Password1234%");
        }

        private void Precondition_NavigateToCreatePlan()
        {
            Precondition_perform_login();

            // 1. Go to Select Classes (Steps 1-2)
            _driver.Navigate().GoToUrl(_URI + "plan/selectclassesforplan");

            // 2. Search & Select a class (Step 3)
            selectClasses_PO.SearchClasses(classToSelect, DateTime.Today);

            // Using the colleague-style method name
            if (selectClasses_PO.IsTableVisible())
            {
                selectClasses_PO.SelectClass(classToSelect);
            }
            else
            {
                Assert.Fail("Precondition failed: No classes found to select.");
            }

            // 3. Click Continue (Step 4)
            Assert.True(selectClasses_PO.IsContinueButtonEnabled(), "Continue button should be enabled");
            _driver.FindElement(By.Id("continueToCreatePlanButton")).Click();

            // Wait for navigation
            Thread.Sleep(1000);
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC_CreatePlan_MainFlow_Success()
        {
            // Steps 1-4 handled in precondition
            // Arrange
            Precondition_NavigateToCreatePlan();

            // Act (Step 5 & 6)
            createPlan_PO.FillPlanDetails(validPlanName, validDescription, validWeeks, validHealth, validPaymentId);
            createPlan_PO.ClickSavePlan();

            // Assert (Step 7)
            Assert.True(createPlan_PO.CheckSuccessMessage(), "Success message should be visible");
            Assert.True(createPlan_PO.CheckPlanDetailsVisible(validPlanName), "Created plan details should be displayed");
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC_CreatePlan_MandatoryDataMissing()
        {
            // Arrange
            Precondition_NavigateToCreatePlan();

            // Act
            // Leave Name empty (Mandatory)
            createPlan_PO.FillPlanDetails("", validDescription, validWeeks, validHealth, validPaymentId);
            createPlan_PO.ClickSavePlan();

            // Assert
            Assert.True(createPlan_PO.CheckValidationError(), "System should notify user about missing mandatory data");
            // Should NOT see success message
            Assert.False(createPlan_PO.CheckSuccessMessage());
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC_CreatePlan_ModifyPlan()
        {
            // Arrange
            Precondition_NavigateToCreatePlan();

            // Act
            createPlan_PO.ClickModifySelectedClasses();

            // Assert
            // Should be back on Select Classes page (Step 2)
            Assert.True(selectClasses_PO.IsTableVisible(), "Should have returned to the Select Classes table");
            Assert.Contains("selectclassesforplan", _driver.Url.ToLower());
        }
    }
}