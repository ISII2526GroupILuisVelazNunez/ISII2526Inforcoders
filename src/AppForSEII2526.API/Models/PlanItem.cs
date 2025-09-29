namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(ClassId),
    nameof(PlanId))]
    public class PlanItem
    {
        public int ClassId { get; set; }
        public string? Goal { get; set; }
        public int PlanId { get; set; }

        [Precision(9,2)]
        public decimal Price { get; set; }

        [Required]
        public Plan Plan { get; set; } // Class relationship with Plan
        public Class Class { get; set; } // Class relationship with Class
    }
}
