namespace AppForSEII2526.API.DTOs.ItemDTOs
{
    public class ItemForPurchaseDTO
    {
        public ItemForPurchaseDTO(int id, string name, string brandName, string description, decimal price, int quantityAvailableForPurchase)
        {
            Id = id;
            Name = name;
            Brand = brandName;
            Description = description;
            PurchasePrice = price;
            QuantityAvailableForPurchase = quantityAvailableForPurchase;
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

        public override bool Equals(object? obj)
        {
            var other = obj as ItemForPurchaseDTO;
            if (other == null) return false;

            return Id == other.Id &&
                   Name == other.Name &&
                   Brand == other.Brand &&
                   Description == other.Description &&
                   PurchasePrice == other.PurchasePrice &&
                   QuantityAvailableForPurchase == other.QuantityAvailableForPurchase;
        }
    }
}
