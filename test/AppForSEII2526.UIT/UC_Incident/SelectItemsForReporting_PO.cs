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
        By selectedItemsList = By.Id("selectedItemsList");
        By noItemsP = By.Id("noItemsP");
        By itemsCount = By.Id("itemsCount");
       
        private By ReportButton = By.Id("Report");

        private IWebElement _reportButton() => _driver.FindElement(ReportButton); 


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
                var b = _driver.FindElement(noItemsP);
                return false;
            } catch (NoSuchElementException e) {
                return true;
            }
        }

        public void SelectItems(List<string> names)
        {
            foreach(var name in names)
            {
                WaitForBeingVisible(By.Id($"itemToReport_{name}"));
                _driver.FindElement(By.Id($"itemToReport_{name}")).Click();
            }
        }

        public void RemoveItemFromList(string name)
        {
            WaitForBeingVisible(By.Id($"removeItem_{name}"));
            _driver.FindElement(By.Id($"removeItem_{name}")).Click();
        }

        public bool isReportButtonDisabled()
        {
            return !(_reportButton().Enabled);
        }

        public bool CheckListOfSelectedItems(List<string[]> expectedItems)
        {
            try
            {
                IWebElement SelectedItemsList = _driver.FindElement(selectedItemsList);
                if (SelectedItemsList.GetAttribute("hidden") != null)
                {
                    return expectedItems.Count == 0;
                }

                IList<IWebElement> actualItemRows = SelectedItemsList.FindElements(By.CssSelector(":scope > .row"));
                if (actualItemRows.Count != expectedItems.Count) return false;

                for (int i = 0; i < expectedItems.Count; i++)
                {
                    string[] expectedItem = expectedItems[i];
                    IWebElement actualRow = actualItemRows[i]; 

                    IWebElement itemNameElement = actualRow.FindElement(By.CssSelector(".col-md-6 > p"));                     
                    string actualItemName = itemNameElement.Text.Trim(); 
                                                                         
                    if (!actualItemName.Equals(expectedItem[0])) return false;

                    if (expectedItem.Length > 1)
                    {
                        IWebElement removeButton = actualRow.FindElement(By.CssSelector(".col-md-6 > button"));
                        string actualButtonText = removeButton.Text.Trim(); 

                        if (!actualButtonText.Equals(expectedItem[1])) return false;
                    }
                }

                return true;
            }
            catch (NoSuchElementException e)
            {
                return false;
            }
        }

        public string CheckItemCount()
        {
            IWebElement element = _driver.FindElement(itemsCount);
            return element.Text;
        }

        public  SelectItemsForReporting_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }
    }
}
