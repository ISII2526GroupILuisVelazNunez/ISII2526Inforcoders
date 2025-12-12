namespace AppForSEII2526.UIT.UC_Plan
{
    public class CreatePlan_PO : PageObject
    {
        // Locators
        By inputPlanName = By.Id("planName");
        By inputDescription = By.Id("planDescription");
        By inputWeeks = By.Id("planWeeks");
        By inputHealthIssues = By.Id("healthIssues");
        By selectPaymentMethod = By.Id("paymentMethod");
        By submitPlanButton = By.Id("submitPlan");

        // No ID on Modify button, using text content
        By modifyClassesButton = By.XPath("//button[contains(text(), 'Modify Selected Classes')]");

        // Success/Error elements
        By alertSuccess = By.CssSelector(".alert.alert-success");
        By alertDanger = By.CssSelector(".alert.alert-danger");
        By validationMessage = By.CssSelector(".validation-message");

        public CreatePlan_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void FillPlanDetails(string name, string description, int weeks, string healthIssues, string paymentMethodValue)
        {
            WaitForBeingVisible(inputPlanName);

            _driver.FindElement(inputPlanName).Clear();
            _driver.FindElement(inputPlanName).SendKeys(name);

            _driver.FindElement(inputDescription).Clear();
            _driver.FindElement(inputDescription).SendKeys(description);

            _driver.FindElement(inputWeeks).Clear();
            _driver.FindElement(inputWeeks).SendKeys(weeks.ToString());

            _driver.FindElement(inputHealthIssues).Clear();
            _driver.FindElement(inputHealthIssues).SendKeys(healthIssues);

            // Select Payment Method by Value (e.g., "1")
            var paymentSelect = new SelectElement(_driver.FindElement(selectPaymentMethod));
            paymentSelect.SelectByValue(paymentMethodValue);
        }

        public void ClickSavePlan()
        {
            WaitForBeingClickable(submitPlanButton);

            // Scroll to ensure visibility
            new Actions(_driver).MoveToElement(_driver.FindElement(submitPlanButton)).Perform();

            _driver.FindElement(submitPlanButton).Click();
            Thread.Sleep(1500); // Wait for API response/Success UI
        }

        public void ClickModifySelectedClasses()
        {
            WaitForBeingClickable(modifyClassesButton);
            // Scroll to ensure visibility
            new Actions(_driver).MoveToElement(_driver.FindElement(modifyClassesButton)).Perform();

            _driver.FindElement(modifyClassesButton).Click();
            Thread.Sleep(500);
        }

        public bool CheckSuccessMessage()
        {
            try
            {
                WaitForBeingVisible(alertSuccess);
                return _driver.FindElement(alertSuccess).Text.Contains("Plan created successfully");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckPlanDetailsVisible(string planName)
        {
            try
            {
                // Check if the header with the plan name exists
                // Waiting for the element to appear first is safer
                var header = By.XPath($"//h2[contains(text(), '{planName}')]");
                WaitForBeingVisible(header);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckValidationError()
        {
            try
            {
                // Check for either field validation messages or the global alert danger
                return _driver.FindElements(validationMessage).Any() ||
                       _driver.FindElements(alertDanger).Any();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}