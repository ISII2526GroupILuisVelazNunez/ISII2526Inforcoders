using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.UC_Incident
{
    public class DetailIncident_PO : PageObject
    {
        public bool CheckItemsTable(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, By.Id("reportedItems"));
        }

        public bool CheckIncidentDetails(string title, DateTime dateOfIdentification, 
            string exercise, string reporterName)
        {
            System.Threading.Thread.Sleep(1000);
            bool result = true;
            result &= _driver.FindElement(By.Id("Title")).Text.Contains(title);
            result &= _driver.FindElement(By.Id("Exercise")).Text.Contains(exercise);
            result &= _driver.FindElement(By.Id("ReporterName")).Text.Contains(reporterName);

            var actualDateOfIdent = DateTime.Parse(_driver.FindElement(By.Id("DateOfIdent")).Text);
            result &= ((actualDateOfIdent - dateOfIdentification) < new TimeSpan(12, 1, 0));

            return result;
        }

        public DetailIncident_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }
    }
}
