namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(ItemId),
    nameof(PurchaseId))]
    public class PurchaseItem
    {
        public PurchaseItem(int itemId, Item item, int quantity, int amount_bought, decimal price, Purchase purchase, int purchaseId)
        {
            ItemId = itemId;
            Item = item;
            Quantity = quantity;
            Amount_bought = amount_bought;
            Price = price;
            Purchase = purchase;
            PurchaseId = purchaseId;
        }

        //only IDs for post
        public PurchaseItem(int itemId,int quantity, int amount_bought, decimal price, int purchaseId)
        {
            ItemId = itemId;
            Quantity = quantity;
            Amount_bought = amount_bought;
            Price = price;
            PurchaseId = purchaseId;
        }

        public int ItemId { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "You must purchase at least one item.")]
        public int Amount_bought {  get; set; }
        [Precision(10, 2)]
        public decimal Price { get; set; }
        public Purchase Purchase { get; set; }
        public int PurchaseId { get; set; }
        public bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            return false;
        }
    }
}
