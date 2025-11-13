namespace AppForSEII2526.API.Models
{
    public class ItemForExercise
    {
        public ItemForExercise() { }
        public ItemForExercise(string location, Item item, IList<IncidentItem> incidentItems)
        {
            Location = location;
            Item = item;
            IncidentItems = incidentItems;
        }

        public ItemForExercise(string location, Item item)
        {
            Location = location;
            Item = item;
        }

        public int Id { get; set; }
        public string Location { get; set; }
        public Item Item { get; set; }
        public IList<IncidentItem> IncidentItems { get; set; }
    }
}
