namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(IncidentId), nameof(ItemForExerciseId))]
    public class IncidentItem
    {
        public int IncidentId { get; set; }
        public Incident Incident { get; set; }
        public int ItemForExerciseId { get; set; }
        public ItemForExercise ItemForExercise { get; set; }
        public IncidentPriority IncidentPriority { get; set; }
        public IncidentItem() { }
        public IncidentItem(Incident incident, ItemForExercise ife) 
        {
            Incident = incident;
            ItemForExercise = ife;
            IncidentId = incident.Id;
            ItemForExerciseId = ife.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is IncidentItem item &&
                   IncidentId == item.IncidentId &&
                   ItemForExerciseId == item.ItemForExerciseId;
        }
    }
}
