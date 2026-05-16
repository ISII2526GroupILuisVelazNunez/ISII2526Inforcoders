using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AppForSEII2526.UIT.UC_Plan
{
    public class UCCreatePlan_UIT : UC_UIT
    {
        private SelectClassesForPlan_PO selectClasses_PO;
        private CreatePlan_PO createPlan_PO;
        private PlanDetails_PO planDetails_PO;

        private const string class1Name = "Yoga";
        private const string class1DateStr = "01-Jan-27 0:00";

        private const string class2Name = "Thai";
        private const string class2DateStr = "01-Jan-28 0:00";

        public UCCreatePlan_UIT(ITestOutputHelper output) : base(output)
        {
            Initial_step_opening_the_web_page();
            selectClasses_PO = new SelectClassesForPlan_PO(_driver, _output);
            createPlan_PO = new CreatePlan_PO(_driver, _output);
            planDetails_PO = new PlanDetails_PO(_driver, _output);
        }

        private void Precondition_perform_login()
        {
            Perform_login("elena@uclm.es", "Password1234%");
        }

        private void InitialStepsForCreatePlan()
        {
            Precondition_perform_login();
            Thread.Sleep(1500);

            try
            {
                var planMenuLink = _driver.FindElement(By.XPath("//a[contains(., 'Plan') or contains(@href, 'plan')]"));
                planMenuLink.Click();
            }
            catch
            {
                _driver.Navigate().GoToUrl(_URI + "plan/selectclassesforplan");
            }

            Thread.Sleep(1500);
        }

        private string GenerateUniquePlanName(string baseName)
        {
            return $"{baseName}_{DateTime.Now.ToString("HHmmss")}";
        }

        // BASIC FLOW ---

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_1_BasicFlow_CreditCard()
        {
            InitialStepsForCreatePlan();
            string todayDate = DateTime.Today.ToString("dd/MM/yyyy");
            string planName = GenerateUniquePlanName("PlanYoga");

            selectClasses_PO.FilterClasses("", todayDate);
            Thread.Sleep(1500);

            selectClasses_PO.SelectClass(class1Name);
            selectClasses_PO.PressContinue();
            Thread.Sleep(1500);

            createPlan_PO.FillInPlanInfo(planName, 4, "CreditCard");
            createPlan_PO.PressSavePlan();
            Thread.Sleep(2000);

            Assert.True(planDetails_PO.CheckDetailsInfo(planName), $"Plan name: '{planName}' doesn´t match.");
            Assert.True(planDetails_PO.CheckDetailsInfo("4"), "Total price isn´t 4.");
        }

        // --- PAYPAL ---

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_2_Sce1_PayPal()
        {
            InitialStepsForCreatePlan();
            string todayDate = DateTime.Today.ToString("dd/MM/yyyy");
            string planName = GenerateUniquePlanName("PlanThai");

            selectClasses_PO.FilterClasses("Thai", todayDate);
            Thread.Sleep(1500);

            selectClasses_PO.SelectClass(class2Name);
            Thread.Sleep(1000);

            selectClasses_PO.PressContinue();
            Thread.Sleep(1500);

            createPlan_PO.FillInPlanInfo(planName, 2, "PayPal");
            createPlan_PO.PressSavePlan();
            Thread.Sleep(2000);

            Assert.True(planDetails_PO.CheckDetailsInfo(planName), $"Plan name: '{planName}' doesn´t match.");
            Assert.True(planDetails_PO.CheckDetailsInfo("9"), "Total price isn´t 9.");
        }

        // --- NAME FILTER ---

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_3_Sce2_FilterByName()
        {
            InitialStepsForCreatePlan();
            string todayDate = DateTime.Today.ToString("dd/MM/yyyy");

            selectClasses_PO.FilterClasses("Yoga", todayDate);
            Thread.Sleep(1500);

            Assert.True(_driver.PageSource.Contains(class1Name), "Yoga should be visible in the table.");
            Assert.False(_driver.PageSource.Contains(class2Name), "Thai should not be visible in the table.");
        }


        // --- DATE FILTER ---

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_4_Sce3_DateBeforeToday()
        {
            InitialStepsForCreatePlan();

            string pastDate = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            selectClasses_PO.FilterClasses("", pastDate);
            Thread.Sleep(1500);

            Assert.True(_driver.PageSource.Contains("The search starting date cannot be earlier than today"), "Date error message is missing.");
        }


        // --- MODIFY PLAN ---

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_5_Sce4_ModifyPlan()
        {
            InitialStepsForCreatePlan();
            string todayDate = DateTime.Today.ToString("dd/MM/yyyy");

            selectClasses_PO.FilterClasses("", todayDate);
            Thread.Sleep(1500);

            selectClasses_PO.SelectClass(class1Name);
            Thread.Sleep(1000);
            selectClasses_PO.SelectClass(class2Name);
            Thread.Sleep(1000);

            _driver.FindElement(By.Id("removeClass_" + class1Name)).Click();
            Thread.Sleep(1000);

            var totalSpan = _driver.FindElement(By.Id("TotalPlanPrice")).Text;
            Assert.Contains("9", totalSpan);
        }


        // --- MANDATORY CLASSES ---

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_6_Sce5_ClassesNotSelected()
        {
            InitialStepsForCreatePlan();
            string todayDate = DateTime.Today.ToString("dd/MM/yyyy");

            selectClasses_PO.FilterClasses("", todayDate);
            Thread.Sleep(1500);

            selectClasses_PO.SelectClass(class1Name);
            Thread.Sleep(1000);

            _driver.FindElement(By.Id("removeClass_" + class1Name)).Click();
            Thread.Sleep(1000);

            var continueBtn = _driver.FindElement(By.Id("continueToCreatePlanButton"));
            Assert.False(continueBtn.Enabled, "Continue button should be unavailable.");
        }


        // --- MANDATORY NAME ---

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_7_Sce6_NameNotProvided()
        {
            InitialStepsForCreatePlan();
            string todayDate = DateTime.Today.ToString("dd/MM/yyyy");

            selectClasses_PO.FilterClasses("", todayDate);
            Thread.Sleep(1500);
            selectClasses_PO.SelectClass(class1Name);
            selectClasses_PO.PressContinue();
            Thread.Sleep(1500);

            createPlan_PO.FillInPlanInfo("", 1, "CreditCard");
            createPlan_PO.PressSavePlan();
            Thread.Sleep(1000);

            Assert.True(createPlan_PO.CheckValidationError("Name") || createPlan_PO.CheckValidationError("required"), "Name validation missing.");
        }



        // --- NO CAPACITY ---

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_8_Sce7_NoCapacityAvailable()
        {
            InitialStepsForCreatePlan();

            selectClasses_PO.FilterClasses("Thai", "");
            Thread.Sleep(1500);
            selectClasses_PO.SelectClass(class2Name);
            selectClasses_PO.PressContinue();
            Thread.Sleep(1500);

            createPlan_PO.FillInPlanInfo("No capacity Plan", 1, "CreditCard");
            createPlan_PO.PressSavePlan();
            Thread.Sleep(1500);

            bool isErrorVisible = _driver.PageSource.Contains("alert-danger") ||
                                  _driver.PageSource.Contains("capacity") ||
                                  _driver.PageSource.Contains("plazas");

            Assert.True(isErrorVisible, "Capacity error missing in screen");
        }


        // --- NO CLASS ---

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_9_Sce8_NoClassesAvailable()
        {
            InitialStepsForCreatePlan();
            string todayDate = DateTime.Today.ToString("dd/MM/yyyy");

            selectClasses_PO.FilterClasses("Pilates", todayDate);
            Thread.Sleep(1500);

            Assert.True(_driver.PageSource.Contains("No available classes found") || _driver.PageSource.Contains("Loading"), "Empty list message missing from screen.");
        }


        // --- MANDATORY WEEKS ---

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_10_Sce6_WeeksNotProvided()
        {
            InitialStepsForCreatePlan();
            string todayDate = DateTime.Today.ToString("dd/MM/yyyy");

            selectClasses_PO.FilterClasses("", todayDate);
            Thread.Sleep(1500);
            selectClasses_PO.SelectClass(class1Name);
            selectClasses_PO.PressContinue();
            Thread.Sleep(1500);

            createPlan_PO.FillInPlanInfo("No Weeks Plan", 0, "CreditCard");
            createPlan_PO.PressSavePlan();
            Thread.Sleep(1000);

            Assert.True(createPlan_PO.CheckValidationError("Weeks") || createPlan_PO.CheckValidationError("between"), "Week validation missing.");
        }




        // --- MANDATORY PAYMENT ---

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_11_Sce6_PaymentNotProvided()
        {
            InitialStepsForCreatePlan();
            string todayDate = DateTime.Today.ToString("dd/MM/yyyy");

            selectClasses_PO.FilterClasses("", todayDate);
            Thread.Sleep(1500);
            selectClasses_PO.SelectClass(class1Name);
            selectClasses_PO.PressContinue();
            Thread.Sleep(1500);

            createPlan_PO.FillInPlanInfo("No Payment Plan", 4, "-- Select Payment Method --");
            createPlan_PO.PressSavePlan();
            Thread.Sleep(1000);

            Assert.True(createPlan_PO.CheckValidationError("Payment") || createPlan_PO.CheckValidationError("required"), "Payment validation missing.");
        }
    }
}