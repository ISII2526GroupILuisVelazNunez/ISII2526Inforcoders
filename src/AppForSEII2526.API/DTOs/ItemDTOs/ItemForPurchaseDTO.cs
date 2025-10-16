namespace AppForSEII2526.API.DTOs.ItemDTOs
{
    public class ItemForPurchaseDTO
    {
        public ItemForPurchaseDTO(int id, string name, string brandName)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        [StringLength(30, ErrorMessage = "The name of the Item can be neither longer than 30 characters nor shorter than 1",
       MinimumLength = 1)]
        [Required]
        public string Name { get; set; }
        public string Brand { get; set; }
        [StringLength(70, ErrorMessage = "The description of the Item can be neither longer than 30 characters nor shorter than 1",
        MinimumLength = 1)]
        [Required]
        public string Description { get; set; }
        [Precision(10, 2)]
        [Required]
        public decimal PurchasePrice { get; set; }
        [Required]
        public int QuantityAvailableForPurchase { get; set; }
    }
}
