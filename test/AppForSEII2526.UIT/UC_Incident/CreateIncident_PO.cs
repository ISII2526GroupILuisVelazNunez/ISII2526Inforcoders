using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.UC_Incident
{
    public class CreateIncident_PO : PageObject
    {
        By titleField = By.Id("Title");
        By exerciseField = By.Id("Exercise");
        By reporterNameField = By.Id("ReporterName");
        By dateOfIdentificationField = By.Id("DateOfIdentification");

        private By SubmitButton = By.Id("Submit");

        public void fill_report_data(string title, string exercise, string reporterName)
        {
            WaitForBeingClickable(titleField);
            _driver.FindElement(titleField).SendKeys(title);

            WaitForBeingClickable(exerciseField);
            _driver.FindElement(exerciseField).SendKeys(exercise);

            WaitForBeingClickable(reporterNameField);
            _driver.FindElement(reporterNameField).SendKeys(reporterName);
        }

        public void Submit_Report()
        {
            _driver.FindElement(SubmitButton).Click();
        }


        public CreateIncident_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }
    }
}
