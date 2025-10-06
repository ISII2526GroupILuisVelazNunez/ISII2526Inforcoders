namespace AppForSEII2526.API.Models
{
    public class PayPal:PaymentMethod
    {
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        [Required]
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
