namespace AppForSEII2526.API.Models
{
    public class Class
    {
        public int Capacity { get; set; }
        public DateTime Date { get; set; }
        public int Id { get; set; }

        [StringLength(20, ErrorMessage = "The name of the Plan can be neither longer than 20 characters nor shorter than 1",
        MinimumLength = 1)]
        [Required]
        public string Name { get; set; }

        [Precision(9, 2)]
        public decimal Price { get; set; }
        public IList<PlanItem> PlanItems { get; set; } // Class relationship with PlanItem
        public IList<TypeItem> TypeItems { get; set; } // Class relationship with TypeItem
    }
}
