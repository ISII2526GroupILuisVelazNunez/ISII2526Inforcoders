namespace AppForSEII2526.API.DTOs.ItemDTOs
{
    public class ItemForPurchaseDetailDTO
    {
        public ItemForPurchaseDetailDTO(string name, string brand, int quantityPurchased, decimal price)
        {
            Name = name;
            Brand = brand;
            QuantityPurchased = quantityPurchased;
            Price = price;
        }
        public string Name { get; set; }
        public string Brand { get; set; }
        public int QuantityPurchased { get; set; }
        public decimal Price { get; set; }
    }
}
