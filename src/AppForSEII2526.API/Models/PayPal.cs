namespace AppForSEII2526.API.Models
{
    public class PayPal
    {
        public string Email { get; set; }
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
