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

        public void SearchItems(string name)
        {
            WaitForBeingClickable(itemName);
            _driver.FindElement(itemName).SendKeys(name);
            _driver.FindElement(searchButton).Click();
        }

        public  SelectItemsForReporting_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }
    }
}
