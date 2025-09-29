namespace AppForSEII2526.API.Models
{
    public class PlanItem
    {
        public int Id { get; set; }
        //public int ClassId { get; set; }
        //public string Goal { get; set; }
        //public int PlanId { get; set; }

        [Precision(5,2)]
        public decimal Price { get; set; }
        public Plan Plan { get; set; } // Class relationship with Plan
    }
}
