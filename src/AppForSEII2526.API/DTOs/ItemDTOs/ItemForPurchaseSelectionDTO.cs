using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.DTOs.ItemDTOs
{
    public class ItemForPurchaseSelectionDTO
    {
        public ItemForPurchaseSelectionDTO() { }

        public ItemForPurchaseSelectionDTO(int id, string name, string brand, string description, decimal purchasePrice, int quantityAvailableForPurchase, int quantityToBuy)
        {
            Id = id;
            Name = name;
            Brand = brand;
            Description = description;
            PurchasePrice = purchasePrice;
            QuantityAvailableForPurchase = quantityAvailableForPurchase;
            QuantityToBuy = quantityToBuy;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The item name must be between 1 and 30 characters.", MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The brand name must be between 1 and 30 characters.", MinimumLength = 1)]
        public string Brand { get; set; }

        [Required]
        [StringLength(70, ErrorMessage = "The description must be between 1 and 70 characters.", MinimumLength = 1)]
        public string Description { get; set; }

        [Required]
        [Precision(10, 2)]
        [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than zero.")]
        public decimal PurchasePrice { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Available quantity cannot be negative.")]
        public int QuantityAvailableForPurchase { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity to buy must be at least 1.")]
        public int QuantityToBuy { get; set; }
    }
}
