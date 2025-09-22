namespace AppForSEII2526.API.Models
{
    public class Plan
    {
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public string HealthIssues { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TotalPrice { get; set; }
        public int Weeks { get; set; }
    }
}
