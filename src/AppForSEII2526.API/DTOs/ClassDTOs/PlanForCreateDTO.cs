namespace AppForSEII2526.API.DTOs.ClassDTOs
{
    public class PlanForCreateDTO
    {
        [Required(ErrorMessage = "Payment method is required.")]
        public int PaymentMethodId { get; set; }

        [Required(ErrorMessage = "Plan name is required.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Plan name must have between 1 and 20 characters.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "The number of weeks must be inserted.")]
        [Range(1, 52, ErrorMessage = "Number of weeks cannot be less than 1.")]
        public int Weeks { get; set; }

        public string? HealthIssues { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least 1 class must be selected for the plan.")]
        public List<PlanItemForCreateDTO> Items { get; set; } = new List<PlanItemForCreateDTO>();
    }
}
