namespace AppForSEII2526.API.Models
{
    public class Plan
    {
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
    }
}
