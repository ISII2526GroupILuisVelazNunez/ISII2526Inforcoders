namespace AppForSEII2526.API.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [StringLength(20, ErrorMessage = "The name of the brand can be neither longer than 20 characters nor shorter than 1",
        MinimumLength = 1)]
        [Required]
        public string Name { get; set; }
        public IList<Item> Items { get; set; }
        public bool Equals(Object obj)
        {
            if (obj == this)
            {
                return true;
            }
            return false;
        }
    }
}
