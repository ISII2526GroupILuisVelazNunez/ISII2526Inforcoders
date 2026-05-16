namespace AppForSEII2526.UIT.UC_Plan
{
    public class SelectClassesForPlan_PO : PageObject
    {

        
        private By _inputClassNameBy = By.Id("inputClassName");
        private IWebElement _inputClassName() => _driver.FindElement(_inputClassNameBy);
        private IWebElement _inputFromDate() => _driver.FindElement(By.Id("fromDate"));
        private IWebElement _searchButton() => _driver.FindElement(By.Id("searchClasses"));
        private IWebElement _continueButton() => _driver.FindElement(By.Id("continueToCreatePlanButton"));

        public SelectClassesForPlan_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public void FilterClasses(string className, string date)
        {
            WaitForBeingVisible(_inputClassNameBy);

            // filling name
            _inputClassName().Clear();
            _inputClassName().SendKeys(className);

            // select all from date, clear and fill in
            _inputFromDate().SendKeys(Keys.Control + "a");
            _inputFromDate().SendKeys(Keys.Backspace);
            _inputFromDate().SendKeys(date);

            _searchButton().Click();
        }

        public void FilterByNameOnly(string className)
        {
            WaitForBeingVisible(_inputClassNameBy);
            _inputClassName().Clear();
            _inputClassName().SendKeys(className);
            _searchButton().Click();
        }

        public void SelectClass(string className)
        {
            // dynamic id classToSelect_@classItem.Name
            _driver.FindElement(By.Id("classToSelect_" + className)).Click();
        }

        public void PressContinue()
        {
            _continueButton().Click();
        }

        public bool CheckTableOfClasses(List<string[]> expectedClasses)
        {
            return CheckBodyTable(expectedClasses, By.Id("TableOfClasses"));
        }
    }
}