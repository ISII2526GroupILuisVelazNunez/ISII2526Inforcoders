namespace AppForSEII2526.UIT.Plan
{
    public class SelectClassesForPlan_PO : PageObject
    {
        private By inputClassName = By.Id("inputClassName");
        private By inputFromDate = By.Id("fromDate");
        private By searchClassesButton = By.Id("searchClasses");
        private By tableOfClasses = By.Id("TableOfClasses");
        private By alertWarning = By.CssSelector(".alert.alert-warning");
        private By totalPlanPrice = By.Id("TotalPlanPrice");
        private By continueToCreatePlanButton = By.Id("continueToCreatePlanButton");

        public SelectClassesForPlan_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void SearchClasses(string name, DateTime date)
        {
            WaitForBeingVisible(inputClassName);
            _driver.FindElement(inputClassName).Clear();
            _driver.FindElement(inputClassName).SendKeys(name);

            // Robust Date Entry
            var dateInput = _driver.FindElement(inputFromDate);
            dateInput.SendKeys(date.ToString("dd-MM-yyyy"));

            WaitForBeingClickable(searchClassesButton);

            // Scroll to button to ensure visibility
            new Actions(_driver).MoveToElement(_driver.FindElement(searchClassesButton)).Perform();

            _driver.FindElement(searchClassesButton).Click();

            // Wait for table to update/load
            Thread.Sleep(1500);
        }

        public void SelectClass(string className)
        {
            // IMPROVED: Case-insensitive ID matching
            // 1. Find all buttons whose ID starts with 'classToSelect_'
            var buttons = _driver.FindElements(By.CssSelector("button[id^='classToSelect_']"));
            IWebElement targetButton = null;

            // 2. Loop through them and check if the name part matches 'className' ignoring case
            foreach (var btn in buttons)
            {
                var id = btn.GetAttribute("id");
                // Remove the prefix to get the class name (e.g., "classToSelect_yoga" -> "yoga")
                var namePart = id.Replace("classToSelect_", "");

                if (string.Equals(namePart, className, StringComparison.OrdinalIgnoreCase))
                {
                    targetButton = btn;
                    break;
                }
            }

            // 3. Fallback: if not found by soft match, try exact ID (this will throw the standard error if missing)
            if (targetButton == null)
            {
                targetButton = _driver.FindElement(By.Id($"classToSelect_{className}"));
            }

            // Scroll to the element
            new Actions(_driver).MoveToElement(targetButton).Perform();

            // Wait for it to be clickable using the ID we actually found
            var foundId = targetButton.GetAttribute("id");
            WaitForBeingClickable(By.Id(foundId));

            targetButton.Click();

            // Wait for UI to update (Cart appearing)
            Thread.Sleep(500);
        }

        public bool CheckListOfClasses(List<string[]> expectedClasses)
        {
            return CheckBodyTable(expectedClasses, tableOfClasses);
        }

        public bool IsTableVisible()
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(2));
                wait.Until(ExpectedConditions.ElementIsVisible(tableOfClasses));
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool CheckAlertMessage(string expectedMessage)
        {
            try
            {
                WaitForBeingVisible(alertWarning);
                var text = _driver.FindElement(alertWarning).Text;
                return text.Contains(expectedMessage);
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public string GetTotalPrice()
        {
            WaitForBeingVisible(totalPlanPrice);
            return _driver.FindElement(totalPlanPrice).Text;
        }

        public bool IsContinueButtonEnabled()
        {
            var btn = _driver.FindElement(continueToCreatePlanButton);
            return btn.Enabled;
        }
    }
}