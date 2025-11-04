namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class PurchaseForCreateDTO
    {
        public PurchaseForCreateDTO(int id, PaymentMethod paymentMethod, string street, string city, string country, string? description, IList<PurchaseItem> purchaseItems)
        {
            Id = id;
            PaymentMethod = paymentMethod;
            Street = street;
            City = city;
            Country = country;
            Description = description; 
            PurchaseItems = purchaseItems;
        }
        public int Id {get; set;}
        public PaymentMethod PaymentMethod { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the street.")]
        public string Street { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the city.")]
        public string City { get; set;}
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the country.")]
        public string Country { get; set;}
        [StringLength(200, ErrorMessage = "Description must be shorter than 200 characters.")]
        public string? Description { get; set;}
        [Required]
        public IList<PurchaseItem> PurchaseItems { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Precision(10, 2)]
        public decimal Total_price { get; set; }
    }
}
