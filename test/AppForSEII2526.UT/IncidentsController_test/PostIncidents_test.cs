using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.IncidentDTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.IncidentsController_test
{
    public class PostIncidents_test : AppForSEII25264SqliteUT
    {
        public PostIncidents_test() 
        {
            ApplicationUser user = new ApplicationUser("Mario", "Sanchez");
        }

        public static IEnumerable<object[]> TestCasesFor_CreateIFE()
        {
            var incidentNoItems = new IncidentForCreateDTO(new List<IncidentItemDTO>(), 0, "fake incident",
                DateTime.Today.AddDays(-1), "rock climbing", "Mario Sanchez");

            var incidentItems = new List<IncidentItemDTO>() { new IncidentItemDTO(1, 0, "The pool", 
                "Pool surf incident", "Surf board split in half", "Pool-related")};

            var futureIncident = new IncidentForCreateDTO(incidentItems, 0, "future incident", 
                DateTime.Today.AddDays(1), "Surfing", "Jay Dah");

            var shortNameIncident = new IncidentForCreateDTO(incidentItems, 0, "short name incident",
                DateTime.Today.AddDays(-1), "Surfing", "Lenny");

            var longNameIncident = new IncidentForCreateDTO(incidentItems, 0, "long name incident",
                DateTime.Today.AddDays(-1), "Surfing", "Reece Wayne Shearsmith");

            var allTests = new List<object[]>
            {
                new object[] { incidentNoItems, "Error: you must select at least one item to report!" }, // no incident items
                new object[] { futureIncident, "Error: the date of identification can't be in the future!"}, // identified in the future
                new object[] { shortNameIncident, "Error! Please write the reporter's full name using 2 words"}, // reporter name one word
                new object[] { longNameIncident, "Error! Please write the reporter's full name using 2 words"}, // reporter name more than 2 words
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateIFE))]
        public async Task CreateIncident_Error_test(IncidentForCreateDTO incident, string errorExpected)
        {
            var mock = new Mock<ILogger<IncidentsController>>();
            ILogger<IncidentsController> logger = mock.Object;

            var controller = new IncidentsController(_context, logger);

            // Act
            var result = await controller.CreateIncident(incident);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];
            Assert.StartsWith(errorExpected, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateIncident_Success_test()
        {
            var mock = new Mock<ILogger<IncidentsController>>();
            ILogger<IncidentsController> logger = mock.Object;

            var controller = new IncidentsController(_context, logger);

            var incidentItems = new List<IncidentItemDTO>() { new IncidentItemDTO(1, 0, "The pool",
                "Pool surf incident", "Surf board split in half", "Pool-related")};

            var incidentDTO = new IncidentForCreateDTO(incidentItems, 0, "Pool incident 13-11-25",
                DateTime.Today.AddDays(-1), "Surfing", "Alex Villarejo");

            var expectedDetailDTO = new IncidentDetailDTO(0, "Pool incident 13-11-25", 
                DateTime.Today.AddDays(-1), "Surfing", "Alex Villarejo", incidentItems);

            // Act
            var result = await controller.CreateIncident(incidentDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualIncidentDetailDTO = Assert.IsType<IncidentDetailDTO>(createdResult.Value);

            Assert.Equal(expectedDetailDTO, actualIncidentDetailDTO);
        }
    }
}
