namespace AppForSEII2526.API.DTOs.IncidentDTOs
{
    public class IncidentItemDTO
    {
        public IncidentItemDTO(int itemForExerciseId, IncidentPriority incidentPriority, string location, string name, string description, string type)
        {
            ItemForExerciseId = itemForExerciseId;
            IncidentPriority = incidentPriority;
            Location = location;
            Name = name;
            Description = description;
            Type = type;
        }

        public int ItemForExerciseId { get; set; }
        public IncidentPriority IncidentPriority { get; set; }

        public string Location { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // Item.TypeItem.Name

        public override bool Equals(object? obj)
        {
            return obj is IncidentItemDTO dTO &&
                   ItemForExerciseId == dTO.ItemForExerciseId &&
                   IncidentPriority == dTO.IncidentPriority &&
                   Location == dTO.Location &&
                   Name == dTO.Name &&
                   Description == dTO.Description &&
                   Type == dTO.Type;
        }
    }
}
