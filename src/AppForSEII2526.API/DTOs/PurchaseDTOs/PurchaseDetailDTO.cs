using AppForSEII2526.API.DTOs.ItemDTOs;

namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class PurchaseDetailDTO
    {
        public PurchaseDetailDTO(int id, PaymentMethodForPurchaseDetailDTO paymentMethod, string street, string city, string country, string? description, List<ItemForPurchaseDetailDTO> purchaseItems, decimal total_price)
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
        public int Id { get; set; }
        [Required]
        public string DeliveryAddress => $"{Street}, {City}, {Country}";
        [Precision(10, 2)]
        public decimal Total_price { get; set; }
        [StringLength(200, ErrorMessage = "Description must be shorter than 200 characters.")]
        public string? Description { get; set; }
        public PaymentMethodForPurchaseDetailDTO PaymentMethod { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the street.")]
        public string Street { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the city.")]
        public string City { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the country.")]
        public string Country { get; set; }
        [Required]
        public List<ItemForPurchaseDetailDTO> PurchaseItems { get; set; }
    }
}

