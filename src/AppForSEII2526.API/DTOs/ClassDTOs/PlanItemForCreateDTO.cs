namespace AppForSEII2526.API.DTOs.ClassDTOs
{
    public class PlanItemForCreateDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid class ID.")]
        public int ClassId { get; set; }

        public string? Goal { get; set; } 
    }
}
// representing a single item in a plan when creating a new plan