namespace AppForSEII2526.API.Models
{
    public class PayPal : PaymentMethod
    {
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
