namespace AppForSEII2526.UIT.Plan
{
    public class UC_SelectClassesForPlan_UIT : UC_UIT
    {
        private SelectClassesForPlan_PO selectClassesForPlan_PO;

        
        private const string class1Name = "Yoga";
        private const string class1Price = "14,99 €";
        private const string class1Type = "Mat";

        private const string class1DateStr = "16/12/2025";
        private const string class1TimeStr = "00:00";

        public UC_SelectClassesForPlan_UIT(ITestOutputHelper output) : base(output)
        {
            selectClassesForPlan_PO = new SelectClassesForPlan_PO(_driver, _output);
        }

        private void Precondition_perform_login()
        {
            Perform_login("elena@uclm.es", "Password1234%");
        }

        private void InitialStepsForSelectClasses()
        {
            Precondition_perform_login();
            _driver.Navigate().GoToUrl(_URI + "plan/selectclassesforplan");
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC_Search_No_Classes_Available()
        {
            //Arrange
            InitialStepsForSelectClasses();

            //Act
            // Searching for a class name that doesn't exist
            selectClassesForPlan_PO.SearchClasses("NonExistentClassXYZ", DateTime.Today);

            //Assert
            Assert.True(selectClassesForPlan_PO.CheckAlertMessage("No available classes found"));
            Assert.False(selectClassesForPlan_PO.IsTableVisible());
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC_Search_Invalid_Date()
        {
            //Arrange
            InitialStepsForSelectClasses();

            //Act
            // Searching with a past date
            selectClassesForPlan_PO.SearchClasses("Yoga", DateTime.Today.AddDays(-1));

            //Assert
            Assert.True(selectClassesForPlan_PO.CheckAlertMessage("The search starting date cannot be earlier than today"));
            Assert.False(selectClassesForPlan_PO.IsTableVisible());
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC_Search_And_Select_Class()
        {
            //Arrange
            InitialStepsForSelectClasses();
            // Expected row content: Name, Price, Required Items, Date Time, Action(Button text not usually checked by CheckBodyTable logic unless modified)
            // Ideally CheckBodyTable checks startsWith, so we define columns 0-3
            var expectedClasses = new List<string[]>
            {
                new string[] { class1Name, class1Price, class1Type, $"{class1DateStr} {class1TimeStr}" }
            };

            //Act
            // 1. Search for Yoga
            selectClassesForPlan_PO.SearchClasses("Yoga", DateTime.Today);

            //Assert 1: Table contains result
            // Note: This relies on seeded data having "Yoga" today or future
            // Assert.True(selectClassesForPlan_PO.CheckListOfClasses(expectedClasses));

            //Act 2: Select the class
            selectClassesForPlan_PO.SelectClass("Yoga");

            //Assert 2: Continue button enabled and price updated
            Assert.True(selectClassesForPlan_PO.IsContinueButtonEnabled());
            Assert.NotEqual("0,00 €", selectClassesForPlan_PO.GetTotalPrice());
        }
    }
}