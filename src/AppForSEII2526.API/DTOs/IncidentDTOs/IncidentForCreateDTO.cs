namespace AppForSEII2526.API.DTOs.IncidentDTOs
{
    public class IncidentForCreateDTO
    {
        public IncidentForCreateDTO() 
        {
            IncidentItems = new List<IncidentItemDTO>();
        }

        public IncidentForCreateDTO(IList<IncidentItemDTO> incidentItems, IncidentState incidentState, 
            string title, DateTime dateOfIdentification, string exercise, string reporterName)
        {
            IncidentItems = incidentItems;
            IncidentState = incidentState;
            Title = title;
            DateOfIdentification = dateOfIdentification;
            Exercise = exercise;
            ReporterName = reporterName;
        }

        public IList<IncidentItemDTO> IncidentItems { get; set; }

        public IncidentState IncidentState { get; set; }
        public string Title { get; set; }
        public DateTime DateOfIdentification { get; set; }
        public string Exercise {  get; set; }
        public string ReporterName { get; set; }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

        public override bool Equals(object? obj)
        {
            return obj is IncidentForCreateDTO dTO &&
                   IncidentItems.SequenceEqual(dTO.IncidentItems) &&
                   IncidentState == dTO.IncidentState &&
                   Title == dTO.Title &&
                   CompareDate(DateOfIdentification, dTO.DateOfIdentification) &&
                   Exercise == dTO.Exercise &&
                   ReporterName == dTO.ReporterName;
        }
    }
}
