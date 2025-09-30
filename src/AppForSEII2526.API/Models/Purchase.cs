using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppForSEII2526.API.Models
{
    public class Purchase
    {
        public int Id {get; set;}
        public string City { get; set;} 
        public string Country { get; set;}
        public DateTime Date { get; set;}
        public string Description { get; set;}
        public string Street { get; set;}
        public IList<PurchaseItem> PurchaseItems { get; set; }
        [Precision(10, 2)]
        public decimal Total_price { get; set;}
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
