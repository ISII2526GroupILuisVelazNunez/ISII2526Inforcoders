namespace AppForSEII2526.API.Models
{
    public class Item
    {
        public Item() { }

        public Item(string description) 
        {
            Description = description;
            Name = "-";
        }

        public Item(string description, string name, decimal purchasePrice, int quantityAvailableForPurchase, int quantityForRestock, IList<PurchaseItem> purchaseItems, TypeItem typeItem, Brand brand, decimal? restockPrice)
        {
            Description = description;
            Name = name;
            PurchasePrice = purchasePrice;
            QuantityAvailableForPurchase = quantityAvailableForPurchase;
            QuantityForRestock = quantityForRestock;
            PurchaseItems = purchaseItems;
            TypeItem = typeItem;
            Brand = brand;
            RestockPrice = restockPrice;
        }


        public Item(string description, string name, decimal purchasePrice, int quantityAvailableForPurchase, int quantityForRestock, 
            TypeItem typeItem, Brand brand, decimal? restockPrice)
        {
            Description = description;
            Name = name;
            PurchasePrice = purchasePrice;
            QuantityAvailableForPurchase = quantityAvailableForPurchase;
            QuantityForRestock = quantityForRestock;
            TypeItem = typeItem;
            Brand = brand;
            RestockPrice = restockPrice;
            PurchaseItems = new List<PurchaseItem>();
        }

        public int Id { get; set; }
        [StringLength(70, ErrorMessage = "The description of the Item can be neither longer than 30 characters nor shorter than 1",
        MinimumLength = 1)]
        [Required]
        public string Description { get; set; }
        [StringLength(30, ErrorMessage = "The name of the Item can be neither longer than 30 characters nor shorter than 1",
        MinimumLength = 1)]
        [Required]
        public string Name { get; set; }
        [Precision(10, 2)]
        [Required]
        public decimal PurchasePrice { get; set; }
        [Required]
        public int QuantityAvailableForPurchase { get; set; }
        public int QuantityForRestock { get; set; }
        public IList<PurchaseItem> PurchaseItems { get; set; }
        public TypeItem TypeItem { get; set; }
        [Required]
        public Brand Brand { get; set; }
        [Precision(10,2)]
        public decimal? RestockPrice { get; set; }
        public bool Equals(Object obj)
        {
            if (this == obj){
                return true;
            }
            return false;
        }
    }
}
