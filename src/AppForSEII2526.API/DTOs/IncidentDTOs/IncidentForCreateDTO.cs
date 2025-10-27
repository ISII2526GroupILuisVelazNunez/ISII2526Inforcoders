namespace AppForSEII2526.API.DTOs.IncidentDTOs
{
    public class IncidentForCreateDTO
    {
        public IncidentForCreateDTO() 
        {
            IncidentItems = new List<IncidentItemDTO>();
        }

        public IList<IncidentItemDTO> IncidentItems { get; set; }

        public IncidentState IncidentState { get; set; }
        public string Title { get; set; }
        public DateTime DateOfIdentification { get; set; }
        public string Exercise {  get; set; }
        public string ReporterName { get; set; }
    }
}
