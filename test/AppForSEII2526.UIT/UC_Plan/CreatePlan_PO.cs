namespace AppForSEII2526.UIT.UC_Plan
{
    public class CreatePlan_PO : PageObject
    {

        private By _planNameBy = By.Id("planName");
        private IWebElement _planName() => _driver.FindElement(_planNameBy);
        private IWebElement _weeks() => _driver.FindElement(By.Id("planWeeks"));
        private IWebElement _paymentMethod() => _driver.FindElement(By.Id("paymentMethod"));
        private IWebElement _submitButton() => _driver.FindElement(By.Id("submitPlan"));

        public CreatePlan_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public void FillInPlanInfo(string name, int weeks, string paymentMethodText)
        {
            WaitForBeingVisible(_planNameBy);
            _planName().Clear();
            _planName().SendKeys(name);
            _weeks().Clear();
            _weeks().SendKeys(weeks.ToString());

            SelectElement selectElement = new SelectElement(_paymentMethod());
            selectElement.SelectByText(paymentMethodText);
        }

        public void PressSavePlan()
        {
            var btn = _driver.FindElement(By.Id("submitPlan"));
            // scrolling until the button is visible
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn);
            System.Threading.Thread.Sleep(500);
            btn.Click();
        }

        public bool CheckValidationError(string expectedError)
        {
            return _driver.PageSource.Contains(expectedError);
        }
    }
}