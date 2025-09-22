namespace AppForSEII2526.API.Models
{
    [Index(nameof(Title), IsUnique = true)]
    public class Incident {
        public int Id { get; set; }
        [StringLength(50, ErrorMessage = "The title should be between 1 and 50 characters long", MinimumLength = 1)]
        public string Title { get; set; }
        [StringLength(256, ErrorMessage = "The description should be at most 256 characters long")]
        public string Description { get; set; }
        [StringLength(32, ErrorMessage = "The exercise name should be between 1 and 32 characters long")]
        public string Exercise { get; set; }
        public DateTime DateOfIdentification { get; set;}
    }
}
