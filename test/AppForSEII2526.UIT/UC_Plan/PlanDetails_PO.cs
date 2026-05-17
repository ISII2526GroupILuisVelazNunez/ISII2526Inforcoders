namespace AppForSEII2526.UIT.UC_Plan
{
    public class PlanDetails_PO : PageObject
    {

        private By _headerBy = By.TagName("h2");
        private IWebElement _planTitle() => _driver.FindElement(_headerBy);

        public PlanDetails_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public string GetPlanTitle()
        {
            WaitForBeingVisible(_headerBy);
            return _planTitle().Text;
        }

        public bool CheckEnrolledClasses(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, By.CssSelector("table.table"));
        }

        public bool CheckDetailsInfo(string expectedInfo)
        {
            return _driver.PageSource.Contains(expectedInfo);
        }
    }
}