using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.UC_Incident
{
    public class SelectItemsForReporting_PO : PageObject
    {
        By itemName = By.Id("itemName");
        By itemLocation = By.Id("itemLocation");
        By searchButton = By.Id("searchItems");
        By itemsTable = By.Id("itemsTable");
        By noItemsP = By.Id("noItemsP");

        public void SearchItems(string name, string location)
        {
            WaitForBeingClickable(itemName);
            _driver.FindElement(itemName).SendKeys(name);

            WaitForBeingClickable(itemLocation);
            _driver.FindElement(itemLocation).SendKeys(location);

            WaitForBeingClickable(searchButton);
            _driver.FindElement(searchButton).Click();
        }

        public bool CheckListOfItems(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, itemsTable);
        }

        public bool isTableFound()
        {
            try {
                var b = _driver.FindElement(noItemsP) == null;
                return true;
            } catch (NoSuchElementException e) {
                return false;
            }
        }

        public  SelectItemsForReporting_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }
    }
}
