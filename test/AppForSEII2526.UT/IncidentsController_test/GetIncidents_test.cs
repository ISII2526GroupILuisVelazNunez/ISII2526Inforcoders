using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ItemForExerciseDTOs;
using AppForSEII2526.API.DTOs.IncidentDTOs;

namespace AppForSEII2526.UT.IncidentsController_test
{
    public class GetIncidents_test : AppForSEII25264SqliteUT
    {
        public GetIncidents_test()
        {
            ApplicationUser user = new ApplicationUser("Jorge", "R");

            var incident = new Incident("A new incident has occurred", "item exploded", "weightlifting",
                DateTime.Today.AddDays(-1), new List<IncidentItem>(), 0, user);

            var type = new TypeItem("Strength", new List<Item>());
            var brand = new Brand("Rocosos", new List<Item>());

            var item = new Item("For weightlifting and such", "dumbbell", 30, 8, 15,
                new List<PurchaseItem>(), type, brand, 13);

            var ife = new ItemForExercise("Maine", item, new List<IncidentItem>());

            incident.IncidentItems.Add(new IncidentItem(incident, ife));

            _context.Add(incident);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetIncident_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<IncidentsController>>();
            ILogger<IncidentsController> logger = mock.Object;
            var controller = new IncidentsController(new ApplicationDbContext(_contextOptions), logger);

            // Act
            var result = await controller.GetIncident(999999);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetIncident_IdLeqZero_test()
        {
            // Arrange
            var mock = new Mock<ILogger<IncidentsController>>();
            ILogger<IncidentsController> logger = mock.Object;
            var controller = new IncidentsController(_context, logger);

            // Act
            var result = await controller.GetIncident(0);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetIncident_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<IncidentsController>>();
            ILogger<IncidentsController> logger = mock.Object;
            var controller = new IncidentsController(_context, logger);

            var expectedIncident = new IncidentDetailDTO(1, 0, "A new incident has occurred",
                DateTime.Today.AddDays(-1), "weightlifting", "Jorge", new List<IncidentItemDTO>());
            expectedIncident.IncidentItems.Add(new IncidentItemDTO(1, 0, "Maine", "dumbbell",
                "For weightlifting and such", "Strength"));

            // Act
            var result = await controller.GetIncident(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var incidentDTOActual = Assert.IsType<IncidentDetailDTO>(okResult.Value);
            //var eq = expectedIncident.Equals(IndidentDTOActual);

            Assert.Equal(expectedIncident, incidentDTOActual);
        }
    }
}