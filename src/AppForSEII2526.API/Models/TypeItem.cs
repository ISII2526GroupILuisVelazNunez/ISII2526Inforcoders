namespace AppForSEII2526.API.Models
{
    public class TypeItem
    {
        public int Id { get; set; }
        
        [StringLength(20, ErrorMessage = "The name of the Plan can be neither longer than 20 characters nor shorter than 1",
        MinimumLength = 1)]
        [Required]
        public string Name { get; set; }
    }
}
