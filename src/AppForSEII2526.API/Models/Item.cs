namespace AppForSEII2526.API.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        [Precision(10, 2)]
        public decimal PurchasePrice { get; set; }
        public int QuantityAvailableForComedy { get; set; }
        public int QuantityForRestock { get; set; }
        public IList<PurchaseItem> PurchaseItems { get; set; }
        public TypeItem TypeItem { get; set; }
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
