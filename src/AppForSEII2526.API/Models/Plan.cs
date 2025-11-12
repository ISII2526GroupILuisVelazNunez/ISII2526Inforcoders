namespace AppForSEII2526.API.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Plan
    {
        // ignore warnings on constructors
        public Plan()
        {
            
        }

        public Plan(string name, string? description, DateTime createdDate, string? healthIssues, decimal totalPrice, int weeks, IList<PlanItem> planItems, PaymentMethod? paymentMethod)
        {
            PaymentMethod = paymentMethod;
        }

        public Plan(string name, string? description, DateTime createdDate, string? healthIssues, decimal totalPrice, int weeks, PaymentMethod? paymentMethod)
        {
            
            Name = name;
            Description = description;
            CreatedDate = createdDate;
            HealthIssues = healthIssues;
            TotalPrice = totalPrice;
            Weeks = weeks;
            PaymentMethod = paymentMethod;
        }

        public DateTime CreatedDate { get; set; }
        public string? Description { get; set; }
        public string? HealthIssues { get; set; }

        [Key]
        public int Id { get; set; }

        [StringLength(20, ErrorMessage = "The name of the Plan can be neither longer than 20 characters nor shorter than 1",
        MinimumLength =1)]
        [Required]
        public string Name { get; set; }

        [Precision(9,2)]
        public decimal TotalPrice { get; set; }

        [Required]
        public int Weeks { get; set; }

        [Required]
        public IList<PlanItem> PlanItems { get; set; } // Class relationship with PlanItem
        [Required]
        public PaymentMethod PaymentMethod { get; set; } // Class relationship with PaymentMethod

    }
}
