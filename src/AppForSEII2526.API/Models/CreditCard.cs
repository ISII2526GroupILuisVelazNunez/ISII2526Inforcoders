namespace AppForSEII2526.API.Models
{
    public class CreditCard:PaymentMethod
    {
        public int CreditCardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
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
