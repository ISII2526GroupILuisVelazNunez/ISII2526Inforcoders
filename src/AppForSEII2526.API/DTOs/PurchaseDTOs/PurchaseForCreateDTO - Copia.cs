namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class PurchaseDetailDTO
    {
        public PurchaseDetailDTO(int id, PaymentMethod paymentMethod, string street, string city, string country, string? description, IList<PurchaseItem> purchaseItems, decimal total_price)
        {
            Id = id;
            PaymentMethod = paymentMethod;
            Street = street;
            City = city;
            Country = country;
            Description = description; 
            PurchaseItems = purchaseItems;
            Total_price = total_price;
        }
        public int Id {get; set;}
        [Required]
        public string DeliveryAddress => $"{Street}, {City}, {Country}";
        [Precision(10, 2)]
        public decimal Total_price { get; set; }
        [StringLength(200, ErrorMessage = "Description must be shorter than 200 characters.")]
        public string? Description { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the street.")]
        public string Street { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the city.")]
        public string City { get; set;}
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the country.")]
        public string Country { get; set;}
        [Required]
        public IList<PurchaseItem> PurchaseItems { get; set; }
    }
}
