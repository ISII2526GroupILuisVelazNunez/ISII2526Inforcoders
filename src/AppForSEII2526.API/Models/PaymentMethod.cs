namespace AppForSEII2526.API.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
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
