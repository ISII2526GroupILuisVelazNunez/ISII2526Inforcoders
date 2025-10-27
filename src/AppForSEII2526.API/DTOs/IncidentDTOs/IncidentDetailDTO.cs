namespace AppForSEII2526.API.DTOs.IncidentDTOs
{
    public class IncidentDetailDTO : IncidentForCreateDTO
    {
        public IncidentDetailDTO(int id, IList<IncidentItemDTO> incidentItems, IncidentState incidentState, 
            string title, DateTime dateOfIdentification, string exercise, string reporterName) 
            : base(incidentItems, incidentState, title, dateOfIdentification, exercise, reporterName)
        {
            Id = id;
        }

        public int Id { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is IncidentDetailDTO dTO &&
                   base.Equals(obj);
        }
    }
}
