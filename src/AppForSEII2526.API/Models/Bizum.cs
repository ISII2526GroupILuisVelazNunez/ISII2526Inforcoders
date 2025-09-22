namespace AppForSEII2526.API.Models
{
    public class Bizum : PaymentMethod
    {
        public long TelephoneNumber { get; set; }
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
