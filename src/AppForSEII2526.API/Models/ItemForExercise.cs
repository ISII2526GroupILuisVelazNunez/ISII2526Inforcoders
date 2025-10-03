namespace AppForSEII2526.API.Models
{
    public class ItemForExercise
    {
        public string Location { get; set; }
        public Item Item { get; set; }
        public IList<IncidentItem> IncidentItems { get; set; }
    }
}
