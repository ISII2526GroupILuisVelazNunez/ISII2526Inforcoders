using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class IncidentStateContainer
    {

        public IncidentForCreateDTO Incident { get; private set; } = new IncidentForCreateDTO()
        {
            IncidentItems = new List<IncidentItemDTO>()
        };

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();



        public void AddItemToReport(ItemForReportingDTO ife)
        {
            if (!Incident.IncidentItems.Any(ii => ii.ItemForExerciseId == ife.Id))
                //we add it if it is not in the list
                Incident.IncidentItems.Add(new IncidentItemDTO()
                {
                    ItemForExerciseId = ife.Id,
                    Location = ife.Location,
                    Name = ife.Name,
                    Description = ife.Description,
                    Type = ife.Type,
                }
            );

        }

        //to delete movies from the list of selected movies
        public void RemoveIncidentItemToReport(IncidentItemDTO item)
        {
            Incident.IncidentItems.Remove(item);

        }

        //we eliminate all the movies from the list
        public void ClearReportList()
        {
            Incident.IncidentItems.Clear();

        }

        //we have already finished the process of renting, thus, we create a new Rental 
        public void IncidentProcessed()
        {
            //we have finished the rental process so we create a new object without data
            Incident = new IncidentForCreateDTO()
            {
                IncidentItems = new List<IncidentItemDTO>()
            };
        }
    }
}