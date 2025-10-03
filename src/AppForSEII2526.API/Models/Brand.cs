namespace AppForSEII2526.API.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
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
