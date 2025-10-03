namespace AppForSEII2526.API.Models
{
    public class IncidentItem
    {
        public int IncidentId { get; set; }
        public int ItemId { get; set; }
        public IncidentPriority IncidentPriority { get; set; }
        public IncidentItem() { }
        public IncidentItem(int incidentId, int itemId) 
        {
            IncidentId = incidentId;
            ItemId = itemId;
        }

        public override bool Equals(object? obj)
        {
            return obj is IncidentItem item &&
                   IncidentId == item.IncidentId &&
                   ItemId == item.ItemId;
        }
    }
}
