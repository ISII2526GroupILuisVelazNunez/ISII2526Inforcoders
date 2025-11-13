using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppForSEII2526.API.Models
{
    public class Purchase
    {
        public Purchase(int id, string city, string country, DateTime date, string description, string street, IList<PurchaseItem> purchaseItems, decimal total_price, PaymentMethod paymentMethod)
        {
            Id = id;
            City = city;
            Country = country;
            Date = date;
            Description = description;
            Street = street;
            PurchaseItems = purchaseItems;
            Total_price = total_price;
            PaymentMethod = paymentMethod;
        }
        public Purchase(string city, string country, DateTime date, string? description, string street, List<PurchaseItem> purchaseItems, decimal total_price, PaymentMethod paymentMethod)
        {
            City = city;
            Country = country;
            Date = date;
            Description = description;
            Street = street;
            PurchaseItems = purchaseItems;
            Total_price = total_price;
            PaymentMethod = paymentMethod;
        }

        //fix error
        public Purchase(string city, string country, DateTime date, string? description, string street)
        {
            City = city;
            Country = country;
            Date = date;
            Description = description;
            Street = street;
            PurchaseItems = new List<PurchaseItem>();
        }

        public Purchase() { } // parameterless

        public int Id {get; set;}
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the city.")]
        public string City { get; set;}
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the country.")]
        public string Country { get; set;}
        [Required]
        public DateTime Date { get; set;}
        [StringLength(200, ErrorMessage = "Description must be shorter than 200 characters.")]
        public string Description { get; set;}
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, specify the street.")]
        public string Street { get; set;}
        [Required]
        public IList<PurchaseItem> PurchaseItems { get; set; }
        [Precision(10, 2)]
        public decimal Total_price { get; set;}
        [Required]
        public PaymentMethod PaymentMethod { get; set;}
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
