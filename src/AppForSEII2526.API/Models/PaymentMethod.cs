namespace AppForSEII2526.API.Models
{
    public abstract class PaymentMethod
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public ApplicationUser User { get; set; }
        public IList<Purchase> Purchases { get; set; }
        public IList<Plan> Plans { get; set; }

    }
}
