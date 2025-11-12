namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(ClassId),
    nameof(PlanId))]
    public class PlanItem
    {
        public PlanItem()
        {
    
        }

        public PlanItem(Plan plan, Class @class, string goal, decimal price)
        {
            Plan = plan;
            Class = @class;
            Goal = goal;
            Price = price;
        }

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
