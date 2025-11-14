using AppForSEII2526.API.DTOs.ItemDTOs;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class PurchaseForCreateDTO
    {
        [Required]
        public int PaymentMethodId { get; set; }

        [Required, StringLength(100)]
        public string Street { get; set; }

        [Required, StringLength(50)]
        public string City { get; set; }

        [Required, StringLength(50)]
        public string Country { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        public List<ItemForPurchaseSelectionDTO> PurchaseItems { get; set; } = new();
    }
}
